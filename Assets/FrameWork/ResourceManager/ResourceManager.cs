
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UObject = UnityEngine.Object;
namespace XFrameWork
{ 
public class ResourceManager : FrameWorkModule,IResourceManager
{
        public void Init(IPathConfig pathConfig, Action initSuccess,Action<string>initFail)
        {
            m_pathConfig = pathConfig;
            if (m_helper == null)
            {
                GameObject obj = new GameObject("ResourceManager");
                UObject.DontDestroyOnLoad(obj);
                m_helper = obj.AddComponent<RMMonoHelper>();
            }
            m_badAssetBundleInfo = new AssetBundleInfo(); // asset == null;
            m_helper.TryStartCoroutine(LoadManifest(initSuccess, initFail)); 
        }

        // just like LoadResource("ui_hero","ui_hero",GameObject,onLoaded);  LoadResource("ui_hero","ui_hero",GameObject,onLoaded);
        public void LoadResource(string assName, string resName, Type type, OnResourceLoaded onLoaded)
        {
            // add a request -----
            if (string.IsNullOrEmpty(assName))
            {
                // load resource ---
                m_helper.TryStartCoroutine(RealLoadResource(resName, type, onLoaded));

            }
            else // load asset 
            {

                // 如果loaded 了----- 是否应该直接load 呢--- i think so -------- 写到里面可以统一，似乎，回头再想想 ---
                AssetBundleInfo ab = GetLoadedAssetBundle(assName); 
                if (ab != null)
                {
                    m_helper.TryStartCoroutine(LoadAsset(ab, resName, type, onLoaded));
                    return;
                }

                LoadAssetRequest request = m_loadRequestPool.Get();
                request.assetType = type;
                request.resName = resName;
                request.onLoaded = onLoaded;

                XList<LoadAssetRequest> requests = null;
                if (!m_LoadRequests.TryGetValue(assName, out requests))
                {
                    requests =GetRequestList();
                    requests.Add(request);
                    m_LoadRequests.Add(assName, requests);
                    m_helper.TryStartCoroutine(LoadAssetBundle(assName, resName, type, onLoaded));
                }
                else
                {
                    requests.Add(request);
                }
            }
        }

        public override void ShutDown()
        {
            // mono2.0 的bug - foreach 会造成内存泄露 --
            Dictionary<string, AssetBundleInfo>.Enumerator assetEnumerator = m_loadedAssetBundles.GetEnumerator();
            while (assetEnumerator.MoveNext())
            {
                assetEnumerator.Current.Value.Release();
            }
            assetEnumerator.Dispose();
            m_loadedAssetBundles.Clear();

            //  立刻释放 unload 的资源 ---
            assetEnumerator = m_unLoadAssetBundles.GetEnumerator();
            while (assetEnumerator.MoveNext())
            {
                assetEnumerator.Current.Value.Release();
            }
            assetEnumerator.Dispose();

            m_unLoadAssetBundles.Clear();
            m_realUnLoadAssetBundles.Clear();

            m_assetBundleInfoPool.Clear();
            m_assetBundleManifest = null;
            m_dependencies.Clear();
            m_pathConfig = null;
            m_allManifest = null;
            m_badAssetBundleInfo = null;
            GameObject.DestroyImmediate(m_helper.gameObject);
            m_helper = null;
        }

        public override void Update(float deltaTime)
        {
            //检查 unloadassetBundles -- 如果cachetime <=0 则真正释放
            if (m_unLoadAssetBundles.Count > 0)
            {
                m_realUnLoadAssetBundles.Clear();
                // mono2.0 的bug - foreach 会造成内存泄露 --
                Dictionary<string, AssetBundleInfo>.Enumerator assetEnumerator = m_unLoadAssetBundles.GetEnumerator();
                while (assetEnumerator.MoveNext())
                {
                    AssetBundleInfo bdInfo = assetEnumerator.Current.Value;
                    bdInfo.UpdateCacheTime(deltaTime);
                    if (bdInfo.CanUnload())
                    {
                        m_realUnLoadAssetBundles.Add(assetEnumerator.Current.Key);
                        bdInfo.Release();
                        m_assetBundleInfoPool.Push(bdInfo);
                    }
                }
                assetEnumerator.Dispose();

                int unLoadAssetCount = m_realUnLoadAssetBundles.size;
                if (unLoadAssetCount > 0)
                {
                    for (int i=0;i< unLoadAssetCount;i++)
                    {
                        m_unLoadAssetBundles.Remove(m_realUnLoadAssetBundles[i]);
                    }
                }
            }
        }

        IEnumerator LoadManifest(Action initSuccess, Action<string>initFail)
        {
            // 只可能是从 StreamingAssets 里面 ， 或者download path 里面 ----
            string url = m_pathConfig.getManifestPath();
            WWW loader = new WWW(url);
            yield return loader;
            if (!string.IsNullOrEmpty(loader.error))
            {
                initFail("Load Manifest Url:"+url+" error: "+loader.error);
                yield break;
            }
            AssetBundle assetObj = loader.assetBundle;
            AssetBundleRequest request = assetObj.LoadAllAssetsAsync<AssetBundleManifest>();
            yield return request;
            if (request == null || request.asset == null)
            {
                initFail("Load Manifest Url: "+url+" error: no AssetBundleManifest in the Bundle file");
                yield break;
            }
            m_assetBundleManifest = request.asset as AssetBundleManifest;
            m_allManifest = m_assetBundleManifest.GetAllAssetBundles();
            initSuccess();
        }

        // 这个简单一点，就不加正在加载的判断了。
        IEnumerator RealLoadResource(string resName, Type type, OnResourceLoaded onLoaded)
        {
            // 直接从Resource 里面加载
            ResourceRequest request = Resources.LoadAsync(resName, type);
            yield return request;
            onLoaded(request.asset);
        }

        IEnumerator  LoadAssetBundle(string assName,string resName, Type type, OnResourceLoaded onLoaded)
        {
            yield return m_helper.TryStartCoroutine(RealLoadAsseBundle(assName, false));
            // wait all dependcies over ----
            AssetBundleInfo _info = null;
            while ((_info = GetLoadedAssetBundle(assName))==null)
            {
                yield return 0;
            }
            // load object and call function-------
            m_helper.TryStartCoroutine(LoadAsset(_info, resName, type, onLoaded));
        }
        Dictionary<string,int> m_LoadingAsset = new Dictionary<string,int>(); // 正在loading 的asset  这个主要处理dependence 的，如果像ullua 那样，a，b ，同时依赖c 并且同时开始load，会出现load c 两次的情况 ---
        IEnumerator RealLoadAsseBundle(string assetName,bool isDependence)
        {
            if (isDependence)
            {
                int refCount = 0;
                if (m_LoadingAsset.TryGetValue(assetName, out refCount))// 如果已经在load了，ref++ break;
                {
                    refCount++;
                    m_LoadingAsset[assetName] = refCount;
                    yield break; // 
                }
                else
                {
                    m_LoadingAsset[assetName] = 1;
                }
            }

            // load depencences --
            string[] dependencies = m_assetBundleManifest.GetAllDependencies(assetName);
            int count = dependencies.Length;
            if (count > 0)
            {
                m_dependencies.Add(assetName, dependencies);
                for (int i = 0; i < count; i++)
                {
                    AssetBundleInfo bundleInfo = null;
                    string depName = dependencies[i];
                    if (m_loadedAssetBundles.TryGetValue(depName, out bundleInfo))
                    {
                            bundleInfo.AddReferenceCount();
                    }
                    else
                    {
                        yield return m_helper.TryStartCoroutine(RealLoadAsseBundle(depName, true));
                    }
                }
            }

            // wait for all dependences  loaded------------

            if (count > 0)
            {
                bool waitForDependence = true;
                while (waitForDependence)
                {
                    waitForDependence = false;
                    for (int i = 0; i < count; i++)
                    {
                        AssetBundleInfo bundleInfo = null;
                        string depName = dependencies[i];
                        if (!m_loadedAssetBundles.TryGetValue(depName, out bundleInfo))
                        {
                            waitForDependence = true;
                            break;
                        }
                    }
                    yield return 0;
                }
            }

            string url = m_pathConfig.getAssetDir() + assetName; // + add .unity maybe..
            WWW  load = WWW.LoadFromCacheOrDownload(url, m_assetBundleManifest.GetAssetBundleHash(assetName), 0);
            yield return load;
            AssetBundle assetObj = null;
            if (string.IsNullOrEmpty(load.error)) // no error
            {
                 assetObj = load.assetBundle;
            }
            if (assetObj != null)
            {
                AssetBundleInfo abInfo = m_assetBundleInfoPool.Get();
                abInfo.SetAssetBundle(assetObj);
                m_loadedAssetBundles.Add(assetName, abInfo);
                if (isDependence)
                {
                    int refCount = 0;
                    if (m_LoadingAsset.TryGetValue(assetName, out refCount))// loading zhong -- 等待其他携程load成功---------
                    {
                        abInfo.AddReferenceCount(refCount);
                    }
                }
            } else
            {
                m_loadedAssetBundles.Add(assetName, m_badAssetBundleInfo);
            }
            if (isDependence)
                m_LoadingAsset.Remove(assetName);
        }
        IEnumerator LoadAsset(AssetBundleInfo assetBundle, string resName, Type type, OnResourceLoaded onLoaded)
        {
            if (assetBundle == null || assetBundle.GetAssetBundle() == null)
            {
                onLoaded(null);
                yield break;
            }
               AssetBundleRequest request =  assetBundle.GetAssetBundle().LoadAssetAsync(resName, type);
               yield return request;
              onLoaded(request.asset);
              assetBundle.AddReferenceCount();
        }

        AssetBundleInfo GetLoadedAssetBundle(string assName,bool tryGetCache = true)
        {
            AssetBundleInfo bundle = null;
            if (!m_loadedAssetBundles.TryGetValue(assName, out bundle))
            {
                if (!tryGetCache || !m_unLoadAssetBundles.TryGetValue(assName, out bundle))
                {
                    return null;
                }
                else
                {
                    // get Bundle ---
                    m_loadedAssetBundles[assName] = bundle;
                    m_unLoadAssetBundles.Remove(assName);
                }
            }
            string[] dependencies = null;
            if (!m_dependencies.TryGetValue(assName, out dependencies))
                return bundle;
            int count = dependencies.Length;
            for (int i = 0; i < count; i++)
            {
                if (GetLoadedAssetBundle(dependencies[i]) == null)
                    return null;
            }
            return bundle;
        }
        /// <summary>
        /// 此函数交给外部卸载专用，自己调整是否需要彻底清除AB
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="isThorough"></param>
        public void UnloadAssetBundle(string abName, bool isThorough = false)
        {
            UnloadAssetBundleInternal(abName, isThorough);
            UnloadDependencies(abName, isThorough);
        }

        void UnloadDependencies(string abName, bool isThorough)
        {
            string[] dependencies = null;
            if (!m_dependencies.TryGetValue(abName, out dependencies))
                return;

            // Loop dependencies.
            int count = dependencies.Length;
            for (int i=0; i<count;i++)
            {
                UnloadAssetBundleInternal(dependencies[i], isThorough);
            }
            m_dependencies.Remove(abName);
        }

        void UnloadAssetBundleInternal(string abName, bool isThorough)
        {
            AssetBundleInfo bundle = GetLoadedAssetBundle(abName,false);
            if (bundle == null) return;
            bundle.ReduceReferenceCount();
            if (bundle.NoReference())
            {
                if (m_LoadRequests.ContainsKey(abName) || m_LoadingAsset.ContainsKey(abName))
                {
                    return;     //如果当前AB处于Async Loading过程中，卸载会崩溃，只减去引用计数即可
                }
                bundle.UnLoad(isThorough);
                m_unLoadAssetBundles.Add(abName,bundle);
                m_loadedAssetBundles.Remove(abName);
            }
        }

        RMMonoHelper m_helper;
        private  AssetBundleManifest m_assetBundleManifest = null;
        private String[] m_allManifest;
        private IPathConfig m_pathConfig;
        private AssetBundleInfo m_badAssetBundleInfo; // load 失败的assetbundle 都会被设置成这个,这样每个协程都会返回AssetBundleinfo，由调用者在onload 中处理没有load 成功的情况。

        // 一个request 相当于 对assetBundle 的一次饮用----
        class LoadAssetRequest:ISimplePoolItem
        {
            public Type assetType;
            public  string  resName;
            public OnResourceLoaded onLoaded;

            public void OnGet()
            {
                
            }

            public void OnPush()
            {
                assetType = null;
                resName = "";
                onLoaded = null;
            }
        }

        // load request----
        SimplePool<LoadAssetRequest> m_loadRequestPool = SimplePoolHelper.CreatePool<LoadAssetRequest>();
        private  Dictionary<string, XList<LoadAssetRequest>> m_LoadRequests = new Dictionary<string, XList<LoadAssetRequest>>();

        class AssetBundleInfo : ISimplePoolItem
        {
            public const float REFERENCE_STAY_TIME = 10f;// 没有一次引用，则延迟10s销毁---

            public void AddReferenceCount(int count = 1)
            {
                referencedCount += count;
                if (referencedCount > maxReferenceCount)
                    maxReferenceCount = referencedCount;
            }

            public void ReduceReferenceCount(int count = 1)
            {
                referencedCount -= count;
            }

            public bool NoReference()
            {
                return referencedCount <= 0;
            }

            public void UnLoad(bool thorough)
            {
                isThorough = thorough;
                inCahceTime = maxReferenceCount * REFERENCE_STAY_TIME;
            }

            public void Release()
            {
                if (assetBundle != null)
                {
                    assetBundle.Unload(isThorough);
                }

            }

            public void SetAssetBundle( AssetBundle ab)
            {
                assetBundle = ab;
            }

            public AssetBundle GetAssetBundle()
            {
                return assetBundle;
            }

            public void UpdateCacheTime(float deltaTime)
            {
                inCahceTime -= deltaTime;
            }

            public bool CanUnload()
            {
                return inCahceTime <= 0;
            }

            public void OnGet()
            {

            }
            public void OnPush()
            {
                assetBundle = null;
                referencedCount = 0;
                inCahceTime = 0;
                isThorough = false;
                maxReferenceCount = 0;
            }
            AssetBundle assetBundle;
            int referencedCount;
            float inCahceTime; // ---------
            bool isThorough;//
            int maxReferenceCount;
        }

        SimplePool<AssetBundleInfo> m_assetBundleInfoPool = SimplePoolHelper.CreatePool<AssetBundleInfo>();
        Dictionary<string, AssetBundleInfo> m_loadedAssetBundles = new Dictionary<string, AssetBundleInfo>();
        Dictionary<string, string[]> m_dependencies = new Dictionary<string, string[]>(); //这个一直随着load 扩充好了。
        Dictionary<string, AssetBundleInfo> m_unLoadAssetBundles = new Dictionary<string, AssetBundleInfo>(); // 维护一个延迟释放的 assetBundle列表 ------ 
        XList<string> m_realUnLoadAssetBundles = new XList<string>(); // 缓存真正需要释放的assetbundle 

        XList<LoadAssetRequest> GetRequestList()
        {
            if (m_requestListCache.size > 0)
                return m_requestListCache.Pop();
            return new XList<LoadAssetRequest>();
        }
        void PushRequestList(XList<LoadAssetRequest> list)
        {
            list.Clear();
            if (m_requestListCache == null)
            {
                m_requestListCache = new XList<XList<LoadAssetRequest>>();
            }
            m_requestListCache.Add(list);
        }
        private  XList<XList<LoadAssetRequest>> m_requestListCache;
    }


    public class RMMonoHelper:MonoBehaviour
    {
        // 在editro 模式可用 --
        // ---- 统计类
        // ---- 
        public Coroutine TryStartCoroutine(IEnumerator coroutine)
        {
            return StartCoroutine(coroutine);
        }

    }
}
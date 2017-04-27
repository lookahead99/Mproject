namespace XFrameWork
{
    // 与时俱进 - （滑稽），不支持4.x的加载方式。 只用异步加载 --- 
    using System;
    using UObject = UnityEngine.Object;

    public delegate void OnResourceLoaded(UObject loadedRes);
  
    public interface IResourceManager
    {
        // IPathConfig 用来确认 获取游戏内各种路径 ----
        void Init(IPathConfig pathConfig,Action initSuccess, Action<string>initFail);
        //  assName == “” 说明是从rescource 里面load  ---
        void LoadResource(string assName, string resName,Type type, OnResourceLoaded onLoaded);

         void UnloadAssetBundle(string abName, bool isThorough = false);
    }
}

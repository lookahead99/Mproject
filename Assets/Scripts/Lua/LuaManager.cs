using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XFrameWork;
using XLua;

namespace XGame
{
    public class LuaManager:FrameWorkModule, ILuaManager,ILuaBehaviorManager
    {
        public void Init()
        {
            m_luaEnv = new LuaEnv();
        }
        public object[] DoFile(string filename)
        {
            return m_luaEnv.DoString("require '"+ filename+"'");
        }

        public object[] DoString(string _chunk)
        {
            return m_luaEnv.DoString(_chunk);
        }

        public void SetLuaPath(string luaPath)
        {
            m_luaPath = luaPath;
        }

        public void InitLuaLoader()
        {
            m_luaEnv.AddLoader(LoadLuaFile);
        }

        public void LuaGc()
        {
            m_luaEnv.FullGc();
        }

        public void Release()
        {
           
        }

        public override void ShutDown()
        {
            m_luaEnv.Dispose();
            m_luaEnv = null;
        }

        public override void Update(float deltaTime)
        {
            if (m_luaUpdate != null)
                m_luaUpdate(); // chuan canshu ---------
        }
        public int InitLuaBehaviour(MonoBehaviour behaviour)
        {
            LuaUIBase uiBase = m_luaUIPool.Get();
            int luaIndex = m_curLuaUIIndex;
            int unUsedIndexCount = m_unUsedLuaUIIndexs.size;
            if (unUsedIndexCount > 0)
            {
                luaIndex = m_unUsedLuaUIIndexs.Pop();
                m_luaUIs[luaIndex] = uiBase;
            }
            else
            {
                m_luaUIs.Add(uiBase);
                m_curLuaUIIndex++;
            }
            LuaBehaviour luaScrpit = (LuaBehaviour)behaviour;
            LuaTable scriptEnvTable = CreateNewTableWithLuaFile(luaScrpit.luaFileName);
            uiBase.Init(luaIndex, scriptEnvTable, luaScrpit);
            return luaIndex;
        }
        public void LuaBehaiourUpdate(int id)
        {
            LuaUIBase uiBase = m_luaUIs[id];
            uiBase.Update();

        }
        public void LuaBehaiourStart(int id)
        {
            LuaUIBase uiBase = m_luaUIs[id];
            uiBase.Start();
        }
        public void LuaBehaiourDestory(int id)
        {
            LuaUIBase uiBase = m_luaUIs[id];
            uiBase.Destroy();
        }

        LuaTable CreateNewTable()
        {
            LuaTable newLuaTable = m_luaEnv.NewTable();
            LuaTable meta = m_luaEnv.NewTable();
            meta.Set("__index", m_luaEnv.Global);
            newLuaTable.SetMetaTable(meta);
            meta.Dispose();

            return newLuaTable;
        }
        LuaTable CreateNewTableWithLuaStr(string luaStr)
        {
            LuaTable newLuaTable = CreateNewTable();
            if (!string.IsNullOrEmpty(luaStr))
                 m_luaEnv.DoString(luaStr, "", newLuaTable);
            return newLuaTable;
        }

        LuaTable CreateNewTableWithLuaFile(string luaFile)
        {
            LuaTable newLuaTable = CreateNewTable();
            byte[] luaStr = LoadLuaFile(ref luaFile);
            if (luaStr!=null)
                   m_luaEnv.DoString(luaStr, "", newLuaTable);
            return newLuaTable;
        }

        public void StartWithString(string luaString)
        {
            m_startScriptEnv = CreateNewTable();
            //m_startScriptEnv.Set("self", this);
            //UnityEngine.Debug.LogError("lua start string " + luaString);
            m_luaEnv.DoString(luaString, "StartScript", m_startScriptEnv);
            Action luaStart = m_startScriptEnv.Get<Action>("Start");
            if (luaStart!=null)
                 luaStart();
            m_luaUpdate = m_startScriptEnv.Get<Action>("Update");
        }

        byte[] LoadLuaFile(ref string fileName)
        {
#if UNITY_EDITOR
            fileName = m_luaPath + fileName.Replace('.', '/') + ".lua";
            if (System.IO.File.Exists(fileName))
            {
                return File.ReadAllBytes(fileName);
            }
            else
            {
                return null;
            }
#endif
            // else load from assetbundle ----
            return null;

        }

        private   LuaEnv m_luaEnv;
        private string m_luaPath;
        private LuaTable m_startScriptEnv; // 启动脚本 环境
        private Action m_luaUpdate;

        private int m_curLuaUIIndex = 0;
        private SimplePool<LuaUIBase> m_luaUIPool = new SimplePool<LuaUIBase>();
        private XList<LuaUIBase> m_luaUIs = new XList<LuaUIBase>();
        private XList<int> m_unUsedLuaUIIndexs = new XList<int>();




    }
    public static class LuaConfig
    {
        [CSharpCallLua]
        public static List<Type> CSharpCallLua = new List<Type>()
    {
        typeof(Action),
        typeof(Action<bool>),
    };
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using XLua;
using XFrameWork;
namespace XGame
{
    public class LuaUIBase : ISimplePoolItem
    {
        public void Init(int id, LuaTable scriptEnv,LuaBehaviour behaviour)
        {
            m_scriptEnv = scriptEnv;
            scriptEnv.Set("self", behaviour);
            int count = behaviour.injections.Length;
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    GameObject injection = behaviour.injections[i];
                    if (injection!=null)
                         scriptEnv.Set(injection.name, i);
                }
                LuaUIManager.PushLuaUI(id,behaviour.injections);
            }
            Action luaAwake = scriptEnv.Get<Action>("Awake");
            scriptEnv.Get("Start", out m_luaStart);
            scriptEnv.Get("Update", out m_luaUpdate);
            scriptEnv.Get("OnDestroy", out m_luaOnDestroy);

            if (luaAwake != null)
            {
                luaAwake();
            }
        }

        public void Start()
        {
            if (m_luaStart != null)
            {
                m_luaStart();
            }
        }

        public void Update()
        {
            if (m_luaUpdate != null)
            {
                m_luaUpdate();
            }
        }

        public void Destroy()
        {
            if (m_luaOnDestroy != null)
            {
                m_luaOnDestroy();
            }
            m_luaOnDestroy = null;
            m_luaUpdate = null;
            m_luaStart = null;
            m_scriptEnv.Dispose();
        }

        public void OnGet()
        {
            
        }

        public void OnPush()
        {
            Destroy();
        }

        private Action m_luaStart;
        private Action m_luaUpdate;
        private Action m_luaOnDestroy;

        private LuaTable m_scriptEnv;
    }
}



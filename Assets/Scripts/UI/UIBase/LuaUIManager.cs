using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XGame
{
    public class LuaUIManager
    {
        public static void PushLuaUI(int luaIndex, GameObject[] objs)
        {
            m_luaUIInjections[luaIndex] = objs;
        }

        private static Dictionary<int, GameObject[]> m_luaUIInjections = new Dictionary<int, GameObject[]>();

        public static GameObject GetGameObject(int luaIndex, int injectIndex)
        {
            if (m_luaUIInjections.ContainsKey(luaIndex))
            {
                GameObject[] objs = m_luaUIInjections[luaIndex];
                if (objs!=null && injectIndex < objs.Length)
                    return objs[injectIndex];
            }
            return null;
        }
    }
}


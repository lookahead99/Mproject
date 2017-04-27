using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XLua;
using System;
using UnityEngine.UI;

namespace XGame
{
    [LuaCallCSharp]
    public class LuaBehaviour : MonoBehaviour
    {
        public string luaFileName = "";
        public GameObject[] injections;

        void Awake()
        {
            luaUIIndex = GameCore.Instance.GetLuaBeManager().InitLuaBehaviour(this);
        }
        // Use this for initialization
        void Start()
        {
            GameCore.Instance.GetLuaBeManager().LuaBehaiourStart(luaUIIndex);
            //temp  test code ---
            injections[0].GetComponent<Button>().onClick.AddListener(OnClickButton);
        }

        void OnClickButton()
        {
            Debug.LogError("click button~~~");
        }

        // Update is called once per frame
        void Update()
        {
            GameCore.Instance.GetLuaBeManager().LuaBehaiourUpdate(luaUIIndex);
        }

         void OnDestroy()
        {
            GameCore.Instance.GetLuaBeManager().LuaBehaiourDestory(luaUIIndex);
        }

        private int luaUIIndex;

    }
}


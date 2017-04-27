using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGame;
public class LuaTest : MonoBehaviour {

    // 启动的
    public TextAsset startLuaFile;
    // Use this for initialization
    void Awake ()
    {
        m_luaManager = new LuaManager();
        m_luaManager.Init();
        string luaFilePath = Application.dataPath + "/ZTest/lua/";
        m_luaManager.SetLuaPath(luaFilePath);
        m_luaManager.InitLuaLoader();

       // m_luaManager.DoString("CS.UnityEngine.Debug.Log('hello world')");
        m_luaManager.StartWithString(startLuaFile.text);
    }

     void Update()
    {
        m_luaManager.Update(Time.deltaTime);
    }

    private LuaManager m_luaManager;
}

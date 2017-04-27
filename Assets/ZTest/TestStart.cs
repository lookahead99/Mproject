using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGame;

public class TestStart : MonoBehaviour {

	// Use this for initialization
	void Awake ()
    {
        GameCore.Instance.Init();
        GameCore.Instance.Start();
        string luaFilePath = Application.dataPath + "/ZTest/lua/";
        GameCore.Instance.GetLuaManager().SetLuaPath(luaFilePath);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}

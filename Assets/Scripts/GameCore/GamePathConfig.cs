using System;
using UnityEngine;
using XFrameWork;
namespace XGame
{
    public class GamePathConfig : IPathConfig
    {
        public string getAssetDir()
        {
            return "";
        }

        public string getManifestPath()
        {
            return "";
        }



        public void CheckGamePath()
        {
            // 根据游戏 设定path --------

        }

        public string getLuaPath()
        {
#if UNITY_EDITOR
            return Application.dataPath + "/LuaFile/";
#endif
            return "";
        }
    }
}


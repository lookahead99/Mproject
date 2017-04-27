using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XFrameWork;
using XGame;
/*
 *  启动XFrameWork, 然后启动XGame
 * 
 */
public class GameStart : MonoBehaviour
{
	// Use this for initialization
	void Awake ()
    {
        GameCore.Instance.Init();
        GameCore.Instance.Start();
	}


}

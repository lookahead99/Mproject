﻿// 挂在任意GameObject 用蓝色rect 表示接受碰撞的ui gameobject。

#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RayCastShow : MonoBehaviour {

    static Vector3[] fourCorners = new Vector3[4];
    void OnDrawGizmos()
    {
        foreach (MaskableGraphic g in GameObject.FindObjectsOfType<MaskableGraphic>())
        {
            if (g.raycastTarget)
            {
                RectTransform rectTransform = g.transform as RectTransform;
                rectTransform.GetWorldCorners(fourCorners);
                Gizmos.color = Color.blue;
                for (int i = 0; i < 4; i++)
                    Gizmos.DrawLine(fourCorners[i], fourCorners[(i + 1) % 4]);
            }
        }
    }
}
#endif

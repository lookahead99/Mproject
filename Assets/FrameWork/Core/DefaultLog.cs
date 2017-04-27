
using System;
using UnityEngine;
namespace XFrameWork
{
    public class DefaultLog : ILogger
    {
        public bool enable
        {
            get
            {
                return m_Enable;
            }

            set
            {
                m_Enable = value;
            }
        }
        private bool m_Enable = true;

        public void Error(string info)
        {
            if (m_Enable)
            {
                UnityEngine.Debug.LogError(info);
            }
        }

        public void Print(string info)
        {
            if (m_Enable)
            {
                UnityEngine.Debug.Log(info);
            }
        }

        public void Warning(string info)
        {
            if (m_Enable)
            {
                UnityEngine.Debug.LogWarning(info);
            }
        }
    }
}
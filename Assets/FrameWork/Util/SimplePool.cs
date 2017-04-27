using System.Collections.Generic;
namespace XFrameWork
{

    public interface ISimplePoolItem
    {
        void OnGet();
        void OnPush();

    }

    public class SimplePoolHelper
    {
        public static SimplePool<T> CreatePool<T>() where T : ISimplePoolItem,new()
        {
            SimplePool<T> pool = new SimplePool<T>();
            return pool;
        }

    }

    public class SimplePool<T> where T : ISimplePoolItem,new()
    {
        public T Get()
        {
            T item = default(T);
            if (m_Itmes == null || m_Itmes.size == 0)
            {
                item = new T();
            }
            else
            {
                item = m_Itmes.Pop();
            }
            item.OnGet();
            return item;
        }

        public void Push(T item)
        {
            if (m_Itmes == null)
            {
                m_Itmes = new XList<T>();
            }
            item.OnPush();
            m_Itmes.Add(item);
        }

        public void Clear()
        {
            if (m_Itmes != null)
                m_Itmes.Clear();
        }

        public void Relase()
        {
            if (m_Itmes != null)
                m_Itmes.Release();
            m_Itmes = null;
        }

        private XList<T> m_Itmes;
    }
}

using System;

namespace XFrameWork
{
    public class EventManager : FrameWorkModule,IEventManager
    {
        public void Init(int initCount)
        {
            m_maxEvent = initCount;
            m_handers = new EventHandler[initCount];
        }
        public void RegisterEvent(int id, EventHandler handler)
        {
            if (handler == null)
                return;
            if (id + 1 > m_maxEvent)
            {
                // or throw Exception
                return;
            }
            EventHandler suitHanders = m_handers[id];
            if (suitHanders == null)
                m_handers[id] = handler;
            else
                suitHanders += handler;
        }

        public void UnRegisterEvent(int id, EventHandler handler)
        {
            if (handler == null)
                return;
            if (id + 1 > m_maxEvent)
            {
                // or throw Exception
                return;
            }
            EventHandler suitHanders = m_handers[id];
            if (suitHanders != null)
                suitHanders -= handler;
        }

        public void ExecuteEvent(EventArgs args)
        {
            if (m_Events == null)
                m_Events = new XList<EventArgs>();
            m_Events.Add(args);
        }

        public void ExecuteEventNow(EventArgs args)
        {
              HandeEvent(args);
        }



        private void HandeEvent( EventArgs arg)
        {
            if (arg == EventArgs.Empty)
                return;
            int eventID = arg.eventID;
            if (eventID <0|| eventID + 1 > m_maxEvent)
            {
                // or throw Exception
                return;
            }
            EventHandler suitHanders = m_handers[eventID];
            if (suitHanders != null)
                suitHanders(arg);
        }

        public override void ShutDown()
        {
            if (m_Events != null)
                m_Events.Release();
            if (m_handers != null)
            {
                for (int i=0; i<m_maxEvent;i++)
                    m_handers[i] = null;
                m_handers = null;
            }
        }

        public override void Update(float deltaTime)
        {
            if (m_Events != null)
            {
                int length = m_Events.size;
                for (int i=0; i<length;i++)
                {
                    HandeEvent(m_Events[i]);
                }
                m_Events.Clear();
            }
        }

        private XList<EventArgs> m_Events;
        private  EventHandler[]  m_handers;
        private int m_maxEvent = 0;
    }
}
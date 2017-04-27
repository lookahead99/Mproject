
namespace XFrameWork
{
    public delegate void EventHandler(EventArgs e);
    public class EventArgs
    {
        private int m_event = -1;
        public int eventID
        {
            get
            {
                return m_event;
            }
            set
            {
                m_event = value;
            }
        }
        public static readonly EventArgs Empty;
    }

    public interface IEventManager
    {
        void Init(int initCount);// 最大事件类型 当event 定长时才有意义
        void RegisterEvent(int id, EventHandler handler);

        void UnRegisterEvent(int id, EventHandler handler);

        void ExecuteEvent(EventArgs args);

        void ExecuteEventNow( EventArgs args);
    }
}

namespace XFrameWork
{
    public class FrameWorkCore
    {
        public static FrameWorkCore Instance
        {
            get
            {
                if (m_frameWorkCore == null)
                    m_frameWorkCore = new FrameWorkCore();
                return m_frameWorkCore;
            }
        }
        private static FrameWorkCore m_frameWorkCore;
        private FrameWorkCore() { }

        public void Init(ILogger logger)
        {
            m_logger = logger;
        }

        public void ShutDown()
        {
            for (int i = 0; i < m_modulesCount; i++)
            {
                m_modules[i].ShutDown();
            }
            m_modules.Release();
            m_modules = null;
        }

       public  void AddModule(FrameWorkModule module)
        {
            m_modules.Add(module);
            module.SetLogger(m_logger);
            m_modulesCount++;
        }
        private ILogger m_logger;

        private XList<FrameWorkModule> m_modules = new XList<FrameWorkModule>();
        private int m_modulesCount = 0;
    }
}
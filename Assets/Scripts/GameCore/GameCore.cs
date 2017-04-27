using XFrameWork;

namespace XGame
{
    public class GameCore
    {
        public static GameCore Instance
        {
            get
            {
                if (m_GameCore == null)
                    m_GameCore = new GameCore();
                return m_GameCore;
            }
        }
        private static  GameCore m_GameCore;
        private GameCore() { }

        public void Init()
        {
            FrameWorkCore.Instance.Init(m_logger);
            InitEventManager();
            InitRescourceManager();
            InitLuaManager();
        }

        public void Update()
        {
             
        }

        public void Start()
        {

        }

        void InitEventManager()
        {
            m_eventManager = new EventManager();
            FrameWorkCore.Instance.AddModule(m_eventManager);
            m_eventManager.Init((int)GameEventID.COUNT);
        }
        void InitRescourceManager()
        {
            m_resourceManager = new ResourceManager();
            FrameWorkCore.Instance.AddModule(m_resourceManager);
            m_pathConfig = new GamePathConfig();
            m_resourceManager.Init(m_pathConfig, ResManagerInitSuccess,ResManagerInitFail);
        }

        void ResManagerInitSuccess()
        {
            // 初始化成功 后 才能用loadresource 
        }

        void ResManagerInitFail(string errorInfo)
        {
            m_logger.Error(errorInfo);

        }
        void InitLuaManager()
        {
            m_luaManager = new LuaManager();
            FrameWorkCore.Instance.AddModule(m_luaManager);
            m_luaManager.Init();
            m_luaManager.SetLuaPath(m_pathConfig.getLuaPath());
            m_luaManager.InitLuaLoader();
        }

        public ILuaBehaviorManager GetLuaBeManager() { return m_luaManager; }
        public ILuaManager GetLuaManager() { return m_luaManager; }

        private LuaManager m_luaManager;
        private EventManager m_eventManager;
        private ResourceManager m_resourceManager;
        private DefaultLog m_logger = new DefaultLog();
        private GamePathConfig m_pathConfig;

    }
}


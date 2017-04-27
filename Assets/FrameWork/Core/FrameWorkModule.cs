
namespace XFrameWork
{
    public abstract  class  FrameWorkModule
    {
      public  abstract void ShutDown();
      public abstract void Update(float deltaTime);

        public  void SetLogger(ILogger logger) { m_Logger = logger; }

        ILogger m_Logger;
    }

}


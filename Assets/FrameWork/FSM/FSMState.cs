///  有限状态机的state
///  因为把状态转换都放到State里了，所以比基本的状态机增加了一个addTrans 的方法
///  状态机本身，游戏内应该也不会有更复杂的需求了，所以暂时这样吧。
namespace XFrameWork
{
    public abstract class FSMState
    {
        protected internal void InitTrans(int count)
        {
            if (m_transLations == null)
            {
                m_transLations = new int[count];
            }
            for (int i = 0; i < count; i++)
            {
                m_transLations[i] = UNAVIABLETRANS;
            }
            m_maxTrans = count;
        }
        protected internal void AddTrans(int input, int transTo)
        {
            if (m_transLations == null)
            {
                return;
            }
            if (input < 0 || input >= m_maxTrans)
                return;

            m_transLations[input] = transTo;
        }
        protected internal bool IsInputAviable(int input)
        {
            if (m_transLations == null || input < 0 || input >= m_maxTrans)
            {
                return false;
            }
            return m_transLations[input] != UNAVIABLETRANS;
        }

        protected internal int GetTransState(int input)
        {
            if (m_transLations == null)
            {
                return UNAVIABLETRANS;
            }
            return m_transLations[input];
        }

        protected internal virtual void OnEnter()
        {
        }

        protected internal virtual void OnUpdate(float deltaTime)
        {
        }

        protected internal virtual void OnLeave()
        {
        }
        protected internal virtual void OnDestroy()
        {
            m_transLations = null;
        }

        private readonly int UNAVIABLETRANS = -1;
        private int[] m_transLations;
        private int m_maxTrans;
    }
}
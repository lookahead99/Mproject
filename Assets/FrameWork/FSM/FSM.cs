namespace XFrameWork
{
    public sealed class FSM : FrameWorkModule, IFSM
    {
        public  float currentStateTime
        {
            get
            {
                return m_currentStateTime;
            }
        }
        public void Init(int stateCount, int translations)
        {
            m_maxTrans = translations;
            m_stateCount = stateCount;
            if (m_states == null)
            {
                m_states = new FSMState[m_stateCount];
            }
        }
        public void Start(int stateID)
        {
            m_currentStateTime = 0f;
            m_curState = GetState(stateID);
        }
        public void AddState(int id,FSMState state)
        {
            if (id < 0 || id >= m_stateCount)
                return;
            m_states[id] = state;
            state.InitTrans(m_stateCount);
        }

        public void AddTranslation(int curState, int input, int toState)
        {
            if (curState < 0 || curState >= m_stateCount)
                return;

            if (toState < 0 || toState >= m_stateCount)
                return;

            FSMState state = m_states[toState];
            if (toState == null)
                return;
            state.AddTrans(input, toState);
        }

        public void DoTransLation(int input)
        {
            if (m_Inputs == null)
                m_Inputs = new XList<int>();
                 m_Inputs.Add(input);
        }

        public void DoTransLationNow(int input)
        {
            ChangeState(input);
        }

        private void ChangeState(int input)
        {
            if (m_curState == null)
                return;
            if (!m_curState.IsInputAviable(input))
                return;
            int nextStateIndex = m_curState.GetTransState(input);
            FSMState NextState = GetState(nextStateIndex);
            if (NextState != null)
            {
                m_curState.OnLeave();
                m_curState = NextState;
                m_currentStateTime = 0f;
                NextState.OnEnter();
            }
        }

        private FSMState GetState(int stateID)
        {
            if (stateID < 0 || stateID >= m_stateCount)
                return null;
            if (m_states == null)
                return null;
            return m_states[stateID];
        }

        public override void ShutDown()
        {
            if (m_Inputs != null)
            {
                m_Inputs.Release();
               m_Inputs = null;
            }
            if (m_states != null)
            {
                for (int i = 0; i < m_stateCount; i++)
                {
                    FSMState state = m_states[i];
                    if (state != null)
                        state.OnDestroy();
                }
                m_states = null;
            }
            m_curState = null;
        }
        public override void Update(float deltaTime)
        {
            if (m_curState == null)
                return;
            m_currentStateTime += deltaTime;
            m_curState.OnUpdate(deltaTime);
        }

        private FSMState m_curState;
        private FSMState[] m_states;
        private int m_maxTrans;
        private int m_stateCount;
        private float m_currentStateTime;
        private XList<int> m_Inputs;
    }
}
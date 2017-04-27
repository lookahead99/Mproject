namespace XFrameWork
{
    public interface IFSM 
    {
        //FSMState curState {get;}

        float currentStateTime
        {
            get;
        }

        void Init(int stateCount, int translations);

        void Start(int stateID);

        void AddState(int id,FSMState state);

        void AddTranslation(int curState, int input, int toState);

        void DoTransLation(int input);

        void DoTransLationNow(int input);
    }
}
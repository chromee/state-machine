namespace HC.AI
{
    public interface IState
    {
        void Initialize();
        void StateBegin();
        void StateEnd();
        void SetStateMachine(StateMachine stateMachine);
    }
}
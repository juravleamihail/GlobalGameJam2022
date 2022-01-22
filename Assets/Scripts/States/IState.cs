namespace States
{
    public interface IState
    {
        void OnEnter();
        void Update();
        void OnExit();
        void CanTransitionTo(IState nextState);
    }
}

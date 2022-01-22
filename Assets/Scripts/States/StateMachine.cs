namespace States
{
    public class StateMachine
    {
        private IState _currentState;

        public void ChangeState(IState newState)
        {
            // if (!_currentState.CanTransitionTo(newState))
            // {
            //     return;
            // }
            
            _currentState?.OnExit();
            _currentState = newState;
            _currentState.OnEnter();
        }

        public void UpdateState()
        {
            _currentState?.Update();
        }
    }
}

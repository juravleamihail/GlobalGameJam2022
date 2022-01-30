using System;

namespace States
{
    public class StateMachine
    {
        private StateBase _currentState;
        public StateBase currentState { get {return _currentState;} }
        
        public void ChangeState(StateBase newState, Action<EStates> _setUiCb, Func<EStates, float> _setCameras)
        {
            newState.InitUIAndCameraCb(_setUiCb, _setCameras);
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

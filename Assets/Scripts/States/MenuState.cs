using System;

namespace States
{
    public class MenuState : StateBase
    {
        public MenuState(EStates state, Action onCompleted) : base(state, onCompleted)
        {
            
        }

        public override void OnEnter()
        {
            base.OnEnter();
            _setUiCb?.Invoke(_state);
        }
    }
}
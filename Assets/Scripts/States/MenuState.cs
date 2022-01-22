using System;

namespace States
{
    public class MenuState : StateBase
    {
        public MenuState(EStates state) : base(state)
        {
            
        }

        public override void OnEnter()
        {
            base.OnEnter();
            _setUiCb?.Invoke(_state);
        }
    }
}
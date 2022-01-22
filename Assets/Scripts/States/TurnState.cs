using System;
using UnityEngine;

namespace States
{
    public class TurnState : StateBase
    {
        private float _timer;
        private readonly Action<float> _onTimer;
        private readonly Action<bool> _onMovementToggle;
    
        public TurnState(float time, Action<float> onTimer, Action<bool> onToggle, EStates state) : base(state)
        {
            _timer = time;
            _onTimer = onTimer;
            _onMovementToggle = onToggle;
        }
    
        public override void OnEnter()
        {
            base.OnEnter();
            _setUiCb.Invoke(_state);
            
            ToggleInput(true);
            UpdateTimer(_timer);
        }

        public override void Update()
        {
            base.Update();
            if(!IsTransitionComplate())
            {
                return;
            }
            
            if (_timer > 0)
            {
                _timer -= Time.deltaTime;
                UpdateTimer(_timer);
            }
        }

        public override void OnExit()
        {
            ToggleInput(false);
        }

        private void ToggleInput(bool value)
        {
            _onMovementToggle?.Invoke(value);
        }

        private void UpdateTimer(float timer)
        {
            _onTimer?.Invoke(timer);
        }
    }
}
using System;
using UnityEngine;

namespace States
{
    public class TurnState : IState
    {
        private float _timer;
        private readonly Action<float> _onTimer;
        private readonly Action<bool> _onMovementToggle;
    
        public TurnState(float time, Action<float> onTimer, Action<bool> onToggle)
        {
            _timer = time;
            _onTimer = onTimer;
            _onMovementToggle = onToggle;
        }
    
        public void OnEnter()
        {
            ToggleInput(true);
            UpdateTimer(_timer);
        }

        public void Update()
        {
            if (_timer > 0)
            {
                _timer -= Time.deltaTime;
                UpdateTimer(_timer);
            }
        }

        public void OnExit()
        {
            ToggleInput(false);
        }

        public void CanTransitionTo(IState nextState)
        {
            throw new NotImplementedException();
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
using System;
using UnityEngine;

namespace States
{
    public class TurnState : StateBase
    {
        private float _timer;
        private readonly Action<float> _onTimer;
        private readonly Action<bool> _onMovementToggle;
    
        public TurnState(float time, Action<float> onTimer, Action<bool> onToggle, EStates state, Action onCompleted) : base(state, onCompleted)
        {
            _timer = time;
            _onTimer = onTimer;
            _onMovementToggle = onToggle;
        }
    
        public override void OnEnter()
        {
            base.OnEnter();
            _setUiCb.Invoke(_state);

            GameManager.Instance.ClearInvasions();
            
            SelectFirstAvailableNinjaForPlayer(0);
            SelectFirstAvailableNinjaForPlayer(1);
            
            ToggleInput(true);
            
            UpdateTimer(_timer);
        }

        public void SelectFirstAvailableNinjaForPlayer(int playerID)
        {
            int ninjaIndex = NinjaManager.Instance.GetIndexOfFirstAvailableNinja(playerID);
            Player player = PlayerManager.Instance.GetPlayerByIndex(playerID);
            if (ninjaIndex != -1)
            {
                player.SelectNinja(ninjaIndex);
            }
            else
            {
                Debug.Log("A -1 ninja index was returned for selection at the turn start");
            }
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
                return;
            }

            CompleteState();
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
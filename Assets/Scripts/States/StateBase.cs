using System;
using UnityEngine;

namespace States
{
    public abstract class StateBase
    {
        private readonly Action _onStateCompleted;
        
        protected Action<EStates> _setUiCb;
        protected Func<EStates, float> _setCameras;
        protected readonly EStates _state;

        private float? _transitionTimer = 0;
        private float _delay = 1f;
        
        public StateBase(EStates state, Action onComplated)
        {
            _onStateCompleted = onComplated;
            _state = state;
        }

        public virtual void OnEnter()
        {
            Debug.Log($"Enter state: {_state}");
            _transitionTimer = _setCameras?.Invoke(_state);
        }

        public virtual void Update()
        {
            if(_transitionTimer > 0)
            {
                _transitionTimer -= Time.deltaTime;
                return;
            }

            if (_delay > 0)
            {
                _delay -= Time.deltaTime;
                return;
            }
        }

        public virtual void OnExit()
        {
            Debug.Log($"Exit state: {_state}");
        }

        public virtual bool CanTransitionTo(EStates nextState)
        {
            return true;
        }

        protected bool IsTransitionComplate()
        {
            return _transitionTimer <= 0 && _delay <= 0;
        }
        public void InitUIAndCameraCb(Action<EStates> uiCb, Func<EStates, float> camCb)
        {
            _setUiCb = uiCb;
            _setCameras = camCb;
        }

        protected void ComplateState()
        {
            Debug.Log($"Completed state: {_state}");
            _onStateCompleted?.Invoke();
        }
    }
}

using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace States
{
    public class MoveState : StateBase
    {
        private int _remainingNinjasToMove;
        public MoveState(EStates state, Action onCompleted) : base(state, onCompleted)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();
            _remainingNinjasToMove = NinjaManager.Instance.StartMovePhase(OnMovementComplete);

            Player player0 = PlayerManager.Instance.GetPlayerByIndex(0);
            Player player1 = PlayerManager.Instance.GetPlayerByIndex(1);
            player0.onEnterMoveState?.Invoke();
            player1.onEnterMoveState?.Invoke();
        }

        private void OnMovementComplete()
        {
            --_remainingNinjasToMove;
            if (_remainingNinjasToMove > 0)
            {
                return;
            }

            Wait();
        }
        
        private async void Wait()
        {
            float delay = 3;
            while (delay > 0)
            {
                delay -= Time.deltaTime;
                Task.Yield();
            }

            ComplateState();
        }
    }
}
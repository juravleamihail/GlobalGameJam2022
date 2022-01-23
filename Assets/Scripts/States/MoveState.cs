using System;
using System.Threading;
using System.Threading.Tasks;

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
        
        private async Task Wait() 
        {
            await Task.Delay(2000);
            ComplateState();
        }
    }
}
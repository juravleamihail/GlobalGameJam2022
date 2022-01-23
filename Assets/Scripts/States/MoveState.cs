using System;
using System.Threading;
using System.Threading.Tasks;

namespace States
{
    public class MoveState : StateBase
    {
        public MoveState(EStates state, Action onCompleted) : base(state, onCompleted)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();
            Wait();
        }
        
        private async Task Wait() 
        {
            await Task.Delay(3000);
            ComplateState();
        }
    }
}
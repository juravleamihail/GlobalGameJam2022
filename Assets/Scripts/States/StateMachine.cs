using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    private IState _currentState;

    public void ChangeState(IState newState)
    {
        _currentState?.OnExit();
        _currentState = newState;
        _currentState.OnEnter();
    }

    public void UpdateState()
    {
        _currentState?.Update();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : PlayerInputHandler
{
    public int PlayerIndex { get { return _playerInput.playerIndex; } }


    protected override void OnPlayerMove(InputAction.CallbackContext context)
    {
        base.OnPlayerMove(context);
    }

    protected override void OnPlayerUndo(InputAction.CallbackContext context)
    {
        base.OnPlayerUndo(context);
    }

    protected override void OnPlayerSwitch(InputAction.CallbackContext context)
    {
        base.OnPlayerSwitch(context);
    }

    private void Start()
    {
        Debug.LogError("Player index: " + _playerInput.playerIndex);
    }

}

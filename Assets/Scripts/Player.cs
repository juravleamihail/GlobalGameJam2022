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

    protected override void OnPlayerTrigger(InputAction.CallbackContext context)
    {
        base.OnPlayerTrigger(context);
    }

    private void Start()
    {
        Debug.LogError("Player index: " + _playerInput.playerIndex);
    }

}

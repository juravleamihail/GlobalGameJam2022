using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : PlayerInputHandler
{
    public int PlayerIndex { get { return _playerInput.playerIndex; } }


  
    private void Start()
    {
        Debug.Log("Player index: " + _playerInput.playerIndex);
    }


    protected override void OnPlayerUndo(InputAction.CallbackContext context)
    {
        base.OnPlayerUndo(context);

        Debug.Log("Player index: " + _playerInput.playerIndex +"Undo");
    }

    protected override void OnPlayerSwitch(InputAction.CallbackContext context)
    {
        base.OnPlayerSwitch(context);
        Debug.Log("Player index: " + _playerInput.playerIndex + "Switch");
    }

    protected override void OnPlayerDown(InputAction.CallbackContext context)
    {
        base.OnPlayerDown(context);
        Debug.Log("Player index: " + _playerInput.playerIndex + "down");
    }

    protected override void OnPlayerLeft(InputAction.CallbackContext context)
    {
        base.OnPlayerLeft(context);
        Debug.Log("Player index: " + _playerInput.playerIndex + "left");
    }

    protected override void OnPlayerRight(InputAction.CallbackContext context)
    {
        base.OnPlayerRight(context);
        Debug.Log("Player index: " + _playerInput.playerIndex + "Right");
    }

    protected override void OnPlayerUp(InputAction.CallbackContext context)
    {
        base.OnPlayerUp(context);
        Debug.Log("Player index: " + _playerInput.playerIndex + "Up");
    }
}

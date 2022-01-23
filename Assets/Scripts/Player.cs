using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : PlayerInputHandler
{
    public PlayerTypeSO PlayerType { get; set; }
    private Transform _selectedNinja;
    public Action<GridSystem.Directions> onDrawPathInput { private get; set; }
  
    private void Start()
    {
        Debug.Log("Player index: " + PlayerInput.playerIndex);
    }

    protected override void OnPlayerUndo(InputAction.CallbackContext context)
    {
        base.OnPlayerUndo(context);

        Debug.Log("Player index: " + PlayerInput.playerIndex +"Undo");
    }

    protected override void OnPlayerSwitch(InputAction.CallbackContext context)
    {
        base.OnPlayerSwitch(context);
        Debug.Log("Player index: " + PlayerInput.playerIndex + "Switch");
    }

    protected override void OnPlayerDown(InputAction.CallbackContext context)
    {
        base.OnPlayerDown(context);
        PlayerDrawPath(GridSystem.Directions.Down);
    }

    protected override void OnPlayerLeft(InputAction.CallbackContext context)
    {
        base.OnPlayerLeft(context);
        PlayerDrawPath(GridSystem.Directions.Left);
    }

    protected override void OnPlayerRight(InputAction.CallbackContext context)
    {
        base.OnPlayerRight(context);
        PlayerDrawPath(GridSystem.Directions.Right);
    }

    protected override void OnPlayerUp(InputAction.CallbackContext context)
    {
        base.OnPlayerUp(context);
        PlayerDrawPath(GridSystem.Directions.Up);
    }
    protected void PlayerDrawPath(GridSystem.Directions direction)
    {
        if (!PlayerManager.Instance.canPlayersDrawPaths)
        {
            return;
        }

        onDrawPathInput?.Invoke(direction);
    }
}

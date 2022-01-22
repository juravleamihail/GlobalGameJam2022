using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInputHandler : MonoBehaviour
{
    protected PlayerInput _playerInput;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();

        PlayerInputActions playerActions = new PlayerInputActions();

        if (_playerInput.playerIndex == 0)
        {
            playerActions.PlayerActions.Enable();
            playerActions.PlayerActions.PlayerOneSwitch.performed += OnPlayerSwitch;
            playerActions.PlayerActions.PlayerOneUndo.performed += OnPlayerUndo;
            playerActions.PlayerActions.PlayerOneUp.performed += OnPlayerUp;
            playerActions.PlayerActions.PlayerOneDown.performed += OnPlayerDown;
            playerActions.PlayerActions.PlayerOneLeft.performed += OnPlayerLeft;
            playerActions.PlayerActions.PlayerOneRight.performed += OnPlayerRight;
        }
        else if(_playerInput.playerIndex == 1)
        {
            playerActions.PlayerActions.Enable();
            playerActions.PlayerActions.PlayerTwoSwitch.performed += OnPlayerSwitch;
            playerActions.PlayerActions.PlayerTwoUndo.performed += OnPlayerUndo;
            playerActions.PlayerActions.PlayerOneUp.performed += OnPlayerUp;
            playerActions.PlayerActions.PlayerOneDown.performed += OnPlayerDown;
            playerActions.PlayerActions.PlayerOneLeft.performed += OnPlayerLeft;
            playerActions.PlayerActions.PlayerOneRight.performed += OnPlayerRight;
        }
    }

     protected virtual void OnPlayerUp(CallbackContext context)
    {
    }

    protected virtual void OnPlayerDown(CallbackContext context)
    {

    }

    protected virtual void OnPlayerLeft(CallbackContext context)
    {

    }

    protected virtual void OnPlayerRight(CallbackContext context)
    {

    }

    protected virtual void OnPlayerSwitch(CallbackContext context)
    {
        
    }

    protected virtual void OnPlayerUndo(CallbackContext context)
    {

    }
}

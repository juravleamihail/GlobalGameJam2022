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
            playerActions.PlayerActions.PlayerOneTriggers.performed += OnPlayerTrigger;
            playerActions.PlayerActions.PlayerOneMovement.performed += OnPlayerMove;
        }
        else if(_playerInput.playerIndex == 1)
        {
            playerActions.PlayerActions.Enable();
            playerActions.PlayerActions.PlayerTwoTriggers.performed += OnPlayerTrigger;
            playerActions.PlayerActions.PlayerTwoMovement.performed += OnPlayerMove;
        }
    }

    protected virtual void OnPlayerMove(CallbackContext context)
    {
    }

    protected virtual void OnPlayerTrigger(CallbackContext context)
    {
    }
}

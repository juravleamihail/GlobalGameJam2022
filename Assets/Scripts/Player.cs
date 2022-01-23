using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : PlayerInputHandler
{
    public PlayerTypeSO PlayerType { get; set; }
    private int _selectedNinjaIndex;

    [SerializeField] private Material _pathDrawMaterial;
    public Material pathDrawMaterial { get { return _pathDrawMaterial; } }

    private void Start()
    {
        _pathDrawMaterial = PlayerManager.Instance.pathMaterials[PlayerType.PlayerIndex];
        Debug.Log("Player index: " + PlayerInput.playerIndex);
    }

    protected override void OnPlayerUndo(InputAction.CallbackContext context)
    {
        base.OnPlayerUndo(context);

        UndoDrawPath(false);
    }

    //TODO implement long-press undo

    protected override void OnPlayerSwitch(InputAction.CallbackContext context)
    {
        //TODO replace this with per-key selecting
        base.OnPlayerSwitch(context);
        Debug.Log("Player index: " + PlayerInput.playerIndex + "Switch");
    }

    protected override void OnPlayerDown(InputAction.CallbackContext context)
    {
        base.OnPlayerDown(context);
        TryDrawPath(GridSystem.Directions.Down);
    }

    protected override void OnPlayerLeft(InputAction.CallbackContext context)
    {
        base.OnPlayerLeft(context);
        TryDrawPath(GridSystem.Directions.Left);
    }

    protected override void OnPlayerRight(InputAction.CallbackContext context)
    {
        base.OnPlayerRight(context);
        TryDrawPath(GridSystem.Directions.Right);
    }

    protected override void OnPlayerUp(InputAction.CallbackContext context)
    {
        base.OnPlayerUp(context);
        TryDrawPath(GridSystem.Directions.Up);
    }

    private void TryDrawPath(GridSystem.Directions direction)
    {
        if (!PlayerManager.Instance.isTurnState)
        {
            return;
        }

        int playerIndex = PlayerType.PlayerIndex;
        NinjaManager.Instance.TryDrawPath(playerIndex, _selectedNinjaIndex, direction);
    }

    private void UndoDrawPath(bool longUndo = false)
    {
        if (!PlayerManager.Instance.isTurnState)
        {
            return;
        }

        int playerIndex = PlayerType.PlayerIndex;
        NinjaManager.Instance.UndoDrawPath(playerIndex, _selectedNinjaIndex, longUndo);
    }

    private void SelectNinja(int ninjaIndex)
    {
        //TODO clamp ninja indices
        _selectedNinjaIndex = ninjaIndex;
    }

    public void DEBUG_DrawPath(GridSystem.Directions direction)
    {
        int playerIndex = PlayerType.PlayerIndex;
        NinjaManager.Instance.TryDrawPath(playerIndex, _selectedNinjaIndex, direction);
    }

    public void DEBUG_UndoPath(bool longUndo)
    {
        int playerIndex = PlayerType.PlayerIndex;
        NinjaManager.Instance.UndoDrawPath(playerIndex, _selectedNinjaIndex, longUndo);
    }

    public void DEBUG_SelectNinja(int ninjaIndex)
    {
        SelectNinja(ninjaIndex);
    }
}

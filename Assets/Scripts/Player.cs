using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using States;

public class Player : PlayerInputHandler
{
    public PlayerTypeSO PlayerType { get; set; }
    private int _selectedNinjaIndex = 0;
    public int selectedNinjaIndex { get { return _selectedNinjaIndex; } }

    public int kills { get; private set; } = 0;
    public int aliveNinjas { get; private set; } = 3;

    [SerializeField] private Material _pathDrawMaterialWhiteTile;
    public Material pathDrawMaterialWhiteTile { get { return _pathDrawMaterialWhiteTile; } }

    [SerializeField] private Material _pathDrawMaterialBlackTile;
    public Material pathDrawMaterialBlackTile { get { return _pathDrawMaterialBlackTile; } }

    public Action onEnterMoveState;

    private void Start()
    {
        _pathDrawMaterialWhiteTile = PlayerManager.Instance.pathMaterialsForWhiteTiles[PlayerType.PlayerIndex];
        _pathDrawMaterialBlackTile = PlayerManager.Instance.pathMaterialsForBlackTiles[PlayerType.PlayerIndex];
        Debug.Log("Player index: " + PlayerInput.playerIndex);
    }

    //TODO uncomment these and remove the OldStyle ones when figuring out how to implement the long-press undo

    //protected override void OnPlayerUndo(InputAction.CallbackContext context)
    //{
    //    base.OnPlayerUndo(context);

    //    UndoDrawPath(false);
    //}

    ////TODO implement long-press undo

    //protected override void OnPlayerSwitch(InputAction.CallbackContext context)
    //{
    //    //TODO replace this with per-key selecting
    //    base.OnPlayerSwitch(context);
    //    Debug.Log("Player index: " + PlayerInput.playerIndex + "Switch");
    //}

    //protected override void OnPlayerDown(InputAction.CallbackContext context)
    //{
    //    base.OnPlayerDown(context);
    //    TryDrawPath(GridSystem.Directions.Down);
    //}

    //protected override void OnPlayerLeft(InputAction.CallbackContext context)
    //{
    //    base.OnPlayerLeft(context);
    //    TryDrawPath(GridSystem.Directions.Left);
    //}

    //protected override void OnPlayerRight(InputAction.CallbackContext context)
    //{
    //    base.OnPlayerRight(context);
    //    TryDrawPath(GridSystem.Directions.Right);
    //}

    //protected override void OnPlayerUp(InputAction.CallbackContext context)
    //{
    //    base.OnPlayerUp(context);
    //    TryDrawPath(GridSystem.Directions.Up);
    //}

    private void TryDrawPath(GridSystem.Directions direction)
    {
        TurnState turnState = GameManager.Instance.GetCurrentState() as TurnState;

        if (turnState == null) //if we are not in the turn state
        {
            return;
        }

        int playerIndex = PlayerType.PlayerIndex;
        NinjaManager.Instance.TryDrawPath(playerIndex, _selectedNinjaIndex, direction);
    }

    private void UndoDrawPath(bool longUndo = false)
    {
        TurnState turnState = GameManager.Instance.GetCurrentState() as TurnState;

        if (turnState == null) //if we are not in the turn state
        {
            return;
        }

        int playerIndex = PlayerType.PlayerIndex;
        NinjaManager.Instance.UndoDrawPath(playerIndex, _selectedNinjaIndex, longUndo);
    }

    public void SelectNinja(int ninjaIndex)
    {
        //TODO clamp ninja indices
        UIManager.Instance.SelectCharacter(PlayerType.PlayerIndex, ninjaIndex);
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

    public void IncrementKills()
    {
        ++kills;
        UIManager.Instance.AddKill(PlayerType.PlayerIndex, kills);
        if (GameManager.Instance.winByNrOfKills && kills >= GameManager.Instance.killsToWin)
        {
            GameManager.Instance.PrepareToShowWinScreen(PlayerType.PlayerIndex);
        }
    }

    public void RefreshAliveNinjas(int inAliveNinjas)
    {
        aliveNinjas = inAliveNinjas;
        if (aliveNinjas <= 0)
        {
            int thisPlayerIndex = PlayerType.PlayerIndex;
            int otherPlayerIndex = thisPlayerIndex == 0 ? 1 : 0;
            GameManager.Instance.PrepareToShowWinScreen(otherPlayerIndex);
        }
    }

    public void OldStyleUndo(bool longPress)
    {
        UndoDrawPath(longPress);
    }

    public void OldStyleSelect(int ninjaIndex)
    {
        SelectNinja(ninjaIndex);
    }

    public void OldStyleDown()
    {
        TryDrawPath(GridSystem.Directions.Down);
    }

    public void OldStyleLeft()
    {
        TryDrawPath(GridSystem.Directions.Left);
    }

    public void OldStyleRight()
    {
        TryDrawPath(GridSystem.Directions.Right);
    }

    public void OldStyleUp()
    {
        TryDrawPath(GridSystem.Directions.Up);
    }
}

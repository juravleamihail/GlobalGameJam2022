using DefaultNamespace;
using States;
using UI;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [Space(10), SerializeField] private GameSettingsSO _gameSettings;
    [Space(25), SerializeField] private MainMenu _mainMenuUI;
    [SerializeField] private Hud _hudUI;
   
    private StateMachine _stateMachine;
    private GridSystem _grid;

    public override void Awake()
    {
        base.Awake();
        _stateMachine = new StateMachine();
        _grid = new GridSystem(_gameSettings.GridSize, _gameSettings.TileSize);
    }

    private void Update()
    {
        _stateMachine.UpdateState();
    }

    public void StartGame()
    {
        _stateMachine.ChangeState(new TurnState(_gameSettings.Timer, _hudUI.UpdateTimer,null));
        ToggleUI(UIStates.Gameplay);
    }

    public void BackToMainMenu()
    {
        ToggleUI(UIStates.MainMenu);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    private void ToggleUI(UIStates uiState)
    {
        _mainMenuUI.gameObject.SetActive(uiState == UIStates.MainMenu);
        _hudUI.gameObject.SetActive(uiState == UIStates.Gameplay);
    }

    private enum UIStates{
        MainMenu,
        Gameplay
    }

    public Vector3 ConvertGridCoordsToVector3(uint gridX, uint gridY)
    {
        return _grid.ConvertGridCoordsToVector3(gridX, gridY);
    }

    public Vector2Int ConvertVector3CoordsToGrid(float worldX, float worldZ)
    {
        return _grid.ConvertVector3ToGridCoords(worldX, worldZ);
    }

    public Vector2Int MoveOneTileOnGrid(uint currentGridX, uint currentGridY, GridSystem.Directions direction)
    {
        return _grid.MoveOneTileOnGrid(currentGridX, currentGridY, direction);
    }

    public Vector3 MoveOneTileInWorld(Vector3 currentPosition, GridSystem.Directions direction)
    {
        return _grid.MoveOneTileInWorld(currentPosition, direction);
    }
}
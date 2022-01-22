using DefaultNamespace;
using States;
using UI;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [Space(10), SerializeField] private GameSettingsSO _gameSettings;
    [Space(25), SerializeField] private MainMenu _mainMenuUI;
    [SerializeField] private Hud _hudUI;
   
	[SerializeField] CameraController _cameraController;
   
    private StateMachine _stateMachine;
    private GridSystem _grid;

    public bool isUsingMovePoints; 
    public bool isUsingMaxDistance;

    [SerializeField] private uint _movePoints;
    public uint movePoints { get { return _movePoints; } }
    [SerializeField] private uint _maxDistance;
    public uint maxDistance { get { return _maxDistance; } }

    public override void Awake()
    {
        base.Awake();
        _stateMachine = new StateMachine();
        _grid = new GridSystem(_gameSettings.GridSize, _gameSettings.TileSize);
        BackToMainMenu();
    }

    private void Update()
    {
        _stateMachine.UpdateState();
    }

    private void ChangeState(StateBase state)
    {
       _stateMachine.ChangeState(state, ToggleUI, _cameraController.ChangeState);
    }

    [ContextMenu("triggerTurnState")]
    public void StartGame()
    {
       ChangeState(CreateTurnState());
    }
  
    public void BackToMainMenu()
    {
       ChangeState(CreateMaineMenuState());
    }
   
    [ContextMenu("triggerCombat")]
    public void GoToCombat()
    {
       ChangeState(CreateCombatState());
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

    /// <summary>
    /// Toggles between to UI elements
    /// Main Menu & Hud
    /// </summary>
    /// <param name="state"></param>
    private void ToggleUI(EStates state)
    {
       _mainMenuUI.gameObject.SetActive(state == EStates.MainMenu);
       _hudUI.gameObject.SetActive(state == EStates.Gameplay);
    }

    //Factory Pattern
    private StateBase CreateMaineMenuState()
    {
       return new MenuState(EStates.MainMenu);
    }

    private StateBase CreateTurnState()
    {
       return new TurnState(
          _gameSettings.Timer,
          _hudUI.UpdateTimer,
          null,
          EStates.Gameplay
       );
    }

    private StateBase CreateCombatState()
    {
       return new CombatState(EStates.Combat);
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

    public bool IsOnGrid(Vector2Int gridPos)
    {
        return _grid.IsOnGrid(gridPos);
    }
    public bool IsOnGrid(Vector3 worldPos)
    {
        return _grid.IsOnGrid(worldPos);
    }
}
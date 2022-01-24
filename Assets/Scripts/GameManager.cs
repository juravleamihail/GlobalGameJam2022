using DefaultNamespace;
using States;
using UI;
using System;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [Space(10), SerializeField] private GameSettingsSO _gameSettings;
    [Space(25), SerializeField] private MainMenu _mainMenuUI;
    [SerializeField] private Hud _hudUI;
    [SerializeField] private GameObject _gameplayUI;
   
	[SerializeField] CameraController _cameraController;
   
    private StateMachine _stateMachine;
    private GridSystem _grid;

    public bool isUsingMovePoints; 
    public bool isUsingMaxDistance;
    public bool winByNrOfKills;

    public bool canMoveOnlyOneNinjaPerTurn;
    public bool invisiblePaths;
    public bool canPathIntersectItself;

    [SerializeField] private uint _movePoints;
    public uint movePoints { get { return _movePoints; } }
    [SerializeField] private uint _maxDistance;
    public uint maxDistance { get { return _maxDistance; } }

    [SerializeField] private uint _killsToWin;
    public uint killsToWin { get { return _killsToWin; } }

    public Action<bool> onTurnStateChanged { private get; set; }

    //use these constants to signal exceptions; negative values for positions should not be used anywhere in the game
    public Vector3 vector3Exception { get; } = new Vector3(-100f, -100f, -100f);
    public Vector2Int vector2IntException { get; } = new Vector2Int(-1, -1);
    public float GetGridSize => _gameSettings.GridSize;

    public override void Awake()
    {
        base.Awake();
        _stateMachine = new StateMachine();
        _grid = new GridSystem(_gameSettings.GridSize, _gameSettings.TileSize);
    }

    private void Start()
    {
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

   
    public void StartGame()
    {
        GoToTurnState();
    }

    public void BackToMainMenu()
    {
       ChangeState(CreateMaineMenuState());
    }
    
    [ContextMenu("triggerTurnState")]
    private void GoToTurnState()
    {
        ChangeState(CreateTurnState());
    }

    private void GotoMoveState()
    {
        ChangeState(CreateMoveState());
    }
    
    [ContextMenu("triggerCombat")]
    public void GoToCombatState()
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
        _gameplayUI.SetActive(uiState == UIStates.Gameplay);
        //_hudUI.gameObject.SetActive(uiState == UIStates.Gameplay);
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
        _gameplayUI.gameObject.SetActive(state == EStates.Gameplay);
        UIManager.Instance.Init(state == EStates.Gameplay);
        //_hudUI.gameObject.SetActive(state == EStates.Gameplay);
    }

    //Factory Pattern
    private StateBase CreateMaineMenuState()
    {
       return new MenuState(EStates.MainMenu, null);
    }

    private StateBase CreateTurnState()
    {
       return new TurnState(
          _gameSettings.Timer,
          _hudUI.UpdateTimer,
          OnTurnStateChanged,
          EStates.Gameplay,
          GotoMoveState
       );
    }

    private StateBase CreateMoveState()
    {
        return new MoveState(EStates.Move, GoToCombatState);
    }

    private StateBase CreateCombatState()
    {
       return new CombatState(_cameraController.SetCombatCameraPosition, EStates.Combat, GoToTurnState);
    }

    public Transform GetTileObjectAt(uint gridX, uint gridY)
    {
        return _grid.GetTileObjectAt(gridX, gridY);
    }

    public Vector2Int GetGridCoordsOfTileObject(Transform tile)
    {
        return _grid.GetGridCoordsOfTileObject(tile);
    }

    public Vector3 ConvertGridCoordsToVector3(uint gridX, uint gridY)
    {
        return _grid.ConvertGridCoordsToVector3(gridX, gridY);
    }

    public Vector2Int ConvertVector3CoordsToGrid(float worldX, float worldZ)
    {
        return _grid.ConvertVector3ToGridCoords(worldX, worldZ);
    }

    public Vector2Int GetAdjacentTileOnGrid(uint currentGridX, uint currentGridY, GridSystem.Directions direction)
    {
        return _grid.GetAdjacentTileOnGrid(currentGridX, currentGridY, direction);
    }

    public Vector3 GetAdjacentTileInWorld(Vector3 currentPosition, GridSystem.Directions direction)
    {
        return _grid.GetAdjacentTileInWorld(currentPosition, direction);
    }

    public bool IsOnGrid(Vector2Int gridPos)
    {
        return _grid.IsOnGrid(gridPos);
    }
    public bool IsOnGrid(Vector3 worldPos)
    {
        return _grid.IsOnGrid(worldPos);
    }

    public void InitGridContainer(GameObject gridContainer)
    {
        _grid.InitGameObjectConnection(gridContainer);
    }
 
    public void OnTurnStateChanged(bool inCanPlayersDraw)
    {
        onTurnStateChanged?.Invoke(inCanPlayersDraw);
    }    

    public void ShowWinScreen(int playerIndex)
    {
        UIManager.Instance.ShowWinScreen(playerIndex);
    }


    public float GetTileSize()
    {
        return _grid.TileSize;
    }
}
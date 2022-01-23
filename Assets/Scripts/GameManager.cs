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
   
	[SerializeField] CameraController _cameraController;
   
    private StateMachine _stateMachine;
    private GridSystem _grid;

    public bool isUsingMovePoints; 
    public bool isUsingMaxDistance;
    public bool winByNrOfKills;

    public bool canMoveOnlyOneNinjaPerTurn;

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
          OnTurnStateChanged,
          EStates.Gameplay
       );
    }

    private StateBase CreateCombatState()
    {
       return new CombatState(EStates.Combat);
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
}
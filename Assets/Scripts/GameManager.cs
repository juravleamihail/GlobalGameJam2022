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

   public override void Awake()
   {
      base.Awake();
      _stateMachine = new StateMachine();
      BackToMainMenu();

      _grid = new GridSystem(_gameSettings.GridSize, _gameSettings.TileSize);
   }

   private void Update()
   {
      _stateMachine.UpdateState();
   }

   private void ChangeState(StateBase state)
   {
      _stateMachine.ChangeState(state,  ToggleUI, _cameraController.ChangeState);
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
   public void GoToComat()
   {
      ChangeState(CreateCombatState());
   }

   public void ExitGame()
   {
      Application.Quit();
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

   private Vector3 ConvertGridCoordsToVector3(uint gridX, uint gridY)
   {
      return _grid.ConvertGridCoordsToVector3(gridX, gridY);
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
}
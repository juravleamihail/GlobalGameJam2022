using System;
using DefaultNamespace;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{

   [SerializeField] private MainMenu _mainMenuUI;
   [SerializeField] private Hud _hudUI;
   [SerializeField] private float _tileSize;
   [SerializeField] private uint _gridSize;

   private StateMachine _stateMachine;

   private GridSystem _grid;

   public override void Awake()
   {
      base.Awake();
      _stateMachine = new StateMachine();
      _grid = new GridSystem(_gridSize, _tileSize);
   }

   private void Update()
   {
      _stateMachine.UpdateState();
   }

   public void StartGame()
   {
      _stateMachine.ChangeState(new TurnState(10, _hudUI.UpdateTimer,null));
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

   private Vector3 ConvertGridCoordsToVector3(uint gridX, uint gridY)
   {
      return _grid.ConvertGridCoordsToVector3(gridX, gridY);
   }
}
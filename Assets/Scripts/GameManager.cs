using System;
using DefaultNamespace;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{

   [SerializeField] private MainMenu _mainMenuUI;
   [SerializeField] private Hud _hudUI;

   private StateMachine _stateMachine;

   public override void Awake()
   {
      base.Awake();
      _stateMachine = new StateMachine();
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
}



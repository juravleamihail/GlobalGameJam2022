using States;
using UI;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{

   [SerializeField] private MainMenu mainMenuUI;
   [SerializeField] private Hud hudUI;

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
      _stateMachine.ChangeState(new TurnState(10, hudUI.UpdateTimer,null));
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
      mainMenuUI.gameObject.SetActive(uiState == UIStates.MainMenu);
      hudUI.gameObject.SetActive(uiState == UIStates.Gameplay);
   }

   private enum UIStates{
      MainMenu,
      Gameplay
   }
}



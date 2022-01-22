using UnityEngine;
using UnityEngine.Events;

namespace UI
{
    public class MainMenu : MonoBehaviour {
        [SerializeField] private UnityEvent onStartButton;
        [SerializeField] private UnityEvent onExitButton;
    
        public void OnStartButton() {
            onStartButton.Invoke();
            GameManager.Instance.StartGame();
        }
    
        public void OnExitButton() {
            onExitButton.Invoke();
            GameManager.Instance.ExitGame();
        }
    }
}

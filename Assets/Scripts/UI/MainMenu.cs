using UnityEngine;
using UnityEngine.Events;

public class MainMenu : MonoBehaviour {
    [SerializeField] private UnityEvent _onStartButton;
    [SerializeField] private UnityEvent _OnExitButton;
    
    public void OnStartButton() {
        _onStartButton.Invoke();
        GameManager.Instance.StartGame();
    }
    
    public void OnExitButton() {
        _OnExitButton.Invoke();
        GameManager.Instance.ExitGame();
    }
}

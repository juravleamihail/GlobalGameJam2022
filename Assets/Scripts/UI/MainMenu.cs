using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class MainMenu : MonoBehaviour {

        public void OnStartButton() {
            SceneManager.LoadScene("MainScene");
        }

        public void OnExitButton() {
            Application.Quit();
        }
    }
}

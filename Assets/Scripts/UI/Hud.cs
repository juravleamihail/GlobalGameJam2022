using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI
{
    public class Hud : MonoBehaviour
    {
        [SerializeField] private Text timerTxt; 
    
        [Space(10),Header("Events")]
        [SerializeField] private UnityEvent onBackButton;
        public void OnBackButton()
        {
            onBackButton.Invoke();
            GameManager.Instance.BackToMainMenu();
        }

        public void UpdateTimer(float value)
        {
            timerTxt.text = value.ToString("#");
        }
    }
}

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Hud : MonoBehaviour
{
    [SerializeField] private Text _timerTxt; 
    
    [Space(10),Header("Events")]
    [SerializeField] private UnityEvent _onBackButton;
    public void OnBackButton()
    {
        _onBackButton.Invoke();
        GameManager.Instance.BackToMainMenu();
    }

    public void UpdateTimer(float value)
    {
        _timerTxt.text = value.ToString("#");
    }
}

using DefaultNamespace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private AvatarUI[] Player1Avatars;
    [SerializeField] private AvatarUI[] Player2Avatars;

    [SerializeField] private GameObject Player1KillsParent;
    [SerializeField] private GameObject Player2KillsParent;
    private List<Image> Player1KillsImages = new List<Image>();
    private List<Image> Player2KillsImages = new List<Image>();

    [SerializeField] private Sprite noKillIcon;
    [SerializeField] private Sprite killIcon;

    [SerializeField] private Text timerTxt;
    private float timer;
    [SerializeField] private GameSettingsSO gameSettings;

    [Space(10), Header("Events")]
    [SerializeField] private UnityEvent onBackButton;


    public void DieCharacter(int player, int character)
    {
        if(player == 0)
        {
            Player1Avatars[character].ChangeToDeathAvatar();
        }
        else
        {
            Player2Avatars[character].ChangeToDeathAvatar();
        }
    }

    public void RessurectCharacter(int player, int character)
    {
        if (player == 0)
        {
            Player1Avatars[character].ChangeToAliveAvatar();
        }
        else
        {
            Player2Avatars[character].ChangeToAliveAvatar();
        }
    }

    public void SelectCharacter(int player, int character)
    {
        if (player == 0)
        {
            Player1Avatars[character].SelectAvatar();
        }
        else
        {
            Player2Avatars[character].SelectAvatar();
        }
    }

    public void DeSelectCharacter(int player, int character)
    {
        if (player == 0)
        {
            Player1Avatars[character].DeselectAvatar();
        }
        else
        {
            Player2Avatars[character].DeselectAvatar();
        }
    }

    public void AddKillsIconsForPlayer1(uint killsToWin)
    {
        for(int i=0; i< killsToWin; i++)
        {
            GameObject newGO = new GameObject();
            newGO = Instantiate(newGO, Player1KillsParent.transform);
            var image = newGO.AddComponent<Image>();
            image.sprite = noKillIcon;
            Player1KillsImages.Add(image);
        }
    }

    public void AddKillsIconsForPlayer2(uint killsToWin)
    {
        for (int i = 0; i < killsToWin; i++)
        {
            GameObject newGO = new GameObject();
            newGO = Instantiate(newGO, Player2KillsParent.transform);
            var image = newGO.AddComponent<Image>();
            image.sprite = noKillIcon;
            Player1KillsImages.Add(image);
        }
    }

    public void AddKill(int player, int killNumber)
    {
        if (player == 0)
        {
            Player1KillsImages[killNumber-1].sprite = killIcon;
        }
        else
        {
            Player2KillsImages[killNumber-1].sprite = killIcon;
        }
    }

    private void Start()
    {
        //TODO: I will change it later this parameter
        AddKillsIconsForPlayer1(5);
        AddKillsIconsForPlayer2(5);
        timer = gameSettings.Timer;
    }

    public void OnBackButton()
    {
        onBackButton.Invoke();
        GameManager.Instance.BackToMainMenu();
    }

    public void UpdateTimer(float value)
    {
        timerTxt.text = value.ToString("#");
    }

    private void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            UpdateTimer(timer);
        }
        else
        {
            TimmerTrigger();
        }
    }

    private void TimmerTrigger()
    {
        //TODO: connect after the time is up
        // we call ResetTimer() after
    }

    public void ResetTimer()
    {
        timer = gameSettings.Timer;
    }
}

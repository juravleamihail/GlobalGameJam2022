using DefaultNamespace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
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

    [Space(10), Header("Events")]
    [SerializeField] private UnityEvent onBackButton;

    private bool isInit = false;

    [SerializeField] private GameObject player1WinsScreen;
    [SerializeField] private GameObject player2WinsScreen;

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

            for (int i = 0; i < Player1Avatars.Length; i++)
            {
                if(i != character)
                {
                    DeSelectCharacter(player, i);
                }
            }
        }
        else
        {
            Player2Avatars[character].SelectAvatar();

            for (int i = 0; i < Player2Avatars.Length; i++)
            {
                if (i != character)
                {
                    DeSelectCharacter(player, i);
                }
            }
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

    public void Init(bool enable)
    {
        if(!isInit && enable)
        {
            //TODO: I will change it later this parameter
            AddKillsIconsForPlayer1(5);
            AddKillsIconsForPlayer2(5);
            
            isInit = true;
        }
    }

    public void OnBackButton()
    {
        onBackButton.Invoke();
        GameManager.Instance.BackToMainMenu();
    }
    
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ShowWinScreen(int playerWhoWon)
    {
        if(playerWhoWon == 0)
        {
            player1WinsScreen.SetActive(true);
        }
        else
        {
            player2WinsScreen.SetActive(true);
        }
    }
}

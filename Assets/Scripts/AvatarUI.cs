using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarUI : MonoBehaviour
{
    [SerializeField] private GameObject AliveAvatar;
    [SerializeField] private GameObject DeadAvatar;
    [SerializeField] private GameObject HighlightAvatar;

    public void ChangeToDeathAvatar()
    {
        AliveAvatar.SetActive(false);
        DeadAvatar.SetActive(true);
    }

    public void ChangeToAliveAvatar()
    {
        AliveAvatar.SetActive(true);
        DeadAvatar.SetActive(false);
    }

    public void SelectAvatar()
    {
        HighlightAvatar.SetActive(true);
    }

    public void DeselectAvatar()
    {
        HighlightAvatar.SetActive(false);
    }
}

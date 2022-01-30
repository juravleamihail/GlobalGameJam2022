using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Material defaultMaterial { get; private set; }
    private float _disappearTimer;
    private bool isTimerOn;
    public float disappearTime;

    public InvasionData invasionData { get; private set; }
  
    public struct InvasionData
    {
        public InvasionData(bool inIsBeingInvaded = false, int inInvadedPlayerID = -1, int inInvadedNinjaIndex = -1)
        {
            isBeingInvaded = inIsBeingInvaded;
            invadedPlayerID = inInvadedPlayerID;
            invadedNinjaIndex = inInvadedNinjaIndex;
        }

        public bool isBeingInvaded { get; private set; }
        public int invadedPlayerID { get; private set; }
        public int invadedNinjaIndex { get; private set; }
    }

    private void Awake()
    {
        Renderer renderer = gameObject.GetComponent<Renderer>();
        defaultMaterial = renderer.material;
    }

    public void StartDisappearTimer()
    {
        isTimerOn = true;
    }

    private void Update()
    {
        if (isTimerOn)
        {
            if (_disappearTimer >= disappearTime)
            {
                Renderer renderer = gameObject.GetComponent<Renderer>();
                renderer.material = defaultMaterial;
                isTimerOn = false;
                _disappearTimer = 0;
                return;
            }
            _disappearTimer += Time.deltaTime;
        }
    }

    public void SetInvasion(int invadedPlayerID, int invadedNinjaIndex)
    {
        invasionData = new InvasionData(true, invadedPlayerID, invadedNinjaIndex);
    }

    public void ClearInvasion()
    {
        invasionData = new InvasionData();
    }

    public bool IsBeingInvaded(out int invadedPlayerID, out int invadedNinjaIndex)
    {
        invadedPlayerID = invasionData.invadedPlayerID;
        invadedNinjaIndex = invasionData.invadedNinjaIndex;
        return invasionData.isBeingInvaded;
    }
}

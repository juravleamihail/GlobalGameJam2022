using System;
using System.Collections.Generic;
using Cinemachine;
using States;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private List<CameraSet> _camers;
    [SerializeField] private CinemachineBrain _brain;

    private EStates _currentState;

    public float ChangeState(EStates state)
    {
        _camers.ForEach(camSet =>
        {
            camSet.VCam.Priority = camSet.State == state ? 1 : 0;
        });

        float time = 0;
        
        foreach (var customBlend in _brain.m_CustomBlends.m_CustomBlends)
        {
            if (customBlend.m_From == _currentState.ToString() && customBlend.m_To == state.ToString())
            {
                time = customBlend.m_Blend.m_Time;
            }
        }
        
        _currentState = state;
        return time;
    }

    public void SetCombatCameraPosition(Vector3 pos)
    {
        var combatCamera = _camers.Find(c => c.State == EStates.Combat);
        combatCamera.VCam.transform.position = pos;
    }
    
    [Serializable]
    private struct CameraSet
    {
        public EStates State;
        public CinemachineVirtualCamera VCam;
    }
}

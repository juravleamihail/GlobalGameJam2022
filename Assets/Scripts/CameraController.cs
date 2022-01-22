using System;
using System.Collections.Generic;
using Cinemachine;
using States;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private List<CameraSet> _camers;
    [SerializeField] private CinemachineBrain _brain;

    private CinemachineBlenderSettings.CustomBlend[] _blends;
    private EStates _currentState;
    private void Awake()
    {
        _blends = _brain.m_CustomBlends.m_CustomBlends;
    }

    public float ChangeState(EStates state)
    {
        _camers.ForEach(camSet =>
        {
            camSet.VCam.Priority = camSet.State == state ? 1 : 0;
        });

        float time = 0;
        
        foreach (var customBlend in _blends)
        {
            if (customBlend.m_From == _currentState.ToString() && customBlend.m_To == state.ToString())
            {
                time = customBlend.m_Blend.m_Time;
            }
        }
        
        _currentState = state;
        return time;
    }
    
    [Serializable]
    private struct CameraSet
    {
        public EStates State;
        public CinemachineVirtualCamera VCam;
    }
}

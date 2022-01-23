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
    private const string kAnyStateName = "**ANY CAMERA**";
    public float ChangeState(EStates state)
    {
        _camers.ForEach(camSet =>
        {
            if (state == EStates.Combat && camSet.State == EStates.Combat)
            {
                if (camSet.VCam.Priority == 0)
                {
                    camSet.VCam.Priority = 1;
                    camSet.BackupCamera.Priority = 0;
                }
                else
                {
                    camSet.VCam.Priority = 0;
                    camSet.BackupCamera.Priority = 1;
                }
                
                return;
            }
            
            camSet.VCam.Priority = camSet.State == state ? 1 : 0;
            if (camSet.BackupCamera != null)
            {
                camSet.BackupCamera.Priority = 0;
            }
        });

        float time = 0;
        
        foreach (var customBlend in _brain.m_CustomBlends.m_CustomBlends)
        {
            if ((customBlend.m_From == _currentState.ToString() || customBlend.m_From == kAnyStateName)
                && customBlend.m_To == state.ToString())
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

        if (combatCamera.VCam.Priority == 1)
        {
            combatCamera.BackupCamera.transform.position = pos;
        }
        else
        {
            combatCamera.VCam.transform.position = pos;
        }
    }
    
    [Serializable]
    private struct CameraSet
    {
        public EStates State;
        public CinemachineVirtualCamera VCam;
        public CinemachineVirtualCamera BackupCamera;
    }
}

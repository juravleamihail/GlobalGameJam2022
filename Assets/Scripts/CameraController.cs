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
        if (!IsCameraStateValid(state))
        {
            return 0;
        }

        if (state == EStates.Combat)
        {
            _camers.ForEach(camSet =>
            {
                if (camSet.State != EStates.Combat)
                {
                    camSet.VCam.Priority = 0;
                }
            });

            var cameraSet = _camers.Find(c => c.State == EStates.Combat);
            if (cameraSet.VCam.Priority == 0)
            {
                cameraSet.VCam.Priority = 1;
                cameraSet.BackupCamera.Priority = 0;
            }
            else
            {
                cameraSet.VCam.Priority = 0;
                cameraSet.BackupCamera.Priority = 1;
            }
        }
        else
        {
            _camers.ForEach(camSet =>
            {
                camSet.VCam.Priority = camSet.State == state ? 1 : 0;
                if (camSet.BackupCamera != null)
                { 
                    camSet.BackupCamera.Priority = 0;
                }
            });
        }
        
        float time = 0;
        
        foreach (var customBlend in _brain.m_CustomBlends.m_CustomBlends)
        {
            if ((customBlend.m_From == _currentState.ToString() || customBlend.m_From == kAnyStateName)
                && customBlend.m_To == state.ToString())
            {
                time = customBlend.m_Blend.m_Time;
                break;
            }
        }
        
        _currentState = state;
        return time;
    }

    private bool IsCameraStateValid(EStates state)
    {
        return state != EStates.Move;
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

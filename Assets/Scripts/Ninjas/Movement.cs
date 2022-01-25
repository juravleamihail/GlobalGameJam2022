using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    //this script is meant to be placed on each ninja

    protected Vector3 _nextTileWorldPosition; //TODO find a way to avoid this caching
    protected bool _isMovingOneTile = false;
    protected bool _isMovePhaseForNinja = false;
    private Action _onCompleteCb;
    
    [SerializeField]protected float _distTolerance = 0.05f;
    [SerializeField]float _moveSpeed;
    [SerializeField] private Animator _animatorController;

    private void Awake()
    {
        GetComponent<Path>().onClearPath = OnClearPath;
        GetComponent<Path>().onInitPath = OnInitPath;
    }
    private void OnClearPath()
    {
        //TODO restore all tiles in path to their original colors
    }

    private void OnInitPath()
    {
        //a lot of potential instability due to these caches, find ways to avoid the need for them
        _nextTileWorldPosition = transform.position;
        _isMovingOneTile = false;
        _isMovePhaseForNinja = false;
    }

    public void StartMovePhase(Action onCompleteCb)
    {
        _onCompleteCb = onCompleteCb;
        //call this at the start of the move phase;
        _isMovePhaseForNinja = true;
        
        if (!GetComponent<Path>().IsOnlyCurrentTile())
        {
            _animatorController.SetBool("isMoving", _isMovePhaseForNinja);
        }
    }

    protected void SetupMovementToNextTile(out bool shouldMoveOneTile)
    {
        Path pathComponent = gameObject.GetComponent<Path>();
         
        if (pathComponent.IsOnlyCurrentTile())
        {
            shouldMoveOneTile = false;
            _onCompleteCb?.Invoke();
            return;
        }
        shouldMoveOneTile = true;

        Vector2Int nextTileOnGrid = pathComponent.GetNextTile();
        _nextTileWorldPosition = GameManager.Instance.ConvertGridCoordsToVector3((uint)nextTileOnGrid.x, (uint)nextTileOnGrid.y);
        
        //Rotate to object
        Vector3 pos = _nextTileWorldPosition;
        pos.y = transform.position.y;
        transform.LookAt(pos);
    }

    private void Update()
    {
        if (!GetComponent<Ninja>().IsNinjaAlive)
        {
            return;
        }    

        
        if (!_isMovePhaseForNinja)
        {
            return;
        }

        if (_isMovingOneTile)
        {
            Ninja ninja = GetComponent<Ninja>();
            ninja.SyncWithTile();
        }

        Vector3 nextTileVector = _nextTileWorldPosition - transform.position;
        float distance = Vector3.Magnitude(nextTileVector);
        if (distance < _distTolerance)
        {
            if (_isMovingOneTile)
            {
                _isMovingOneTile = false;
                Path pathComponent = gameObject.GetComponent<Path>();
                pathComponent.ApplyDefaultMaterial(0);
                pathComponent.OnMovedOneTile();
            }
            else
            {
                SetupMovementToNextTile(out _isMovingOneTile);
                _isMovePhaseForNinja = _isMovingOneTile;
                _animatorController.SetBool("isMoving", _isMovePhaseForNinja);
            }
        }
        else
        {
           // Vector3 direction = Vector3.Normalize(nextTileVector);
           //   transform.Translate(direction * _moveSpeed * Time.deltaTime);
           transform.position = Vector3.MoveTowards(transform.position, _nextTileWorldPosition, _moveSpeed * Time.deltaTime);
        }
    }

    //private bool IsBetweenTiles(Vector3 destination)
    //{
    //    float distance = Vector3.Magnitude(destination - transform.position);
    //    if (Mathf.Approximately(distance, GameManager.Instance.GetTileSize()))
    //    {
    //        return true;
    //    }
    //    return false;
    //}
}

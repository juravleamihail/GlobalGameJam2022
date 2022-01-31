using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    //this script is meant to be placed on each ninja

    protected Vector3 _nextTileWorldPosition; //TODO find a way to avoid this caching
    protected bool _isMovingOneTile = false;
    public bool isMovingOneTile { get { return _isMovingOneTile; } }
    protected bool _isMovePhaseForNinja = false;
    private Action _onCompleteCb;
    private Func<Vector2Int, bool> _onTileCb;

    private Path _pathComponent;
    private Ninja _ninja;
    
    [SerializeField]protected float _distTolerance = 0.05f;
    [SerializeField]float _moveSpeed;
    [SerializeField] private Animator _animatorController;

    private void Awake()
    {
        _ninja = GetComponent<Ninja>();
        _pathComponent = GetComponent<Path>();
        _pathComponent.onClearPath = OnClearPath;
        _pathComponent.onInitPath = OnInitPath;
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

    public void StartMovePhase(Action onCompleteCb, Func<Vector2Int,bool> onTileChangedCb)
    {
        SetMovementPhase(true, onCompleteCb, onTileChangedCb);
    }
    
    public void ForceStopMovementPhase()
    {
        SetMovementPhase(false, null, null);
    }

    private void SetMovementPhase(bool state, Action onCompleteCb, Func<Vector2Int,bool> onTileChangedCb)
    {
        _onCompleteCb = onCompleteCb;
        _onTileCb = onTileChangedCb;
        //call this at the start of the move phase;
        _isMovePhaseForNinja = state;
        
        if (!GetComponent<Path>().IsOnlyCurrentTile())
        {
            _animatorController.SetBool("isMoving", state);
        }
    }
 
    protected void SetupMovementToNextTile(out bool shouldMoveOneTile)
    {
        if (_pathComponent.IsOnlyCurrentTile())
        {
            shouldMoveOneTile = false;
            _onCompleteCb?.Invoke();
            return;
        }

        var result = _onTileCb?.Invoke(_pathComponent.GetCurrentTile());
        if (result == true)
        {
            shouldMoveOneTile = false;
            return;
        }

        shouldMoveOneTile = true;
        
        Vector2Int nextTileOnGrid = _pathComponent.GetNextTile();
        _nextTileWorldPosition = GameManager.Instance.ConvertGridCoordsToVector3((uint)nextTileOnGrid.x, (uint)nextTileOnGrid.y);

        //Rotate to object
        Vector3 pos = _nextTileWorldPosition;
        pos.y = transform.position.y;
        transform.LookAt(pos);
    }

    private void Update()
    {
        if (!_ninja.IsNinjaAlive)
        {
            return;
        }    

        
        if (!_isMovePhaseForNinja)
        {
            return;
        }

        if (_isMovingOneTile)
        {
            _ninja.SyncWithTile();
        }

        Vector3 nextTileVector = _nextTileWorldPosition - transform.position;
        float distance = Vector3.Magnitude(nextTileVector);
        if (distance < _distTolerance)
        {
            _pathComponent.ApplyDefaultMaterialToTile(0);
            if (_isMovingOneTile)
            {
                _pathComponent.RemoveCurrentTile();
                _isMovingOneTile = false;
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

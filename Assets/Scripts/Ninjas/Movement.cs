using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    //this script is meant to be placed on each ninja

    protected Vector3 _nextTileWorldPosition;
    public bool _isMovingOneTile = false;
    protected bool _isMovePhaseForNinja = false;
    private Action _onCompleteCb;
    
    [SerializeField]protected float _distTolerance = 0.05f;
    [SerializeField]float _moveSpeed;
    [SerializeField] private Animator _animatorController;

    private void Start()
    {
        _nextTileWorldPosition = transform.position;
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
            return;
        }
        shouldMoveOneTile = true;

        Vector2Int nextTileOnGrid = pathComponent.GetNextTile();
        _nextTileWorldPosition = GameManager.Instance.ConvertGridCoordsToVector3((uint)nextTileOnGrid.x, (uint)nextTileOnGrid.y);
    }

    private void Update()
    {
        if (!_isMovePhaseForNinja)
        {
            return;
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
                _onCompleteCb?.Invoke();
            }
        }
        else
        {
            Vector3 direction = Vector3.Normalize(nextTileVector);
            transform.Translate(direction * _moveSpeed * Time.deltaTime);

            if (IsBetweenTiles(_nextTileWorldPosition))
            {
                Vector2Int nextTileGridPosition = GameManager.Instance.ConvertVector3CoordsToGrid(_nextTileWorldPosition.x, _nextTileWorldPosition.z);
                Transform nextTile = GameManager.Instance.GetTileObjectAt((uint)nextTileGridPosition.x, (uint)nextTileGridPosition.y);

                Ninja ninja = gameObject.GetComponent<Ninja>();

                ninja.SyncWithTile(nextTile);
            }
        }   
    }

    private bool IsBetweenTiles(Vector3 destination)
    {
        float distance = Vector3.Magnitude(destination - transform.position);
        if (Mathf.Approximately(distance, GameManager.Instance.GetTileSize()))
        {
            return true;
        }
        return false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    //this script is meant to be placed on each ninja

    protected Vector3 _nextTileWorldPosition;
    protected bool _isMovingOneTile = false;
    protected bool _isMovePhaseForNinja = false;
    [SerializeField]protected float _distTolerance = 0.05f;
    [SerializeField]float _moveSpeed;

    private void Start()
    {
        _nextTileWorldPosition = transform.position;
    }

    public void StartMovePhase()
    {
        //call this at the start of the move phase;
        _isMovePhaseForNinja = true;
    }

    protected void MoveOneTile(out bool shouldMoveOneTile)
    {
        Path pathComponent = gameObject.GetComponent<Path>();
         
        if (pathComponent.IsAtDestination())
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
        //debug
        if (Input.GetKeyDown(KeyCode.M))
        {
            StartMovePhase();
        }
        //debug ends here

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
                pathComponent.OnMovedOneTile();
            }
            else
            {
                MoveOneTile(out _isMovingOneTile);
                _isMovePhaseForNinja = _isMovingOneTile;
            }
        }
        else
        {
            Vector3 direction = Vector3.Normalize(nextTileVector);
            transform.Translate(direction * _moveSpeed * Time.deltaTime);
        }   
    }
}

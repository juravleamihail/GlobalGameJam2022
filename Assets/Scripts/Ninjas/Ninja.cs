using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Ninja : MonoBehaviour
{
    [SerializeField] private UnityAction _onPlayerDeath;
        
    public NinjaTypeSO NinjaType { get; set; }

    public Action<GridSystem.Directions> onDrawPathInput { private get; set; }
    public Action<bool> onUndoInput { private get; set; }
    public bool hasPath;

    [SerializeField] private Animator _animatorController;
    [SerializeField] private GameObject _mesh;
    
    public void Init(NinjaTypeSO ninjaTypeSO, Action onDeathCb)
    {
        NinjaType = ninjaTypeSO;
        _onPlayerDeath += _onPlayerDeath;
    }

    public int GetPlayerIndex()
    {
        return NinjaType.PlayerIndex;
    }

    public Vector2Int GetPositionOnGrid()
    {
        //TODO is this method ever used? if not, delete it
        Vector3 position = transform.position;
        Vector2Int result = GameManager.Instance.ConvertVector3CoordsToGrid(position.x, position.z);
        if (!GameManager.Instance.IsOnGrid(result))
        {
            Debug.Log("Ninja " + gameObject.name + " at grid position " + result + " is outside the grid.");
            return GameManager.Instance.vector2IntException;
        }
        return result;
    }

    public void TryDrawPath(GridSystem.Directions direction)
    {
        //TODO should we run any of the "can draw?" checks here instead of elsewhere?
        onDrawPathInput?.Invoke(direction);
    }

    public void UndoDrawPath(bool longUndo)
    {
        onUndoInput?.Invoke(longUndo);
    }

    public void StartMovePhase()
    {
        Movement movement = gameObject.GetComponent<Movement>();
        movement.StartMovePhase();
    }

    public void KillEnemy(Ninja otherNinja)
    {
        // Do some more stuff here (score)
        _animatorController.SetBool("isAttacking", true);
        otherNinja.Dead();
        StartCoroutine(WaitToStopAttack());
    }

    private void Dead()
    {
        _onPlayerDeath?.Invoke();
        _animatorController.SetBool("isDead", true);
        //Do some more stuff here (animations)
        StartCoroutine(WaitToDestroyGameObject());
    }

    private IEnumerator WaitToDestroyGameObject()
    {
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
    }

    private IEnumerator WaitToStopAttack()
    {
        yield return new WaitForSeconds(2);
        _animatorController.SetBool("isAttacking", false);
    }
}

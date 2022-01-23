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
    public int ninjaIndex;
    public bool IsNinjaAlive { get; private set; }
    
    [SerializeField] private Animator _animatorController;
    [SerializeField] private GameObject _mesh;
    [SerializeField] private GameObject _katana;
    
    public void Init(NinjaTypeSO ninjaTypeSO, int index)
    {
        NinjaType = ninjaTypeSO;
        ninjaIndex = index;
        SetNinjaAliveStatus(true);
    }

    private void Start()
    {
        //HidePlayer();
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
        if (!IsNinjaAlive)
        {
            return;
        }
        onDrawPathInput?.Invoke(direction);
    }

    public void UndoDrawPath(bool longUndo)
    {        
        if (!IsNinjaAlive)
        {
            return;
        }
        onUndoInput?.Invoke(longUndo);
    }

    public void StartMovePhase(Action onCompleteCb)
    {
        Movement movement = gameObject.GetComponent<Movement>();
        movement.StartMovePhase(onCompleteCb);
    }

    public void KillEnemy(Ninja otherNinja)
    {
        // Do some more stuff here (score)
        _animatorController.SetBool("isAttacking", true);
        otherNinja.Dead();

        int playerIndex = NinjaType.PlayerIndex;
        Player player = PlayerManager.Instance.GetPlayerByIndex(playerIndex);
        player.IncrementKills();

        RevealPlayer();
            
        StartCoroutine(WaitToStopAttack());
    }

    private void Dead()
    {
        _onPlayerDeath?.Invoke();
        _animatorController.SetBool("isDead", true);
        //Do some more stuff here (animations)
        UIManager.Instance.DieCharacter(NinjaType.PlayerIndex, ninjaIndex);
        RevealPlayer();
        StartCoroutine(WaitToDestroyGameObject());
    }

    private IEnumerator WaitToDestroyGameObject()
    {
        NinjaManager.Instance.TryRemoveNinja(GetPlayerIndex(),this);
        yield return new WaitForSeconds(2);
        SetNinjaAliveStatus(false);
    }

    private void SetNinjaAliveStatus(bool value)
    {
        IsNinjaAlive = value;
        ToogleMeshes(IsNinjaAlive);
    }

    private IEnumerator WaitToStopAttack()
    {
        yield return new WaitForSeconds(2);
        _animatorController.SetBool("isAttacking", false);
        HidePlayer();
    }

    private void Update()
    {
        if(!GetComponent<Movement>()._isMovingOneTile)
        {
            return;
        }

        Vector2Int ninjaCoordsOnGrid = GameManager.Instance.ConvertVector3CoordsToGrid(transform.position.x, transform.position.z);
        Transform tileObject = GameManager.Instance.GetTileObjectAt((uint)ninjaCoordsOnGrid.x, (uint)ninjaCoordsOnGrid.y);

        PlayerHolder tileScript = tileObject.gameObject.GetComponent<PlayerHolder>();

        if(tileScript == null || tileScript.PlayerType == null)
        {
            return;
        }

        if(NinjaType.PlayerIndex == tileScript.PlayerType.PlayerIndex)
        {
            HidePlayer();
        }
        else
        {
            RevealPlayer();
        }
    }

    public void HidePlayer()
    {
        ToogleMeshes(false);
    }

    public void RevealPlayer()
    {
        ToogleMeshes(true);
    }

    private void ToogleMeshes(bool value)
    {
        _mesh.SetActive(value);
        _katana.SetActive(value);
    }
}

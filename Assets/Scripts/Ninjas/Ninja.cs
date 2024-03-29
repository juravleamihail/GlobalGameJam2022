using System;
using System.Collections;
using System.Collections.Generic;
using States;
using UnityEngine;
using UnityEngine.Events;

public class Ninja : MonoBehaviour
{
    //[SerializeField] private UnityAction _onNinjaDeath;
    [SerializeField] private UnityAction<Ninja> _onNinjaDeath;

    public NinjaTypeSO NinjaType { get; set; }

    public Action<GridSystem.Directions> onDrawPathInput { private get; set; }
    public Action<bool> onUndoInput { private get; set; }

    public int ninjaIndex;
    
    public NinjaStates NinjaStatus { get; private set; }

    public bool IsNinjaAlive => NinjaStatus != NinjaStates.Dead;

    public AudioSource _soundSource;

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

    public Vector3 GetMeshForwardVector()
    {
        return _mesh.transform.forward;
    }

    public int GetPlayerIndex()
    {
        return NinjaType.PlayerIndex;
    }

    public Vector2Int GetGridPositionViaTransform()
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
        if (NinjaStatus == NinjaStates.Dead)
        {
            return;
        }
        onDrawPathInput?.Invoke(direction);
    }

    public void UndoDrawPath(bool longUndo)
    {
        if (NinjaStatus == NinjaStates.Dead)
        {
            return;
        }
        onUndoInput?.Invoke(longUndo);
    }

    public void ForceStopMovementPhase()
    {
        Movement movement = gameObject.GetComponent<Movement>();
        movement.ForceStopMovementPhase();
    }

    private Func<NinjaMovementData, bool> _onTileChangedCb;
    public void StartMovePhase(Action onCompleteCb, Func<NinjaMovementData,bool> onTileChangedCb)
    {
        _onTileChangedCb = onTileChangedCb;
        Movement movement = gameObject.GetComponent<Movement>();
        movement.StartMovePhase(onCompleteCb, OnTileChangedCB);
    }

    private bool OnTileChangedCB(Vector2Int tilePos)
    {
        if (tilePos == GameManager.Instance.vector2IntException)
        {
            return false;
        }
        
        var result = new NinjaMovementData();
        result.PlayerId = GetPlayerIndex();
        result.NinjaId = ninjaIndex;
        result.TilePos = tilePos;
            
        return _onTileChangedCb.Invoke(result);
    }
    
    public void KillEnemy(Ninja otherNinja)
    {
        // Do some more stuff here (score)
        _animatorController.SetBool("isAttacking", true);
        otherNinja.Dead();

        int playerIndex = NinjaType.PlayerIndex;
        Player player = PlayerManager.Instance.GetPlayerByIndex(playerIndex);
        player.IncrementKills();
        SoundManager.Instance.PlayRandomSwordKillSound(_soundSource);

        StartCoroutine(WaitToStopAttack());
    }

    private void Dead()
    {
        _animatorController.SetBool("isDead", true);
        //Do some more stuff here (animations)
        UIManager.Instance.DieCharacter(NinjaType.PlayerIndex, ninjaIndex);
        Reveal(false);

        SetNinjaAliveStatus(false);
        GetComponent<Path>().ClearPath();

        StartCoroutine(WaitToFinishNinjaDeathLogic());
    }

    private IEnumerator WaitToFinishNinjaDeathLogic()
    {
        yield return new WaitForSeconds(2);
        FinishNinjaDeathLogic();
    }

    private void FinishNinjaDeathLogic()
    {
        ShowNinjaAliveStatus();
        _onNinjaDeath?.Invoke(this);
    }

    public void RespawnAt(Vector2Int respawnPoint)
    {
        Vector3 respawnWorldPosition = GameManager.Instance.ConvertGridCoordsToVector3((uint)respawnPoint.x, (uint)respawnPoint.y);
        transform.position = respawnWorldPosition;

        transform.LookAt(new Vector3(GameManager.Instance.GetGridSize / 2, 0, GameManager.Instance.GetGridSize / 2));

        _animatorController.SetBool("isDead", false);
        UIManager.Instance.RessurectCharacter(NinjaType.PlayerIndex, ninjaIndex);

        GetComponent<Path>().Init();
        
        OnTileChangedCB(respawnPoint);
        SetNinjaAliveStatus(true);
        ShowNinjaAliveStatus();
    }
    
    private void SetNinjaAliveStatus(bool value)
    {
        NinjaStatus = value ? NinjaStates.Alive : NinjaStates.Dead;
    }

    private void ShowNinjaAliveStatus()
    {
        ToogleMeshes(IsNinjaAlive);
    }

    private IEnumerator WaitToStopAttack()
    {
        yield return new WaitForSeconds(2);
        _animatorController.SetBool("isAttacking", false);
        NinjaStatus = NinjaStatus == NinjaStates.InCombat ? NinjaStates.Alive : NinjaStates.Dead;
    }

    public void SyncWithTile()//Transform tileObject)
    {
        if (NinjaStatus == NinjaStates.InCombat)
        {
            return;
        }

        Vector2Int ninjaCoordsOnGrid = GameManager.Instance.ConvertVector3CoordsToGrid(transform.position.x, transform.position.z);
        //Vector2Int ninjaCoordsOnGrid = GetGridPositionViaPath();
        Transform tileObject = GameManager.Instance.GetTileObjectAt((uint)ninjaCoordsOnGrid.x, (uint)ninjaCoordsOnGrid.y);
        //TODO implement something like the IsBetweenTiles commented method in Movement.cs


        TileToPlayerConnection tileConnection = tileObject.gameObject.GetComponent<TileToPlayerConnection>();

        if (tileConnection == null || tileConnection.PlayerType == null)
        {
            return;
        }

        if (NinjaType.PlayerIndex == tileConnection.PlayerType.PlayerIndex)
        {
            Hide();
        }
        else
        {
            Reveal(false);
        }
    }

    public void Hide()
    {
        ToogleMeshes(false);
    }

    public void Reveal(bool forceInCombat)
    {
        if (forceInCombat)
        {
            NinjaStatus = NinjaStates.InCombat;
        }
        ToogleMeshes(true);
    }

    private void ToogleMeshes(bool value)
    {
        _mesh.SetActive(value);
        _katana.SetActive(value);
    }  

    public void AddToOnNinjaDeath(UnityAction<Ninja> onNinjaDeath)
    {
        _onNinjaDeath += onNinjaDeath;
    }

    public bool IsDrawing()
    {
        if (!IsNinjaAlive)
        {
            return false;
        }
        Path path = GetComponent<Path>();
        return !(path.IsOnlyCurrentTile());
    }

    public Vector2Int GetGridPositionViaPath()
    {
        //this is called GetPositionViaPath() and not GetNinjaLocation because
        //during the move phase, while moving between tiles,
        //the logical first element of the path may not always reflect where the ninja appears to be

        Path path = GetComponent<Path>();
        if (path)
        {
            return path.GetOrigin();
        }    
        else
        {
            return GameManager.Instance.vector2IntException;
        }
    }

    public bool HasPath()
    {
        return (IsNinjaAlive && !GetComponent<Path>().IsOnlyCurrentTile());
    }

    public Vector2Int GetDestination()
    {
        return GetComponent<Path>().GetDestination();
    }

    public enum NinjaStates
    {
        Alive,
        Dead,
        Moving,
        InCombat,
        Defending
    }
}

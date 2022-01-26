using System;
using System.Collections;
using System.Collections.Generic;
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
    public bool IsNinjaAlive { get; private set; }

    public AudioSource _soundSource;

    [SerializeField] private Animator _animatorController;
    [SerializeField] private GameObject _mesh;
    [SerializeField] private GameObject _katana;

    public void Init(NinjaTypeSO ninjaTypeSO, int index)
    {
        NinjaType = ninjaTypeSO;
        ninjaIndex = index;
        ChangeNinjaAliveStatus(true);
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
        SoundManager.Instance.PlayRandomSwordKillSound(_soundSource);

        Reveal();

        StartCoroutine(WaitToStopAttack());
    }

    private void Dead()
    {
        _animatorController.SetBool("isDead", true);
        //Do some more stuff here (animations)
        UIManager.Instance.DieCharacter(NinjaType.PlayerIndex, ninjaIndex);
        Reveal();
        StartCoroutine(WaitToSetupNinjaDead());
    }

    private IEnumerator WaitToSetupNinjaDead()
    {
        yield return new WaitForSeconds(2);
        SetupNinjaDead();
    }

    private void SetupNinjaDead()
    {
        ChangeNinjaAliveStatus(false);
        GetComponent<Path>().ClearPath();
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
        
        ChangeNinjaAliveStatus(true);
    }
    
    private void ChangeNinjaAliveStatus(bool value)
    {
        IsNinjaAlive = value;
        ToogleMeshes(IsNinjaAlive);
    }

    private IEnumerator WaitToStopAttack()
    {
        yield return new WaitForSeconds(2);
        _animatorController.SetBool("isAttacking", false);
        Hide();
    }

    public void SyncWithTile()//Transform tileObject)
    {
        Vector2Int ninjaCoordsOnGrid = GameManager.Instance.ConvertVector3CoordsToGrid(transform.position.x, transform.position.z);
        Transform tileObject = GameManager.Instance.GetTileObjectAt((uint)ninjaCoordsOnGrid.x, (uint)ninjaCoordsOnGrid.y);

        //TODO this seems to cause bugs sometimes; get the tiles using the path instead of the position (see IsBetweenTiles commented method in Movement.cs)

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
            Reveal();
        }
    }

    public void Hide()
    {
        ToogleMeshes(false);
    }

    public void Reveal()
    {
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
        Path path = GetComponent<Path>();
        return path.IsOnlyCurrentTile();
    }

    public Vector2Int GetGridPositionViaPath()
    {
        //this is called GetPositionViaPath() and not GetNinjaLocation because
        //during the move phase, while moving between tiles,
        //the logical first element of the path may not always reflect where the ninja appears to be

        Path path = GetComponent<Path>();
        return path.GetOrigin();
    }
}

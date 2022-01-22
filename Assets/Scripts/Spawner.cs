using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    private SamuraiSO _samuraiSO;

    public Samurai SpawnChracter()
    {
        var go = Instantiate(_samuraiSO.Prefab).GetComponent<Samurai>();
        go.transform.position = transform.position;

        return go;
    }

    private void Start()
    {
       SamuraiManager.Instance.AddSamurai(SpawnChracter());
    }
}

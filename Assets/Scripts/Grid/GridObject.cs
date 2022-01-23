using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject : MonoBehaviour
{
    private void Awake()
    {
        GameManager.Instance.InitGridContainer(gameObject);
    }
}

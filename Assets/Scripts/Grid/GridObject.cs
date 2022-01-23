using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject : MonoBehaviour
{
    private void Start()
    {
        GameManager.Instance.InitGridContainer(gameObject);
    }
}

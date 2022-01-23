using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Material defaultMaterial { get; private set; }
    private void Awake()
    {
        Renderer renderer = gameObject.GetComponent<Renderer>();
        defaultMaterial = renderer.material;
    }
}

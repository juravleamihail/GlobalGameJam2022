using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Material defaultMaterial { get; private set; }
    private float _disappearTimer;
    private bool isTimerOn;
    public float disappearTime;
    

    private void Awake()
    {
        Renderer renderer = gameObject.GetComponent<Renderer>();
        defaultMaterial = renderer.material;
    }

    public void StartDisappearTimer()
    {
        isTimerOn = true;
    }

    private void Update()
    {
        if (isTimerOn)
        {
            if (_disappearTimer >= disappearTime)
            {
                Renderer renderer = gameObject.GetComponent<Renderer>();
                renderer.material = defaultMaterial;
                isTimerOn = false;
                _disappearTimer = 0;
                return;
            }
            _disappearTimer += Time.deltaTime;
        }
    }
}

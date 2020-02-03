using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeWarpShaderHelper : MonoBehaviour
{

    private Material mat;
    private Renderer _renderer;

    void Awake()
    {
        _renderer = GetComponent<Renderer>();
        mat = _renderer.material;
    }
    
    void Update()
    {
        mat.SetVector("_position", transform.position);
        
    }
}

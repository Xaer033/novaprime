using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFixture : MonoBehaviour
{
    public Rigidbody2D[] rigidBodyList;

    public LineRenderer lineRenderer;
    
    // Start is called before the first frame update
    private Vector3[] _linePositions;
    void Start()
    {
        _linePositions = new Vector3[rigidBodyList.Length];
        lineRenderer.useWorldSpace = true;
        updateLinePositions();
    }

    // Update is called once per frame
    void Update()
    {
        updateLinePositions();
    }

    private void updateLinePositions()
    {
        for (int i = 0; i < _linePositions.Length; ++i)
        {
            _linePositions[i] = rigidBodyList[i].position;
        }
        
        lineRenderer.SetPositions(_linePositions);
    }
}

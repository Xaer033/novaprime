using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletView : MonoBehaviour, ITimeWarpTarget
{

    public LayerMask collisionMask;
    public TrailRenderer _trailRenderer;
    
    public ProjectileState state { get; set; }
    
    public void OnTimeWarpEnter(float timeScale)
    {
        state.timeScale = timeScale;
    }

    public void OnTimeWarpExit()
    {
        state.timeScale = 1.0f;
    }

    public void Recycle()
    {
        gameObject.SetActive(false);
    }

    public void Reset(Vector3 position)
    {
        transform.position = position;
        _trailRenderer.Clear();
        gameObject.SetActive(true);
    }
}

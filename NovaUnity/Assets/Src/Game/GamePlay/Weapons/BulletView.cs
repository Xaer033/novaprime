using UnityEngine;
public class BulletView : MonoBehaviour, ITimeWarpTarget
{

    public LayerMask       collisionMask;
    public TrailRenderer   _trailRenderer;
    // public NetworkIdentity netIdentity;
    
    public ProjectileState state { get; set; }
    
    public void OnTimeWarpEnter(float timeScale)
    {
        // state.timeScale = timeScale;
    }

    public void OnTimeWarpExit()
    {
        // state.timeScale = 1.0f;
    }

    public void Recycle()
    {
        _trailRenderer.Clear();
        _trailRenderer.emitting = false;
        gameObject.SetActive(false);
    }

    public void Reset(Vector3 position)
    {
        transform.position = position;
        gameObject.SetActive(true);
        
        _trailRenderer.Clear();
        _trailRenderer.emitting = true;
    }
}

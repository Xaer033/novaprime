using System;
using Mirror;
using UnityEngine;
using UnityEngine.Serialization;

public class PlatformView : NetworkBehaviour, ITimeWarpTarget
{
    public PlatformData data;
    public PlatformType platformType;
    public string triggerTag;
    public PlatformCycleMode cycleMode;
    public float waitTime;
    [Range(0,2)]
    public float easeAmount = 1;
    public float speed;
    [HideInInspector]
    public Vector2 startPosition;

    public Vector2[] localWaypoints;
    public LayerMask passengerMask;
    public float     distanceBetweenRays = 0.2f;

    public Collider2D collisionCollider;
    [FormerlySerializedAs("_viewRoot")]
    public Transform viewRoot;

    public int               index;
    // public NetworkIdentity   netIdentity;
    public PlatformState     state;
    public RaycastController _raycastController;
    
    public event Action<PlatformView> onClientStart;
    
    // Start is called before the first frame update
    void Awake()
    {
        startPosition = transform.position;
        _raycastController = new RaycastController(distanceBetweenRays, collisionCollider, passengerMask, collisionSkinHeight:0.1f);
    }
    
    public void OnTimeWarpEnter(float timeScale)
    {
        state.timeScale = timeScale;
    }
    
    public void OnTimeWarpExit()
    {
        state.timeScale = 1.0f;
    }

    public override void OnStartClient()
    {
        onClientStart?.Invoke(this);
    }

    private void OnDrawGizmos()
    {
        const float alphaColor = 0.5f;
        
        if (localWaypoints != null && collisionCollider != null)
        {
            float size = 0.3f;
            for (int i = 0; i < localWaypoints.Length; ++i)
            {
                Gizmos.color = Color.red;
                Vector2 globalPos = Application.isPlaying && state.globalWayPoints != null ? state.globalWayPoints[i] : localWaypoints[i] + new Vector2(transform.position.x, transform.position.y);
                Gizmos.DrawLine(globalPos - Vector2.up * size, globalPos + Vector2.up * size);
                Gizmos.DrawLine(globalPos - Vector2.left * size, globalPos + Vector2.left * size);

                
                Gizmos.color = i == 0 ? new Color(0.42f, 0.75f, 0.4f, alphaColor) : new Color(0.2f, 0.2f, 1.0f, alphaColor);
                Vector3 platformSize = collisionCollider.bounds.size;
                Gizmos.DrawCube(globalPos, platformSize);
            }
        }
    }
}

public struct PassengerMovement
{
    public readonly Transform transform;
    public readonly Vector2 velocity;
    public readonly bool isOnPlatform;
    public readonly bool moveBeforePlatform;

    public PassengerMovement(Transform t, Vector2 _velocity, bool _isOnPlatform, bool _moveBeforePlatform)
    {
        transform = t;
        velocity = _velocity;
        isOnPlatform = _isOnPlatform;
        moveBeforePlatform = _moveBeforePlatform;
    }
}

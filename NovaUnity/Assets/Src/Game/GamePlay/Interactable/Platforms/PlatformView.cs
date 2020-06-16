using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlatformView : MonoBehaviour, ITimeWarpTarget
{
    public PlatformState.CycleMode cycleMode;
    public float waitTime;
    [Range(0,2)]
    public float easeAmount = 1;
    
    public float speed;
    public Vector3[] localWaypoints;
    
    public LayerMask passengerMask;
    public float distanceBetweenRays = 0.2f;


    public Collider2D collisionCollider;
    [FormerlySerializedAs("_viewRoot")]
    public Transform viewRoot;

    
    public PlatformState state;
    public Vector3 startPosition;
    public RaycastController _raycastController;
    
    // Start is called before the first frame update
    void Awake()
    {
        startPosition = transform.position;
        _raycastController = new RaycastController(distanceBetweenRays, collisionCollider, passengerMask);
    }
    
    public void OnTimeWarpEnter(float timeScale)
    {
        state.timeScale = timeScale;
    }
    
    public void OnTimeWarpExit()
    {
        state.timeScale = 1.0f;
    }

    void Update()
    {
    //     float alpha = (Time.time - Time.fixedTime) / Time.fixedDeltaTime;
    //    _viewRoot.position = Vector3.Lerp(state.prevPosition, state.position, alpha);
    }
    
    private void OnDrawGizmos()
    {
        if (localWaypoints != null)
        {
            float size = 0.3f;
            for (int i = 0; i < localWaypoints.Length; ++i)
            {
                Gizmos.color = Color.red;
                Vector3 globalPos = Application.isPlaying && state != null ? state.globalWayPoints[i] : localWaypoints[i] + transform.position;
                Gizmos.DrawLine(globalPos - Vector3.up * size, globalPos + Vector3.up * size);
                Gizmos.DrawLine(globalPos - Vector3.left * size, globalPos + Vector3.left * size);

                
                Gizmos.color = i == 0 ? new Color(0.42f, 0.75f, 0.4f, 0.8f) : new Color(0.2f, 0.2f, 1.0f, 0.8f);
                Vector3 platformSize = collisionCollider.bounds.size;
                Gizmos.DrawCube(globalPos, platformSize);
            }
        }
    }
}

public struct PassengerMovement
{
    public readonly Transform transform;
    public readonly Vector3 velocity;
    public readonly bool isOnPlatform;
    public readonly bool moveBeforePlatform;

    public PassengerMovement(Transform t, Vector3 _velocity, bool _isOnPlatform, bool _moveBeforePlatform)
    {
        transform = t;
        velocity = _velocity;
        isOnPlatform = _isOnPlatform;
        moveBeforePlatform = _moveBeforePlatform;
    }
}

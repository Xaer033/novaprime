using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlatformView : MonoBehaviour, ITimeWarpTarget
{
    public enum CycleMode
    {
        YOYO,
        CYCLIC
    }

    public CycleMode cycleMode;
    public float waitTime;
    [Range(0,2)]
    public float easeAmount = 1;
    
    public float speed;
    public Vector3[] localWaypoints;
    
    public LayerMask passengerMask;
    public float distanceBetweenRays = 0.2f;


    public Collider collisionCollider;

    private RaycastController _raycastController;
    private HashSet<Transform> _movedPassengersSet = new HashSet<Transform>();
    private List<PassengerMovement> _passengerMovementList = new List<PassengerMovement>();
    private Dictionary<Transform, IPlatformPassenger> _passengerDict = new Dictionary<Transform, IPlatformPassenger>();

    private int _fromWaypointIndex;
    private float _percentBetweenWaypoints;
    private float _nextMoveTime;
    private float _timeScale = 1.0f;
    
    private Vector3[] _globalWayPoints;
    // Start is called before the first frame update
    void Awake()
    {
        _raycastController = new RaycastController(distanceBetweenRays, collisionCollider, passengerMask);
        _globalWayPoints = new Vector3[localWaypoints.Length];
        for (int i = 0; i < localWaypoints.Length; ++i)
        {
            _globalWayPoints[i] = localWaypoints[i] + transform.position;
        }
    }
    
    public void OnTimeWarpEnter(float timeScale)
    {
        _timeScale = timeScale;
    }
    
    public void OnTimeWarpExit()
    {
        _timeScale = 1.0f;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        float deltaTime = Time.fixedDeltaTime * _timeScale;
        float time = Time.fixedTime;
        
        _raycastController.UpdateRaycastOrigins();
        Vector3 velocity = _calculatePlatformMovement(deltaTime, time);
        
        _calculatePassengerMovement(velocity);

        _movePassengers(true);
        transform.Translate(velocity);
        _movePassengers(false);

    }

    private Vector3 _calculatePlatformMovement(float deltaTime, float time)
    {
        if (time < _nextMoveTime)
        {
            return Vector3.zero;
        }
        
        _fromWaypointIndex %= _globalWayPoints.Length;
        
        int toWaypointIndex = (_fromWaypointIndex + 1) % _globalWayPoints.Length;
        Vector3 fromWaypoint = _globalWayPoints[_fromWaypointIndex];
        Vector3 toWaypoint = _globalWayPoints[toWaypointIndex];
        float distanceBetweenWaypoint = Vector3.Distance(fromWaypoint, toWaypoint);
        _percentBetweenWaypoints += deltaTime * speed / distanceBetweenWaypoint;
        _percentBetweenWaypoints = Mathf.Clamp01(_percentBetweenWaypoints);

        float lerpValue = _ease(_percentBetweenWaypoints);

        Vector3 newPos = Vector3.Lerp(fromWaypoint, toWaypoint, lerpValue);
        if (_percentBetweenWaypoints >= 1)
        {
            _percentBetweenWaypoints = 0;
            _fromWaypointIndex++;

            _nextMoveTime = time + waitTime;
            if ( cycleMode == CycleMode.YOYO && _fromWaypointIndex >= _globalWayPoints.Length - 1)
            {
                _fromWaypointIndex = 0;
                System.Array.Reverse(_globalWayPoints);
            }
        }
        return newPos - transform.position;
    }

    private float _ease(float x)
    {
        float a = easeAmount + 1;
        return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(1 - x, a));
    }
    private void _movePassengers(bool beforeMovePlatform)
    {
        for (int i = 0; i < _passengerMovementList.Count; ++i)
        {
            var passenger = _passengerMovementList[i];
            
            IPlatformPassenger platformPat;
            if(!_passengerDict.TryGetValue(passenger.transform, out platformPat))
            {
                platformPat = passenger.transform.gameObject.GetComponent<IPlatformPassenger>();
                _passengerDict[passenger.transform] = platformPat;
            }
            
            if (passenger.moveBeforePlatform == beforeMovePlatform && platformPat != null)
            { 
                platformPat.RequestMovement(passenger);
                
//                passenger.transform.Translate(passenger.velocity);
            }
        }
    }
    private void _calculatePassengerMovement(Vector3 velocity)
    {
        _movedPassengersSet.Clear();
        _passengerMovementList.Clear();
        
        float directionX = Mathf.Sign(velocity.x);
        float directionY = Mathf.Sign(velocity.y);
        RaycastHit hit;

        // Moving vertically
        if (Mathf.Abs(velocity.y)> 0)
        {
            float rayLength = Mathf.Abs(velocity.y) + _raycastController.skinHeight;
            
            for (int i = 0; i < _raycastController.verticalRayCount; ++i)
            {
                Vector3 rayOrigin = (directionY == -1) ? _raycastController.origins.bottomLeft : _raycastController.origins.topLeft;
                rayOrigin += Vector3.right * (_raycastController.verticalRaySpacing * i);
               
                Debug.DrawRay(rayOrigin, Vector3.up * directionY * rayLength, Color.blue);

                bool isHit = Physics.Raycast(rayOrigin, Vector3.up * directionY, out hit, rayLength, passengerMask);
                if (isHit &&  Mathf.Abs(hit.distance) > 0)
                {
                    if (!_movedPassengersSet.Contains(hit.transform))
                    {
                        _movedPassengersSet.Add(hit.transform);
                        
                        float pushX = (directionY == 1) ? velocity.x : 0;
                        float pushY = velocity.y - (hit.distance - _raycastController.skinHeight) * directionY;
                    
                        _passengerMovementList.Add( 
                            new PassengerMovement(
                                hit.transform, 
                                new Vector3(pushX, pushY), 
                                directionY == 1, 
                                true));
                    }
                }
            }
        }

        //Moving Horizontally
        if (Mathf.Abs(velocity.x )> 0)
        {
            float rayLength = Mathf.Abs(velocity.x) + _raycastController.skinWidth;
            for (int i = 0; i < _raycastController.horizontalRayCount; ++i)
            {
                Vector3 rayOrigin = (directionX == -1) ? _raycastController.origins.bottomLeft : _raycastController.origins.bottomRight;
                rayOrigin += Vector3.up * (_raycastController.horizontalRaySpacing * i);
                
                Debug.DrawRay(rayOrigin, Vector3.right * directionX * rayLength, Color.yellow);

                bool isHit = Physics.Raycast(rayOrigin, Vector3.right * directionX, out hit, rayLength, passengerMask);
                if (isHit && Mathf.Abs(hit.distance) > 0)
                {
                    if (!_movedPassengersSet.Contains(hit.transform))
                    {
                        _movedPassengersSet.Add(hit.transform);
                        float pushX = velocity.x - (hit.distance - _raycastController.skinWidth) * directionX;
                        float pushY = -_raycastController.skinWidth;
                        
                        _passengerMovementList.Add( 
                            new PassengerMovement(
                                hit.transform, 
                                new Vector3(pushX, pushY), 
                                false, 
                                true));
                    }
                }
            }
        }
        
        // The passenger is on top of a horizontal platform or a downward moving platform
        if(directionY == -1 || Mathf.Abs(velocity.y ) <= 0.00001f && Mathf.Abs(velocity.x) > 0)
        {
            float skin = Mathf.Abs(directionY) > 0 ? _raycastController.skinHeight : _raycastController.skinWidth;
            float rayLength =  skin * 2.0f;
            
            for (int i = 0; i < _raycastController.verticalRayCount; ++i)
            {
                Vector3 rayOrigin = _raycastController.origins.topLeft + Vector3.right * (_raycastController.verticalRaySpacing * i);
               
                Debug.DrawRay(rayOrigin, Vector3.up * rayLength, Color.green);
                
                bool isHit = Physics.Raycast(rayOrigin, Vector3.up, out hit, rayLength, passengerMask);
                if(isHit && Mathf.Abs(hit.distance) > 0)
                {
                    if (!_movedPassengersSet.Contains(hit.transform))
                    {
                        _movedPassengersSet.Add(hit.transform);
                        float pushX = velocity.x;
                        float pushY = velocity.y;

                        _passengerMovementList.Add( 
                            new PassengerMovement(
                                hit.transform, 
                                new Vector3(pushX, pushY), 
                                true, 
                                false));
                    }
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (localWaypoints != null)
        {
            float size = 0.3f;
            for (int i = 0; i < localWaypoints.Length; ++i)
            {
                Gizmos.color = Color.red;
                Vector3 globalPos = Application.isPlaying ? _globalWayPoints[i] : localWaypoints[i] + transform.position;
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
    public Transform transform;
    public Vector3 velocity;
    public bool isOnPlatform;
    public bool moveBeforePlatform;

    public PassengerMovement(Transform t, Vector3 _velocity, bool _isOnPlatform, bool _moveBeforePlatform)
    {
        transform = t;
        velocity = _velocity;
        isOnPlatform = _isOnPlatform;
        moveBeforePlatform = _moveBeforePlatform;
    }
}
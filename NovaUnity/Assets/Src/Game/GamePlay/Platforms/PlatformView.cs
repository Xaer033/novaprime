using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlatformView : MonoBehaviour
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
    public int horizontalRayCount = 4;
    public int verticalRayCount = 4;


    public Collider collider;

    private RaycastController _raycastController;
    private HashSet<Transform> _movedPassengersSet = new HashSet<Transform>();
    private List<PassengerMovement> _passengerMovementList = new List<PassengerMovement>();
    private Dictionary<Transform, IPlatformPassenger> _passengerDict = new Dictionary<Transform, IPlatformPassenger>();

    private int _fromWaypointIndex;
    private float _percentBetweenWaypoints;
    private float _nextMoveTime;
    
    private Vector3[] _globalWayPoints;
    // Start is called before the first frame update
    void Awake()
    {
        _raycastController = new RaycastController(horizontalRayCount, verticalRayCount, collider, passengerMask);
        _globalWayPoints = new Vector3[localWaypoints.Length];
        for (int i = 0; i < localWaypoints.Length; ++i)
        {
            _globalWayPoints[i] = localWaypoints[i] + transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        _raycastController.Step();
        Vector3 velocity = _calculatePlatformMovement(Time.deltaTime, Time.time);
        
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
        float distanceBetweenWaypoint = Vector3.Distance(fromWaypoint, toWaypoint) + 0.0001f;
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
        if (velocity.y != 0)
        {
            float rayLength = Mathf.Abs(velocity.y) + _raycastController.skinWidth;
            
            for (int i = 0; i < verticalRayCount; ++i)
            {
                Vector3 rayOrigin = (directionY == -1) ? _raycastController.origins.bottomLeft : _raycastController.origins.topLeft;
                rayOrigin += Vector3.right * (_raycastController.verticalRaySpacing * i);
                if (Physics.Raycast(rayOrigin, Vector3.up * directionY, out hit, rayLength, passengerMask))
                {
                    if (!_movedPassengersSet.Contains(hit.transform))
                    {
                        _movedPassengersSet.Add(hit.transform);
                        
                        float pushX = (directionY == 1) ? velocity.x : 0;
                        float pushY = velocity.y - (hit.distance - _raycastController.skinWidth) * directionY;
                        
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
        if (velocity.x != 0)
        {
            float rayLength = Mathf.Abs(velocity.x) + _raycastController.skinWidth;
            for (int i = 0; i < horizontalRayCount; ++i)
            {
                Vector3 rayOrigin = (directionX == -1) ? _raycastController.origins.bottomLeft : _raycastController.origins.bottomRight;
                rayOrigin += Vector3.up * (_raycastController.horizontalRaySpacing * i);
                if (Physics.Raycast(rayOrigin, Vector3.right * directionX, out hit, rayLength, passengerMask))
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
        
        if(directionY == -1 || (velocity.y == 0 && velocity.x !=0))
        {
            float rayLength = _raycastController.skinWidth * 2.0f;
            
            for (int i = 0; i < verticalRayCount; ++i)
            {
                Vector3 rayOrigin = _raycastController.origins.topLeft + (_raycastController.verticalRaySpacing * i * Vector3.right);
                if (Physics.Raycast(rayOrigin, Vector3.up, out hit, rayLength, passengerMask))
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
            Gizmos.color = Color.red;
            float size = 0.3f;
            for (int i = 0; i < localWaypoints.Length; ++i)
            {
                Vector3 globalPos = Application.isPlaying ? _globalWayPoints[i] : localWaypoints[i] + transform.position;
                Gizmos.DrawLine(globalPos - Vector3.up * size, globalPos + Vector3.up * size);
                Gizmos.DrawLine(globalPos - Vector3.left * size, globalPos + Vector3.left * size);
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
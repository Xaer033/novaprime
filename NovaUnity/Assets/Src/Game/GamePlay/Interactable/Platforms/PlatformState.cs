using System.Collections.Generic;
using UnityEngine;

public class PlatformState
{
    public PlatformState(Vector2 startPosition, int raycastArraySize, Vector2[] localWayPoints)
    {
        position                = startPosition;
        prevPosition            = startPosition;
        velocity                = Vector2.zero;
        timeScale               = 1.0f;
        nextMoveTime            = 0;
        fromWaypointIndex       = 0;
        percentBetweenWaypoints = 0;
        wasTriggered            = false;
        
        int raycastSize       = raycastArraySize > 0 ? raycastArraySize : 1;
        raycastHits           = new RaycastHit2D[raycastSize];
        movedPassengersSet    = new HashSet<Transform>();
        passengerMovementList = new List<PassengerMovement>();
        passengerDict         = new Dictionary<Transform, IPlatformPassenger>();

        int localWayPointSize = localWayPoints != null && localWayPoints.Length > 0 ? localWayPoints.Length : 0;
        globalWayPoints = new Vector2[localWayPointSize];
        
        for (int j = 0; j < localWayPointSize; ++j)
        {
            globalWayPoints[j] = position + localWayPoints[j];
        }
    }
    
    public Vector2 prevPosition;
    public Vector2 position;
    public Vector2 velocity;
    
    public int   fromWaypointIndex;
    public float percentBetweenWaypoints;
    public float nextMoveTime;
    public bool  wasTriggered;
    public float timeScale;
    
    public RaycastHit2D[] raycastHits;
    public Vector2[]      globalWayPoints;
    
    public HashSet<Transform>                        movedPassengersSet;
    public List<PassengerMovement>                   passengerMovementList;
    public Dictionary<Transform, IPlatformPassenger> passengerDict;
}

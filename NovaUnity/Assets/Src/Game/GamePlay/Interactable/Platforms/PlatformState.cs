using System.Collections.Generic;
using UnityEngine;

public class PlatformState
{
    public PlatformState(Vector2 startPosition, int raycastArraySize, Vector2[] localWayPoints)
    {
        int raycastSize = raycastArraySize > 0 ? raycastArraySize : 1;
        
        position = startPosition;
        prevPosition = startPosition;
        raycastHits = new RaycastHit2D[raycastSize];

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
    
    public int fromWaypointIndex;
    public float percentBetweenWaypoints;
    public float nextMoveTime;
    public bool wasTriggered;
    public float timeScale = 1.0f;
    
    public RaycastHit2D[] raycastHits;
    public Vector2[] globalWayPoints;
    
    // public RaycastController raycastController;
    public HashSet<Transform> movedPassengersSet = new HashSet<Transform>();
    public List<PassengerMovement> passengerMovementList = new List<PassengerMovement>();
    public Dictionary<Transform, IPlatformPassenger> passengerDict = new Dictionary<Transform, IPlatformPassenger>();
}

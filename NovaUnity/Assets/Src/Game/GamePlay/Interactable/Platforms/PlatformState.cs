using System.Collections.Generic;
using UnityEngine;

public class PlatformState
{
    public PlatformState(Vector3 startPosition, int raycastArraySize, Vector3[] localWayPoints)
    {
        int raycastSize = raycastArraySize > 0 ? raycastArraySize : 1;
        
        position = startPosition;
        prevPosition = startPosition;
        raycastHits = new RaycastHit2D[raycastSize];

        int localWayPointSize = localWayPoints != null && localWayPoints.Length > 0 ? localWayPoints.Length : 0;
        globalWayPoints = new Vector3[localWayPointSize];
        
        for (int j = 0; j < localWayPointSize; ++j)
        {
            globalWayPoints[j] = position + localWayPoints[j];
        }
    }
    
    public Vector3 prevPosition;
    public Vector3 position;
    public Vector3 velocity;
    
    public int fromWaypointIndex;
    public float percentBetweenWaypoints;
    public float nextMoveTime;
    public bool wasTriggered;
    public float timeScale = 1.0f;
    
    public RaycastHit2D[] raycastHits;
    public Vector3[] globalWayPoints;
    
    // public RaycastController raycastController;
    public HashSet<Transform> movedPassengersSet = new HashSet<Transform>();
    public List<PassengerMovement> passengerMovementList = new List<PassengerMovement>();
    public Dictionary<Transform, IPlatformPassenger> passengerDict = new Dictionary<Transform, IPlatformPassenger>();
}

using System;
using GhostGen;
using UnityEngine;

public class TimePlatformController : BasePlatformController
{
    protected override Vector2 calculatePlatformMovement(ref PlatformState state, PlatformView view, float adjustedDeltaTime, float time)
    {
        if (time < state.nextMoveTime)
        {
            return Vector2.zero;
        }

        if(state.globalWayPoints.Length > 0)
        {
            state.fromWaypointIndex %= state.globalWayPoints.Length;
        }
        
        int toWaypointIndex = (state.fromWaypointIndex + 1) % state.globalWayPoints.Length;
        Vector2 fromWaypoint = state.globalWayPoints[state.fromWaypointIndex];
        Vector2 toWaypoint = state.globalWayPoints[toWaypointIndex];
        float distanceBetweenWaypoint = Vector2.Distance(fromWaypoint, toWaypoint);
        float validDistance = distanceBetweenWaypoint > 0 ? distanceBetweenWaypoint : 0.001f; 
        
        // Delta time has already been mutliplied by state.timeScale
        state.percentBetweenWaypoints += adjustedDeltaTime * view.speed / validDistance;
        state.percentBetweenWaypoints = Mathf.Clamp01(state.percentBetweenWaypoints);
        
        float lerpValue = MathUtil.Ease(state.percentBetweenWaypoints, view.easeAmount);

        Vector2 newPos = Vector2.Lerp(fromWaypoint, toWaypoint, lerpValue);
        if (state.percentBetweenWaypoints >= 1)
        {
            state.percentBetweenWaypoints = 0;
            state.fromWaypointIndex++;

            state.nextMoveTime = time + view.waitTime;
            if(view.cycleMode == PlatformCycleMode.YOYO && state.fromWaypointIndex >= state.globalWayPoints.Length - 1)
            {
                state.fromWaypointIndex = 0;
                Array.Reverse(state.globalWayPoints);
            }
        }
        return newPos - state.position;
    }
}

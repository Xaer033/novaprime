using System;
using System.Collections;
using System.Collections.Generic;
using GhostGen;
using UnityEngine;

public class TriggerPlatformController : BasePlatformController
{
    protected override Vector2 calculatePlatformMovement(PlatformState state, PlatformView view, float adjustedDeltaTime, float time)
    {
        if (time < state.nextMoveTime)
        {
            return Vector2.zero;
        }

        if(state.globalWayPoints.Length > 0)
        {
            state.fromWaypointIndex %= state.globalWayPoints.Length;
        }

        
        int fromWaypointIndex = state.fromWaypointIndex;
        int toWaypointIndex = (state.fromWaypointIndex + 1) % state.globalWayPoints.Length;
        
        if(fromWaypointIndex + 1 >= state.globalWayPoints.Length && view.cycleMode == PlatformCycleMode.ONCE)
        {
            return Vector2.zero;
        }
            
        if(fromWaypointIndex == 0 && toWaypointIndex == 1 && state.percentBetweenWaypoints < 0.0001f)
        {
            if(state.wasTriggered)
            {
                state.wasTriggered = false;
            }
            else
            {
                return Vector2.zero;
            }
        }
        else
        {
            state.wasTriggered = false;
        }
        
        Vector2 fromWaypoint = state.globalWayPoints[fromWaypointIndex];
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
            
            //Got to the end of the waypoint array
            if(state.fromWaypointIndex >= state.globalWayPoints.Length - 1 && view.cycleMode == PlatformCycleMode.YOYO)
            { 
                state.percentBetweenWaypoints = fromWaypointIndex == 0 ? 0.0015f : 0.0f;
                state.fromWaypointIndex = 0;
                Array.Reverse(state.globalWayPoints);
            }
        }
        return newPos - state.position;
    }
}

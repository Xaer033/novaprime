using System;
using System.Collections;
using System.Collections.Generic;
using GhostGen;
using UnityEngine;

public class TriggerPlatformController : BasePlatformController
{
    protected override Vector3 calculatePlatformMovement(PlatformState state, PlatformView view, float adjustedDeltaTime, float time)
    {
        if (time < state.nextMoveTime)
        {
            return Vector3.zero;
        }

        if(state.globalWayPoints.Length > 0)
        {
            state.fromWaypointIndex %= state.globalWayPoints.Length;
        }

        int fromWaypointIndex = state.fromWaypointIndex;
        int toWaypointIndex = (state.fromWaypointIndex + 1) % state.globalWayPoints.Length;
        if(fromWaypointIndex == 0 && toWaypointIndex == 1 && state.percentBetweenWaypoints < 0.0001f)
        {
            if(state.wasTriggered)
            {
                state.wasTriggered = false;
            }
            else
            {
                return Vector3.zero;
            }
            // 
        }
        else
        {
            state.wasTriggered = false;
        }
        
        Vector3 fromWaypoint = state.globalWayPoints[fromWaypointIndex];
        Vector3 toWaypoint = state.globalWayPoints[toWaypointIndex];
        float distanceBetweenWaypoint = Vector3.Distance(fromWaypoint, toWaypoint);
        float validDistance = distanceBetweenWaypoint > 0 ? distanceBetweenWaypoint : 0.001f; 
        
        // Delta time has already been mutliplied by state.timeScale
        state.percentBetweenWaypoints += adjustedDeltaTime * view.speed / validDistance;
        state.percentBetweenWaypoints = Mathf.Clamp01(state.percentBetweenWaypoints);
        
        float lerpValue = MathUtil.Ease(state.percentBetweenWaypoints, view.easeAmount);

        Vector3 newPos = Vector3.Lerp(fromWaypoint, toWaypoint, lerpValue);
        if (state.percentBetweenWaypoints >= 1)
        {
            state.percentBetweenWaypoints = 0;
            state.fromWaypointIndex++;

            state.nextMoveTime = time + view.waitTime;
            if(view.cycleMode == PlatformCycleMode.YOYO && state.fromWaypointIndex >= state.globalWayPoints.Length - 1)
            {
                state.percentBetweenWaypoints = fromWaypointIndex == 0 ? 0.0015f : 0.0f;
                state.fromWaypointIndex = 0;
                Array.Reverse(state.globalWayPoints);
            }
        }
        return newPos - state.position;
    }
}

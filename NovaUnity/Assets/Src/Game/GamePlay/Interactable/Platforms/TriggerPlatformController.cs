using System;
using System.Collections;
using System.Collections.Generic;
using GhostGen;
using UnityEngine;

public class TriggerPlatformController : BasePlatformController
{
    protected override Vector3 calculatePlatformMovement(PlatformState state, PlatformView view, float deltaTime, float time)
    {
        int toWaypointIndex = (state.fromWaypointIndex + 1) % state.globalWayPoints.Length;
        Vector3 fromWaypoint = state.globalWayPoints[state.fromWaypointIndex];
        Vector3 toWaypoint = state.globalWayPoints[toWaypointIndex];
        
        float distanceBetweenWaypoint = Vector3.Distance(fromWaypoint, toWaypoint);
        float validDistance = distanceBetweenWaypoint > 0 ? distanceBetweenWaypoint : 0.001f; 
        state.percentBetweenWaypoints += deltaTime * view.speed / validDistance;
        state.percentBetweenWaypoints = Mathf.Clamp01(state.percentBetweenWaypoints);
        
        float lerpValue = MathUtil.Ease(state.percentBetweenWaypoints, view.easeAmount);

        Vector3 newPos = Vector3.Lerp(fromWaypoint, toWaypoint, lerpValue);
        if (state.percentBetweenWaypoints >= 1)
        {
            state.percentBetweenWaypoints = 0;
        }
        return newPos - state.position;
    }
}

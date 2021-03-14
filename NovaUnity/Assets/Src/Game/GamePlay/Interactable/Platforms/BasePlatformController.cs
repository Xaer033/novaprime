using Mirror;
using UnityEngine;

public class BasePlatformController
{
    public void UpdatePlatform(ref PlatformState state, PlatformView view, float adjustedDeltaTime, float time)
    {
        view._raycastController.UpdateRaycastOrigins();
        Vector2 newVelocity = calculatePlatformMovement(ref state, view, adjustedDeltaTime, time);
        
        calculatePassengerMovement(ref state, view, state.velocity);

        state.velocity = newVelocity;
        state.prevPosition = state.position;
        
        movePassengers(ref state, true);
        state.position += state.velocity;
        view.transform.position = state.position; 
        movePassengers(ref state, false);
    }
    
    private void movePassengers(ref PlatformState state, bool beforeMovePlatform)
    {
        for (int i = 0; i < state.passengerMovementList.Count; ++i)
        {
            PassengerMovement passenger = state.passengerMovementList[i];
            
            IPlatformPassenger platformPat;
            if(!state.passengerDict.TryGetValue(passenger.transform, out platformPat))
            {
                platformPat = passenger.transform.gameObject.GetComponent<IPlatformPassenger>();
                state.passengerDict[passenger.transform] = platformPat;
            }
            
            if (passenger.moveBeforePlatform == beforeMovePlatform && platformPat != null)
            { 
                platformPat.RequestMovement(passenger);
            }
        }
    }
    
    protected virtual Vector2 calculatePlatformMovement(ref PlatformState state, PlatformView view, float adjustedDeltaTime, float time)
    {
        return Vector2.zero;
    }
    
    private void calculatePassengerMovement(ref PlatformState state, PlatformView view, Vector2 velocity)
    {
        state.movedPassengersSet.Clear();
        state.passengerMovementList.Clear();
        
        float directionX = Mathf.Sign(velocity.x);
        float directionY = Mathf.Sign(velocity.y);

        RaycastController _raycastController = view._raycastController;
        
        // Moving vertically
        if (Mathf.Abs(velocity.y) > 0)
        {
            float rayLength = Mathf.Abs(velocity.y) + _raycastController.skinHeight;
            
            for (int i = 0; i < _raycastController.verticalRayCount; ++i)
            {
                Vector2 rayOrigin = (directionY == -1) ? _raycastController.origins.bottomLeft : _raycastController.origins.topLeft;
                rayOrigin += Vector2.right * (_raycastController.verticalRaySpacing * i);
               
                Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.blue);

                int hitCount = Physics2D.RaycastNonAlloc(rayOrigin, Vector2.up * directionY, state.raycastHits, rayLength, view.passengerMask);
                for (int x = 0; x < hitCount; ++x)
                {
                    RaycastHit2D hit = state.raycastHits[x];
                    if (Mathf.Abs(hit.distance) > 0)
                    {
                        if (!state.movedPassengersSet.Contains(hit.transform))
                        {
                            state.movedPassengersSet.Add(hit.transform);
                            
                            float pushX = (directionY == 1) ? velocity.x : 0;
                            float pushY = velocity.y - (hit.distance - _raycastController.skinHeight) * directionY;
                        
                            state.passengerMovementList.Add( 
                                new PassengerMovement(
                                    hit.transform, 
                                    new Vector2(pushX, pushY), 
                                    directionY == 1, 
                                    true));
                        }
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
                Vector2 rayOrigin = (directionX == -1) ? _raycastController.origins.bottomLeft : _raycastController.origins.bottomRight;
                rayOrigin += Vector2.up * (_raycastController.horizontalRaySpacing * i);
                
                Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.yellow);

                int hitCount = Physics2D.RaycastNonAlloc(rayOrigin, Vector2.right * directionX, state.raycastHits, rayLength, view.passengerMask);

                for (int x = 0; x < hitCount; ++x)
                {
                    RaycastHit2D hit = state.raycastHits[x];
                    if (Mathf.Abs(hit.distance) > 0)
                    {
                        if (!state.movedPassengersSet.Contains(hit.transform))
                        {
                            state.movedPassengersSet.Add(hit.transform);
                            float pushX = velocity.x - (hit.distance - _raycastController.skinWidth) * directionX;
                            float pushY = -_raycastController.skinWidth;
                            
                            state.passengerMovementList.Add( 
                                new PassengerMovement(
                                    hit.transform, 
                                    new Vector2(pushX, pushY), 
                                    false, 
                                    true));
                        }
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
                Vector2 rayOrigin = _raycastController.origins.topLeft + Vector2.right * (_raycastController.verticalRaySpacing * i);
               
                Debug.DrawRay(rayOrigin, Vector2.up * rayLength, Color.green);
                
                int hitCount = Physics2D.RaycastNonAlloc(rayOrigin, Vector2.up, state.raycastHits, rayLength, view.passengerMask);
                
                for (int x = 0; x < hitCount; ++x)
                {
                    RaycastHit2D hit = state.raycastHits[x];
                    if(Mathf.Abs(hit.distance) > 0)
                    {
                        if (!state.movedPassengersSet.Contains(hit.transform))
                        {
                            state.movedPassengersSet.Add(hit.transform);
                            float pushX = velocity.x;
                            float pushY = velocity.y;

                            state.passengerMovementList.Add( 
                                new PassengerMovement(
                                    hit.transform, 
                                    new Vector2(pushX, pushY), 
                                    true, 
                                    false));
                        }
                    }
                }
            }
        }
    }
}

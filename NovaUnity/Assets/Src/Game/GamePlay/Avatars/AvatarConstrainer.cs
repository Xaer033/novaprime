using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;

public class AvatarConstrainer : MonoBehaviour
{
    private const int kRaycastHitCount = 20;
    private const float kFallThroughPlatformWaitDuration = 0.15f;
    
    public Collider collisionCollider;
    
    public LayerMask collisionMask;
    public float distanceBetweenRays = 0.08f;
    public float maxSlopeAngle = 50;
    
    [Range(0, 1)] 
    public float walkStepRayPercentage = 0.25f;
    
    private float _walkStepHeight = 0.25f;
    private int _walkStepIndex;
    private RaycastController _raycastController;
    private CollisionInfo _collisionInfo;
    private FrameInput _input;
    private RaycastHit[] _raycastHits;
    
    private float _resetPlatformTime;

    public CollisionInfo collisionInfo
    {
        get { return _collisionInfo; }
    }
    
    void Awake()
    {
        _raycastHits = new RaycastHit[kRaycastHitCount];
        
        _raycastController = new RaycastController(
            distanceBetweenRays,
            collisionCollider,
            collisionMask);
        
        _collisionInfo = new CollisionInfo();
        _collisionInfo.faceDir = 1;
    }
    
    public Vector3 Move(Vector3 moveDelta, bool isOnPlatform, FrameInput input = default(FrameInput))
    {
        return _move(moveDelta, isOnPlatform, input);
    }
    
    private Vector3 _move(Vector3 moveDelta, bool isOnPlatform, FrameInput input)
    {
        _input = input;
        
        _raycastController.distanceBetweenRays = distanceBetweenRays;
        _raycastController.UpdateRaycastOrigins();
        _walkStepIndex = Mathf.RoundToInt(_raycastController.verticalRayCount * walkStepRayPercentage);
        
        _collisionInfo.Reset();
        _collisionInfo.oldMoveDelta = moveDelta;

        if (moveDelta.y < 0)
        {
            _decsendSlope(ref moveDelta);
        }

        if (Mathf.Abs(moveDelta.x) > 0)
        {
            _collisionInfo.faceDir = (int)Mathf.Sign(moveDelta.x);
        }
        
        _horizontalCollisions(ref moveDelta);
        _verticalCollisions(ref moveDelta);
        
        if (isOnPlatform)
        {
            _collisionInfo.below = true;
        }
        
        return moveDelta;
    }

    protected void _horizontalCollisions(ref Vector3 moveDelta)
    {
        float directionX = _collisionInfo.faceDir;
        float rayLength = Mathf.Abs(moveDelta.x) + _raycastController.skinWidth;

       Vector3 rayOrigin = (directionX == -1) ? _raycastOrigins.bottomLeft : _raycastOrigins.bottomRight;
        if (Mathf.Abs(moveDelta.x) < _raycastController.skinWidth)
        {
            rayLength = 2 * _raycastController.skinWidth;
        }

        _walkStepHeight =  (_raycastController.horizontalRaySpacing * _walkStepIndex);
        
//        RaycastHit hit;
        for (int i = 0; i < _raycastController.horizontalRayCount; ++i)
        {
            Vector3 firstRayOrigin = rayOrigin;
            firstRayOrigin += Vector3.up * (_raycastController.horizontalRaySpacing * i);
            
            Debug.DrawRay(firstRayOrigin, Vector3.right * directionX, Color.cyan);
            
            int hitCount = Physics.RaycastNonAlloc(firstRayOrigin, Vector3.right * directionX, _raycastHits, rayLength, collisionMask);
            // Potential future bug for not looping through all hitCount
            if (hitCount > 0)
            {
                RaycastHit hit = _raycastHits[0];
                if (Mathf.Abs(hit.distance) < 0.00001f)
                {
                    continue;
                }

                if (hit.collider.CompareTag("Platform"))
                {
                    continue;
                }

                float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
                if (i == 0 && slopeAngle <= maxSlopeAngle)
                {
                    if (_collisionInfo.decendingSlope)
                    {
                        _collisionInfo.decendingSlope = false;
                        moveDelta = _collisionInfo.oldMoveDelta;
                    }
                    float distanceToSlopeStart = 0;
                    if (slopeAngle != _collisionInfo.slopeAngleOld)
                    {
                        distanceToSlopeStart = hit.distance - _raycastController.skinWidth;
                        moveDelta.x -= distanceToSlopeStart * directionX;
                    }

                    _climbSlope(ref moveDelta, slopeAngle, hit.normal);
                    moveDelta.x += distanceToSlopeStart * directionX;
                }

                if (_collisionInfo.belowOld && i <= _walkStepIndex)
                {
                    _collisionInfo.stepUp = true;
                }
                else
                {
                    _collisionInfo.stepUp = false;
                    
                    if (!_collisionInfo.climbingSlope || slopeAngle > maxSlopeAngle)
                    {    
                        moveDelta.x = (hit.distance - _raycastController.skinWidth) * directionX;
                        rayLength = hit.distance;

                        if (_collisionInfo.climbingSlope)
                        {
                            moveDelta.y = Mathf.Tan(_collisionInfo.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveDelta.x);
                        }

                        _collisionInfo.left = directionX == -1;
                        _collisionInfo.right = directionX == 1;
                    }
                }
            }
        }
    }
    
    protected void _verticalCollisions(ref Vector3 moveDelta)
    {
        float directionY = Mathf.Sign(moveDelta.y);

        bool isOnSlope = (_collisionInfo.climbingSlope || _collisionInfo.decendingSlope);
        float walkStepHeight = directionY == 1 || isOnSlope ? 0 : _walkStepHeight;
        float rayLength = Mathf.Abs(moveDelta.y) + _raycastController.skinHeight + walkStepHeight;
        
        int inputDirY =  _input.verticalMovement == 0 ?  0 : _input.verticalMovement < 0 ? -1 : 1;

        for (int i = 0; i < _raycastController.verticalRayCount; ++i)
        {
            Vector3 rayOrigin = (directionY == -1) ? _raycastOrigins.bottomLeft : _raycastOrigins.topLeft;
            rayOrigin.y = rayOrigin.y + walkStepHeight;
            rayOrigin += Vector3.right * (_raycastController.verticalRaySpacing * i + moveDelta.x);
            
            Debug.DrawRay(rayOrigin, Vector3.up * directionY * rayLength, Color.red);
            
            int hitCount = Physics.RaycastNonAlloc(rayOrigin, Vector3.up * directionY, _raycastHits, rayLength, collisionMask);

            float smallestDist = rayLength;
            RaycastHit activeHit = default;
            int activeIndex = -1;
            for(int x = 0; x < hitCount; ++x)
            {
                RaycastHit hit = _raycastHits[x];

                if (hit.distance < smallestDist)
                {
                    smallestDist = hit.distance;
                    activeHit = hit;
                    activeIndex = x;
                }
            }

            if (activeIndex >= 0)
            {
                if (activeHit.collider.CompareTag( "Platform"))
                {
                    if (directionY == 1 || Mathf.Abs(activeHit.distance) < 0.0001f)
                    {
                        continue;
                    }

                    if (_collisionInfo.fallingThroughPlatform)
                    {
                        continue;
                    }
                    
                    if (inputDirY == -1)
                    {
                        Singleton.instance.StartCoroutine(_startFallThroughPlatformTimer());//Gross...
                        continue;
                    }
                }

                moveDelta.y = (activeHit.distance - _raycastController.skinHeight - walkStepHeight) * directionY;
                rayLength = activeHit.distance;

                if (_collisionInfo.climbingSlope)
                {
                    moveDelta.x = moveDelta.y / Mathf.Tan(_collisionInfo.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(moveDelta.x);
                }
                
                _collisionInfo.below = directionY == -1;
                _collisionInfo.above = directionY == 1;
            }
        }

        if (_collisionInfo.climbingSlope)
        {
            float directionX = Mathf.Sign(moveDelta.x);
            rayLength = Mathf.Abs(moveDelta.x) + _raycastController.skinWidth;
            Vector3 rayOrigin = ((directionX == -1) ? _raycastOrigins.bottomLeft : _raycastOrigins.bottomRight) + Vector3.up * moveDelta.y;
            
            int hitCount = Physics.RaycastNonAlloc(rayOrigin, Vector3.right * directionX, _raycastHits, rayLength, collisionMask);
            if (hitCount > 0)
            {
                RaycastHit hit = _raycastHits[0];
                float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
                if (slopeAngle != _collisionInfo.slopeAngle)
                {
                    moveDelta.x = (hit.distance - _raycastController.skinWidth) * directionX;
                    _collisionInfo.slopeAngle = slopeAngle;
                    _collisionInfo.slopeNormal = hit.normal;
                }
            }
        }
    }

    private void _climbSlope(ref Vector3 moveDelta, float slopeAngle, Vector3 slopeNormal)
    {
        float moveDistance = Mathf.Abs(moveDelta.x);
        float climbVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
        
        moveDelta.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveDelta.x);
        
        if (moveDelta.y <= climbVelocityY)
        {
            moveDelta.y = climbVelocityY;
            _collisionInfo.below = true;
            _collisionInfo.climbingSlope = true;
            _collisionInfo.slopeAngle = slopeAngle;
            _collisionInfo.slopeNormal = slopeNormal;
        }
    }

    private RaycastController.RaycastOrigins _raycastOrigins
    {
        get { return _raycastController.origins; }
    }

    private void _decsendSlope(ref Vector3 moveDelta)
    {
        RaycastHit maxSlopeHitLeft;
        RaycastHit maxSlopeHitRight;
        bool isLeftHit     = Physics.Raycast(_raycastOrigins.bottomLeft, Vector3.down, out maxSlopeHitLeft, Mathf.Abs(moveDelta.y) + _raycastController.skinHeight, collisionMask);
        bool isRightHit    = Physics.Raycast(_raycastOrigins.bottomRight, Vector3.down, out maxSlopeHitRight, Mathf.Abs(moveDelta.y) + _raycastController.skinHeight, collisionMask);

        if(isLeftHit ^ isRightHit)
        {
            if (isLeftHit)
            {
                _slideDownMaxSlope(ref moveDelta, maxSlopeHitLeft);
            }

            if (isRightHit)
            {
                _slideDownMaxSlope(ref moveDelta, maxSlopeHitRight);
            }
        }

        if (!_collisionInfo.slidingDownMaxSlope)
        {
            float directionX = Mathf.Sign(moveDelta.x);
            RaycastHit hit;
            Vector3 rayOrigin = (directionX == -1) ? _raycastOrigins.bottomRight :  _raycastOrigins.bottomLeft;
            
            if (Physics.Raycast(rayOrigin, Vector3.down, out hit, Mathf.Infinity,collisionMask))
            {
                float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
                if (Mathf.Abs(slopeAngle) > 0 && slopeAngle <= maxSlopeAngle)
                {
                    if (Mathf.Sign(hit.normal.x) == directionX)
                    {
                        if (hit.distance - _raycastController.skinHeight <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveDelta.x))
                        {
                            float moveDistance = Mathf.Abs(moveDelta.x);
                            float decendVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                            moveDelta.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveDelta.x);
                            moveDelta.y -= decendVelocityY;

                            _collisionInfo.slopeAngle = slopeAngle;
                            _collisionInfo.decendingSlope = true;
                            _collisionInfo.slopeNormal = hit.normal;
                            _collisionInfo.below = true;
                        }
                    }
                }
            }
        }
    }

    private void _slideDownMaxSlope(ref Vector3 moveDelta, RaycastHit hit)
    {
        float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
        if (slopeAngle > maxSlopeAngle)
        {
            moveDelta.x = hit.normal.x * ((Mathf.Abs(moveDelta.y) - hit.distance) / Mathf.Tan(slopeAngle * Mathf.Deg2Rad));
            _collisionInfo.slopeAngle = slopeAngle;
            _collisionInfo.slidingDownMaxSlope = true;
            _collisionInfo.slopeNormal = hit.normal;
        }
    }
    
    
    private IEnumerator _startFallThroughPlatformTimer()
    {
        _collisionInfo.fallingThroughPlatform = true;
        yield return new WaitForSeconds(kFallThroughPlatformWaitDuration);
        _collisionInfo.fallingThroughPlatform = false;
    }
}    

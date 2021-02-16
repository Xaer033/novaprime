using System.Collections;
using UnityEngine;

public class AvatarConstrainer : MonoBehaviour
{
    private const int kRaycastHitCount = 20;
    private const float kFallThroughPlatformWaitDuration = 0.25f;
    
    public Collider2D collisionCollider;
    
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
    private RaycastHit2D[] _raycastHits;
    private RaycastHit2D[] _leftSlopeCastHits;
    private RaycastHit2D[] _rightSlopeCastHits;
    private RaycastHit2D[] _otherHits;
    
    private float _resetPlatformTime;

    public CollisionInfo collisionInfo
    {
        get { return _collisionInfo; }
    }
    
    void Awake()
    {
        _raycastHits = new RaycastHit2D[kRaycastHitCount];
        _leftSlopeCastHits = new RaycastHit2D[1];
        _rightSlopeCastHits = new RaycastHit2D[1];
        _otherHits = new RaycastHit2D[10];
        
        _raycastController = new RaycastController(
            distanceBetweenRays,
            collisionCollider,
            collisionMask);
        
        _collisionInfo = new CollisionInfo();
        _collisionInfo.faceDir = 1;
    }
    
    public Vector2 Move(Vector2 moveDelta, bool isOnPlatform, FrameInput input = default(FrameInput))
    {
        return _move(moveDelta, isOnPlatform, input);
    }
    
    private Vector2 _move(Vector2 moveDelta, bool isOnPlatform, FrameInput input)
    {
        _input = input;
        
        _raycastController.distanceBetweenRays = distanceBetweenRays;
        _raycastController.UpdateRaycastOrigins();
        _walkStepIndex = Mathf.RoundToInt(_raycastController.verticalRayCount * walkStepRayPercentage);
        
        _collisionInfo.Reset(moveDelta);

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

    protected void _horizontalCollisions(ref Vector2 moveDelta)
    {
        float directionX = _collisionInfo.faceDir;
        float rayLength = Mathf.Abs(moveDelta.x) + _raycastController.skinWidth;

        Vector2 firstRayOrigin = (directionX == -1) ? _raycastOrigins.bottomLeft : _raycastOrigins.bottomRight;
        Vector2 antiRayOrigin = (directionX == -1) ? _raycastOrigins.bottomRight : _raycastOrigins.bottomLeft;
        
        if (Mathf.Abs(moveDelta.x) < _raycastController.skinWidth)
        {
            rayLength = 2 * _raycastController.skinWidth;
        }

        _walkStepHeight =  (_raycastController.horizontalRaySpacing * _walkStepIndex);
        
        for (int i = 0; i < _raycastController.horizontalRayCount; ++i)
        {
            Vector2 rayOrigin = firstRayOrigin;
            rayOrigin += Vector2.up * (_raycastController.horizontalRaySpacing * i);
            
            Debug.DrawRay(rayOrigin, Vector2.right * directionX, Color.cyan);
            
            int hitCount = Physics2D.RaycastNonAlloc(rayOrigin, Vector2.right * directionX, _raycastHits, rayLength, collisionMask);
            // Potential future bug for not looping through all hitCount
            if (hitCount > 0)
            {
                RaycastHit2D hit = _raycastHits[0];
                if (Mathf.Abs(hit.distance) < 0.00001f)
                {
                    continue;
                }

                if (hit.collider.CompareTag("Platform"))
                {
                    continue;
                }

                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
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

                if (_collisionInfo.belowOld && 
                    i <= _walkStepIndex && 
                    !_collisionInfo.climbingSlope && 
                    !_collisionInfo.decendingSlope)
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

                        _collisionInfo.left = directionX < 0;
                        _collisionInfo.right = directionX > 0;
                        
                        Vector2 antiRayPos = antiRayOrigin + (Vector2.up * _raycastController.horizontalRaySpacing * i);
                        hitCount = Physics2D.RaycastNonAlloc(antiRayPos, Vector2.right * -directionX, _raycastHits, rayLength, collisionMask);
                        if(hitCount > 0)
                        {
                            _collisionInfo.crushed = true;
                        }
                    }
                }
            }
        }
    }
    
    protected void _verticalCollisions(ref Vector2 moveDelta)
    {
        float directionY = Mathf.Sign(moveDelta.y);

        bool isOnSlope = (_collisionInfo.climbingSlope || _collisionInfo.decendingSlope);
        float walkStepHeight = directionY == 1 || isOnSlope ? 0 : _walkStepHeight;
        float rayLength = Mathf.Abs(moveDelta.y) + _raycastController.skinHeight + walkStepHeight;
        
        int inputDirY =  _input.verticalMovement == 0 ?  0 : _input.verticalMovement < 0 ? -1 : 1;

        for (int i = 0; i < _raycastController.verticalRayCount; ++i)
        {
            Vector2 rayOrigin = (directionY == -1) ? _raycastOrigins.bottomLeft : _raycastOrigins.topLeft;
            Vector2 antiRayOrigin = (directionY == -1) ? _raycastOrigins.topLeft : _raycastOrigins.bottomLeft;
            
            rayOrigin.y = rayOrigin.y + walkStepHeight;
            rayOrigin += Vector2.right * (_raycastController.verticalRaySpacing * i + moveDelta.x);
            
            Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);
            
            int hitCount = Physics2D.RaycastNonAlloc(rayOrigin, Vector2.up * directionY, _raycastHits, rayLength, collisionMask);

            float smallestDist = rayLength;
            RaycastHit2D activeHit = default;
            int activeIndex = -1;
            for(int x = 0; x < hitCount; ++x)
            {
                RaycastHit2D hit = _raycastHits[x];

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
                
                _collisionInfo.below = directionY < 0;
                _collisionInfo.above = directionY > 0;

                if(!activeHit.collider.CompareTag("Platform"))
                {
                    Vector2 crushedOrigin = antiRayOrigin +
                                            (Vector2.right * (_raycastController.verticalRaySpacing * i + moveDelta.x));
                    hitCount = Physics2D.RaycastNonAlloc(crushedOrigin, Vector2.up * -directionY, _raycastHits,
                                                         rayLength, collisionMask);
                    if(hitCount > 0)
                    {
                        _collisionInfo.crushed = true;
                    }
                }
            }
        }

        if (_collisionInfo.climbingSlope)
        {
            float directionX = Mathf.Sign(moveDelta.x);
            rayLength = Mathf.Abs(moveDelta.x) + _raycastController.skinWidth;
            Vector2 rayOrigin = ((directionX == -1) ? _raycastOrigins.bottomLeft : _raycastOrigins.bottomRight) + Vector2.up * moveDelta.y;
            
            int hitCount = Physics2D.RaycastNonAlloc(rayOrigin, Vector2.right * directionX, _raycastHits, rayLength, collisionMask);
            if (hitCount > 0)
            {
                RaycastHit2D hit = _raycastHits[0];
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != _collisionInfo.slopeAngle)
                {
                    moveDelta.x = (hit.distance - _raycastController.skinWidth) * directionX;
                    _collisionInfo.slopeAngle = slopeAngle;
                    _collisionInfo.slopeNormal = hit.normal;
                }
            }
        }
    }

    private void _climbSlope(ref Vector2 moveDelta, float slopeAngle, Vector2 slopeNormal)
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

    private void _decsendSlope(ref Vector2 moveDelta)
    {
        bool isLeftHit     = Physics2D.RaycastNonAlloc(_raycastOrigins.bottomLeft, Vector2.down, _leftSlopeCastHits, Mathf.Abs(moveDelta.y) + _raycastController.skinHeight, collisionMask) > 0;
        bool isRightHit    = Physics2D.RaycastNonAlloc(_raycastOrigins.bottomRight, Vector2.down, _rightSlopeCastHits, Mathf.Abs(moveDelta.y) + _raycastController.skinHeight, collisionMask) > 0;

        if(isLeftHit ^ isRightHit)
        {
            if (isLeftHit)
            {
                _slideDownMaxSlope(ref moveDelta, _leftSlopeCastHits[0]);
            }

            if (isRightHit)
            {
                _slideDownMaxSlope(ref moveDelta, _rightSlopeCastHits[0]);
            }
        }

        if (!_collisionInfo.slidingDownMaxSlope)
        {
            float directionX = Mathf.Sign(moveDelta.x);
            Vector2 rayOrigin = (directionX == -1) ? _raycastOrigins.bottomRight :  _raycastOrigins.bottomLeft;

            int hitCount = Physics2D.RaycastNonAlloc(rayOrigin, Vector2.down, _otherHits, Mathf.Infinity, collisionMask);
            if (hitCount > 0)
            {
                RaycastHit2D hit = _otherHits[0];
                
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
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

    private void _slideDownMaxSlope(ref Vector2 moveDelta, RaycastHit2D hit)
    {
        float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
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

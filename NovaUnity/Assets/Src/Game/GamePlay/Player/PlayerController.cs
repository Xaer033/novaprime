using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

public class PlayerController
{

//    struct RaycastOrigins
//    {
//        public Vector3 topLeft;
//        public Vector3 topRight;
//        public Vector3 bottomLeft;
//        public Vector3 bottomRight;
//    }

    public struct CollisionInfo
    {
        public bool above;
        public bool below;
        public bool right;
        public bool left;
        public bool climbingSlope;
        public bool decendingSlope;
        public float slopeAngle;
        public float slopeAngleOld;
        public Vector3 velocityOld;
        public int faceDir;
        

        public void Reset(Vector3 oldVelocity)
        {
            above = below = right = left = climbingSlope = decendingSlope = false;
            slopeAngleOld = slopeAngle;
            slopeAngle = 0;
            velocityOld = oldVelocity;
        }
    }

    
    private PlayerAvatarView _view; 
    
    private Vector3 _velocity;
    private float _timeToWallUnstick;
    private Vector3 _fixedPosition;
    private Vector3 _fixedPositionOld;
    private float _velocityXSmoothing;
    private FrameInput _lastInput;

//    private float _horizontalRaySpacing;
//    private float _verticalRaySpacing;

    private RaycastController _raycastController;
//    private RaycastOrigins _raycastOrigins;
    public CollisionInfo _collisionInfo;
    
    // Start is called before the first frame update
    public void Start(PlayerAvatarView view)
    {
        _view = view;
        _view.controller = this;
        
        _raycastController = new RaycastController(
            _view.horizontalRayCount, 
            _view.verticalRayCount,
            _view.collider, 
            _view.collisionMask);

        _collisionInfo.faceDir = 1;
    }

    public Vector3 position
    {
        get
        {
            return _view.transform.position;
        }
    }
    
    public Quaternion rotation
    {
        get
        {
            return _view.transform.rotation;
        }
    }
    
    // Update is called once per frame
    public void Step(float deltaTime, FrameInput input)
    {
        _lastInput = input;
        
//        if (_view && _view.transform != null)
//        {
//            Vector3 thing = Vector3.zero;
//            _view.transform.localPosition = Vector3.Lerp(_fixedPositionOld, _fixedPosition, deltaTime);
//        }
    }
    
    public void FixedStep(float deltaTime)
    {
        if (_view)
        {
            float targetVelocityX  = _lastInput.horizontalMovement * _view.speed;
            
            float accelerationTime = _collisionInfo.below ? _view.accelerationTimeGround : _view.accelerationTimeAir;
            _velocity.x = Mathf.SmoothDamp(_velocity.x, targetVelocityX, ref _velocityXSmoothing, accelerationTime);
            
            int wallDirX = _collisionInfo.left ? -1 : 1;
            int inputDirX = _lastInput.horizontalMovement == 0 ?  0 : _lastInput.horizontalMovement < 0 ? -1 : 1;
            
            bool wallSliding = false;
            bool wallOnSide = (_collisionInfo.left || _collisionInfo.right);
            if ( wallOnSide && !_collisionInfo.below && _velocity.y < 0 )
            {
                wallSliding = true;
                if (_velocity.y < -_view.wallSlideSpeedMax)
                {
                    _velocity.y = -_view.wallSlideSpeedMax;
                }

                if (_timeToWallUnstick > 0)
                {
                    _velocityXSmoothing = 0;
                    _velocity.x = 0;
                    
                    if (inputDirX != wallDirX && inputDirX != 0)
                    {
                        _timeToWallUnstick -= deltaTime;
                    }
                    else
                    {
                        _timeToWallUnstick = _view.wallStickTime;
                    }
                }
                else
                {
                    _timeToWallUnstick = _view.wallStickTime;
                }
            }
            
            if (_collisionInfo.above || _collisionInfo.below)
            {
                _velocity.y = 0;
            }

            float timeToJumpApex = _view.timeToJumpApex;
            float gravity = -(2 * _view.jumpHeight) / (timeToJumpApex * timeToJumpApex);
            float jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
            
            if (_lastInput.jump)
            {
                if (wallSliding)
                {
                    if (wallDirX == inputDirX)
                    {
                        _velocity.x = -wallDirX * _view.wallJumpClimb.x;
                        _velocity.y =  _view.wallJumpClimb.y;
                    }
                    else if (inputDirX == 0)
                    {
                        _velocity.x = -wallDirX * _view.wallJumpOff.x;
                        _velocity.y = _view.wallJumpOff.y;
                    }
                    else
                    {
                        _velocity.x = -wallDirX * _view.wallJumpLeap.x;
                        _velocity.y = _view.wallJumpLeap.y;
                    }
                }
                else if (_collisionInfo.below)
                {
                    _velocity.y = jumpVelocity;
                }
            }
            

            _velocity.y += gravity * deltaTime;
            
            _move(_velocity * deltaTime, false);

        }
    }

    private Vector3 _verticalCollisions(Vector3 currentVelocity)
    {
        Vector3 newVelocity = currentVelocity;
        float directionY = Mathf.Sign(currentVelocity.y);
        float rayLength = Mathf.Abs(currentVelocity.y) + _raycastController.skinWidth;
        RaycastHit hit;
        
        for (int i = 0; i < _view.verticalRayCount; ++i)
        {
            Vector3 rayOrigin = (directionY == -1) ? _raycastOrigins.bottomLeft : _raycastOrigins.topLeft;
            rayOrigin += Vector3.right * (_raycastController.verticalRaySpacing * i + currentVelocity.x);
            if (Physics.Raycast(rayOrigin, Vector3.up * directionY, out hit, rayLength, _view.collisionMask))
            {
                newVelocity.y = (hit.distance - _raycastController.skinWidth) * directionY;
                rayLength = hit.distance;

                if (_collisionInfo.climbingSlope)
                {
                    newVelocity.x = newVelocity.y / Mathf.Tan(_collisionInfo.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(newVelocity.x);
                }
                _collisionInfo.below = directionY == -1;
                _collisionInfo.above = directionY == 1;
            }
            
            Debug.DrawRay(rayOrigin, Vector3.up * directionY * rayLength, Color.red);
        }

        if (_collisionInfo.climbingSlope)
        {
            float directionX = Mathf.Sign(newVelocity.x);
            rayLength = Mathf.Abs(newVelocity.x) + _raycastController.skinWidth;
            Vector3 rayOrigin = ((directionX == -1) ? _raycastOrigins.bottomLeft : _raycastOrigins.bottomRight) + Vector3.up * newVelocity.y;
            
            if(Physics.Raycast(rayOrigin, Vector3.right * directionX, out hit, rayLength, _view.collisionMask))
            {
                float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
                if (slopeAngle != _collisionInfo.slopeAngle)
                {
                    _velocity.x = (hit.distance - _raycastController.skinWidth) * directionX;
                    _collisionInfo.slopeAngle = slopeAngle;
                }
            }
        }
        return newVelocity;
    }
    
    private Vector3 _horizontalCollisions(Vector3 currentVelocity)
    {
        Vector3 newVelocity = currentVelocity;
        float directionX = _collisionInfo.faceDir;
        float rayLength = Mathf.Abs(currentVelocity.x) + _raycastController.skinWidth;

        if (Mathf.Abs(currentVelocity.x) < _raycastController.skinWidth)
        {
            rayLength = 2 * _raycastController.skinWidth;
        }
        
        RaycastHit hit;
        
        for (int i = 0; i < _view.horizontalRayCount; ++i)
        {
            Vector3 rayOrigin = (directionX == -1) ? _raycastOrigins.bottomLeft : _raycastOrigins.bottomRight;
            rayOrigin += Vector3.up * (_raycastController.horizontalRaySpacing * i);
            if (Physics.Raycast(rayOrigin, Vector3.right * directionX, out hit, rayLength, _view.collisionMask))
            {
                if (hit.distance == 0)
                {
                    continue;
                }
                
                float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
                if (i == 0 && slopeAngle <= _view.maxClimbAngle)
                {
                    if (_collisionInfo.decendingSlope)
                    {
                        _collisionInfo.decendingSlope = false;
                        newVelocity = _collisionInfo.velocityOld;
                    }
                    float distanceToSlopeStart = 0;
                    if (slopeAngle != _collisionInfo.slopeAngleOld)
                    {
                        distanceToSlopeStart = hit.distance - _raycastController.skinWidth;
                        newVelocity.x -= distanceToSlopeStart * directionX;
                    }

                    newVelocity = _climbSlope(currentVelocity, slopeAngle);
                    newVelocity.x += distanceToSlopeStart * directionX;
                }

                if (!_collisionInfo.climbingSlope || slopeAngle > _view.maxClimbAngle)
                {
                    newVelocity.x = (hit.distance - _raycastController.skinWidth) * directionX;
                    rayLength = hit.distance;

                    if (_collisionInfo.climbingSlope)
                    {
                        newVelocity.y = Mathf.Tan(_collisionInfo.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(newVelocity.x);
                    }
                    _collisionInfo.left = directionX == -1;
                    _collisionInfo.right = directionX == 1;
                }
            }
            Debug.DrawRay(rayOrigin, Vector3.right * directionX * rayLength, Color.red);

        }

        return newVelocity;
    }

    private Vector3 _climbSlope(Vector3 velocity, float slopeAngle)
    {
        Vector3 newVelocity = velocity;
        float moveDistance = Mathf.Abs(velocity.x);
        float climbVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
        
        newVelocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
        
        if (velocity.y <= climbVelocityY)
        {
            newVelocity.y = climbVelocityY;
            _collisionInfo.below = true;
            _collisionInfo.climbingSlope = true;
            _collisionInfo.slopeAngle = slopeAngle;
        }
        
        return newVelocity;

    }

    private RaycastController.RaycastOrigins _raycastOrigins
    {
        get { return _raycastController.origins; }
    }

    private Vector3 _decendSlope(Vector3 velocity)
    {
        Vector3 newVelocity = velocity;
        float directionX = Mathf.Sign(velocity.x);
        RaycastHit hit;
        Vector3 rayOrigin = (directionX == -1) ? _raycastOrigins.bottomRight :  _raycastOrigins.bottomLeft;
        
        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, Mathf.Infinity, _view.collisionMask))
        {
            float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
            if (Mathf.Abs(slopeAngle) > 0.0001f && slopeAngle <= _view.maxDecendAngle)
            {
                if (Mathf.Sign(hit.normal.x) == directionX)
                {
                    if (hit.distance - _raycastController.skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x))
                    {
                        float moveDistance = Mathf.Abs(velocity.x);
                        float decendVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                        newVelocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);;
                        newVelocity.y -= decendVelocityY;

                        _collisionInfo.slopeAngle = slopeAngle;
                        _collisionInfo.decendingSlope = true;
                        _collisionInfo.below = true;
                    }
                }
            }
        }

        return newVelocity;
    }

    public void Move(Vector3 velocity, bool isOnPlatform)
    {
        _move(velocity, isOnPlatform);
    }
    
    private void _move(Vector3 velocity, bool isOnPlatform)
    {
        _raycastController.Step();
        _collisionInfo.Reset(velocity);

        if (velocity.x != 0)
        {
            _collisionInfo.faceDir = (int)Mathf.Sign(velocity.x);
        }
        
        if (velocity.y < 0)
        {
            velocity = _decendSlope(velocity);
        }
        
            velocity = _horizontalCollisions(velocity);
        

        if (Mathf.Abs(velocity.y) > 0.0001f)
        {
            velocity = _verticalCollisions(velocity);
        }
        
        _view.transform.Translate(velocity);
        
        if (isOnPlatform)
        {
            _collisionInfo.below = true;
        }
        
    }
}

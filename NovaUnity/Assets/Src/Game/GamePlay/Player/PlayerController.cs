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

        public bool fallingThroughPlatform;

        public void Reset(Vector3 oldVelocity)
        {
            above = below = right = left = climbingSlope = decendingSlope = false;
            slopeAngleOld = slopeAngle;
            slopeAngle = 0;
            velocityOld = oldVelocity;
        }
    }

    private const float kFallThroughPlatformWaitDuration = 0.5f;

    private PlayerAvatarView _view;

    private Vector3 _velocity;
    private float _timeToWallUnstick;
    private bool _isWallSliding;
    private Vector3 _fixedPosition;
    private Vector3 _fixedPositionOld;
    private float _velocityXSmoothing;
    private FrameInput _lastInput;

    private PlayerInput _input;
    private float _resetPlatformTime;

    private RaycastController _raycastController;

//    private RaycastOrigins _raycastOrigins;
    private CollisionInfo _collisionInfo;

    public PlayerController(PlayerAvatarView view, PlayerInput input)
    {
        _view = view;
        _view.controller = this;

        _input = input;

        _raycastController = new RaycastController(
            _view.horizontalRayCount,
            _view.verticalRayCount,
            _view.collider,
            _view.collisionMask);

        _collisionInfo.faceDir = 1;
    }

    // Start is called before the first frame update
    public void Start()
    {

    }

    public Vector3 position
    {
        get { return _view.transform.position; }
    }

    public Quaternion rotation
    {
        get { return _view.transform.rotation; }
    }

    public FrameInput lastInput
    {
        get { return _lastInput; }
    }

    // Update is called once per frame
    public void Step(float deltaTime)
    {
        _lastInput = _input.GetInput();

    }

    public void FixedStep(float deltaTime)
    {
        if (_view)
        {
            int wallDirX = _collisionInfo.left ? -1 : 1;
            int inputDirX = _lastInput.horizontalMovement == 0 ? 0 : _lastInput.horizontalMovement < 0 ? -1 : 1;
            
            _handlePlatformFallThrough(deltaTime);
            _handleDirectionMovement(ref _velocity);
            _handleWallSliding(ref _velocity, deltaTime, wallDirX, inputDirX);
            _handleJumping(ref _velocity, deltaTime, wallDirX, inputDirX);

            // Apply constraints to velocity and move avatar, no modifications to _velocity here
            _move(_velocity * deltaTime, false);

            // *Bop*
            if (_collisionInfo.above || _collisionInfo.below)
            {
                _velocity.y = 0;
            }
        }
        
        // Clear buffered input as we handled it above (for transient button up/down events that may get missed in a fixedUpdate
        _input.Clear();
    }

    private void _handlePlatformFallThrough(float deltaTime)
    {
        if (_collisionInfo.fallingThroughPlatform && _resetPlatformTime > 0)
        {
            _resetPlatformTime -= deltaTime;
            if (_resetPlatformTime <= 0.0f)
            {
                _collisionInfo.fallingThroughPlatform = false;
            }
        }
    }
    
    private void _handleDirectionMovement(ref Vector3 velocity)
    {
        float targetVelocityX = _lastInput.horizontalMovement * _view.speed;
        float accelerationTime = _collisionInfo.below ? _view.accelerationTimeGround : _view.accelerationTimeAir;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref _velocityXSmoothing, accelerationTime);
    }
    
    private void _handleJumping(ref Vector3 velocity, float deltaTime, int wallDirX, int inputDirX)
    {
        float timeToJumpApex = _view.timeToJumpApex;
        float gravity = -(2 * _view.maxJumpHeight) / (timeToJumpApex * timeToJumpApex);
        float maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        float minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs((gravity) * _view.minJumpHeight));

        if (_lastInput.jumpPressed)
        {
            if (_isWallSliding)
            {
                if (wallDirX == inputDirX)
                {
                    velocity.x = -wallDirX * _view.wallJumpClimb.x;
                    velocity.y = _view.wallJumpClimb.y;
                }
                else if (inputDirX == 0)
                {
                    velocity.x = -wallDirX * _view.wallJumpOff.x;
                    velocity.y = _view.wallJumpOff.y;
                }
                else
                {
                    velocity.x = -wallDirX * _view.wallJumpLeap.x;
                    velocity.y = _view.wallJumpLeap.y;
                }
            }
            else if (_collisionInfo.below)
            {
                velocity.y = maxJumpVelocity;
            }
        }

        if (_lastInput.jumpReleased)
        {
            if (velocity.y > minJumpVelocity)
            {
                velocity.y = minJumpVelocity;
            }
        }

        velocity.y += gravity * deltaTime;
    }


    private void _handleWallSliding(ref Vector3 velocity, float deltaTime, int wallDirX, int inputDirX)
    {
        _isWallSliding = false;
        bool wallOnSide = (_collisionInfo.left || _collisionInfo.right);
        if (wallOnSide && !_collisionInfo.below && velocity.y < 0)
        {
            _isWallSliding = true;
            if (velocity.y < -_view.wallSlideSpeedMax)
            {
                velocity.y = -_view.wallSlideSpeedMax;
            }

            if (_timeToWallUnstick > 0)
            {
                _velocityXSmoothing = 0;
                velocity.x = 0;

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
    }

    private Vector3 _horizontalCollisions(Vector3 moveDelta)
    {
        Vector3 newMoveDelta = moveDelta;
        float directionX = _collisionInfo.faceDir;
        float rayLength = Mathf.Abs(moveDelta.x) + _raycastController.skinWidth;

        if (Mathf.Abs(moveDelta.x) < _raycastController.skinWidth)
        {
            rayLength = 2 * _raycastController.skinWidth;
        }
        
        RaycastHit hit;
        
        for (int i = 0; i < _view.horizontalRayCount; ++i)
        {
            Vector3 rayOrigin = (directionX == -1) ? _raycastOrigins.bottomLeft : _raycastOrigins.bottomRight;
            rayOrigin += Vector3.up * (_raycastController.horizontalRaySpacing * i);
            
            Debug.DrawRay(rayOrigin, Vector3.right * directionX, Color.red);
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
                        newMoveDelta = _collisionInfo.velocityOld;
                    }
                    float distanceToSlopeStart = 0;
                    if (slopeAngle != _collisionInfo.slopeAngleOld)
                    {
                        distanceToSlopeStart = hit.distance - _raycastController.skinWidth;
                        newMoveDelta.x -= distanceToSlopeStart * directionX;
                    }

                    newMoveDelta = _climbSlope(moveDelta, slopeAngle);
                    newMoveDelta.x += distanceToSlopeStart * directionX;
                }

                if (!_collisionInfo.climbingSlope || slopeAngle > _view.maxClimbAngle)
                {
                    newMoveDelta.x = (hit.distance - _raycastController.skinWidth) * directionX;
                    rayLength = hit.distance;

                    if (_collisionInfo.climbingSlope)
                    {
                        newMoveDelta.y = Mathf.Tan(_collisionInfo.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(newMoveDelta.x);
                    }
                    _collisionInfo.left = directionX == -1;
                    _collisionInfo.right = directionX == 1;
                }
            }
        }

        return newMoveDelta;
    }
    
    private Vector3 _verticalCollisions(Vector3 moveDelta)
    {
        Vector3 newMoveDelta = moveDelta;
        float directionY = Mathf.Sign(moveDelta.y);
        float rayLength = Mathf.Abs(moveDelta.y) + _raycastController.skinWidth;
        
        RaycastHit hit;
        
        int inputDirY =  _lastInput.verticalMovement == 0 ?  0 : _lastInput.verticalMovement < 0 ? -1 : 1;

        for (int i = 0; i < _view.verticalRayCount; ++i)
        {
            Vector3 rayOrigin = (directionY == -1) ? _raycastOrigins.bottomLeft : _raycastOrigins.topLeft;
            rayOrigin += Vector3.right * (_raycastController.verticalRaySpacing * i + moveDelta.x);
            
            Debug.DrawRay(rayOrigin, Vector3.up * directionY, Color.red);
            if (Physics.Raycast(rayOrigin, Vector3.up * directionY, out hit, rayLength, _view.collisionMask))
            {
                if (hit.collider.tag == "Through")
                {
                    if (directionY == 1 || hit.distance == 0)
                    {
                        continue;
                    }

                    if (_collisionInfo.fallingThroughPlatform)
                    {
                        continue;
                    }
                    
                    if (inputDirY == -1)
                    {
                        _startFallThroughPlatformTimer();
                        continue;
                    }
                }
                newMoveDelta.y = (hit.distance - _raycastController.skinWidth) * directionY;
                rayLength = hit.distance;

                if (_collisionInfo.climbingSlope)
                {
                    newMoveDelta.x = newMoveDelta.y / Mathf.Tan(_collisionInfo.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(newMoveDelta.x);
                }
                _collisionInfo.below = directionY == -1;
                _collisionInfo.above = directionY == 1;
            }
            
        }

        if (_collisionInfo.climbingSlope)
        {
            float directionX = Mathf.Sign(newMoveDelta.x);
            rayLength = Mathf.Abs(newMoveDelta.x) + _raycastController.skinWidth;
            Vector3 rayOrigin = ((directionX == -1) ? _raycastOrigins.bottomLeft : _raycastOrigins.bottomRight) + Vector3.up * newMoveDelta.y;
            
            if(Physics.Raycast(rayOrigin, Vector3.right * directionX, out hit, rayLength, _view.collisionMask))
            {
                float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
                if (slopeAngle != _collisionInfo.slopeAngle)
                {
                    newMoveDelta.x = (hit.distance - _raycastController.skinWidth) * directionX;
                    _collisionInfo.slopeAngle = slopeAngle;
                }
            }
        }
        return newMoveDelta;
    }

    private Vector3 _climbSlope(Vector3 moveDelta, float slopeAngle)
    {
        Vector3 newMoveDelta = moveDelta;
        float moveDistance = Mathf.Abs(moveDelta.x);
        float climbVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
        
        newMoveDelta.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveDelta.x);
        
        if (moveDelta.y <= climbVelocityY)
        {
            newMoveDelta.y = climbVelocityY;
            _collisionInfo.below = true;
            _collisionInfo.climbingSlope = true;
            _collisionInfo.slopeAngle = slopeAngle;
        }
        
        return newMoveDelta;

    }

    private RaycastController.RaycastOrigins _raycastOrigins
    {
        get { return _raycastController.origins; }
    }

    private Vector3 _decendSlope(Vector3 moveDelta)
    {
        Vector3 newMoveDelta = moveDelta;
        float directionX = Mathf.Sign(moveDelta.x);
        RaycastHit hit;
        Vector3 rayOrigin = (directionX == -1) ? _raycastOrigins.bottomRight :  _raycastOrigins.bottomLeft;
        
        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, Mathf.Infinity, _view.collisionMask))
        {
            float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
            if (Mathf.Abs(slopeAngle) > 0.0001f && slopeAngle <= _view.maxDecendAngle)
            {
                if (Mathf.Sign(hit.normal.x) == directionX)
                {
                    if (hit.distance - _raycastController.skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveDelta.x))
                    {
                        float moveDistance = Mathf.Abs(moveDelta.x);
                        float decendVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                        newMoveDelta.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveDelta.x);;
                        newMoveDelta.y -= decendVelocityY;

                        _collisionInfo.slopeAngle = slopeAngle;
                        _collisionInfo.decendingSlope = true;
                        _collisionInfo.below = true;
                    }
                }
            }
        }

        return newMoveDelta;
    }

    public void Move(Vector3 moveDelta, bool isOnPlatform)
    {
        _move(moveDelta, isOnPlatform);
    }
    
    private void _move(Vector3 moveDelta, bool isOnPlatform)
    {
        _raycastController.UpdateRaycastOrigins();
        _collisionInfo.Reset(moveDelta);

        if (moveDelta.x != 0)
        {
            _collisionInfo.faceDir = (int)Mathf.Sign(moveDelta.x);
        }
        
        if (moveDelta.y < 0)
        {
            moveDelta = _decendSlope(moveDelta);
        }
        
        moveDelta = _horizontalCollisions(moveDelta);
        

        if (Mathf.Abs(moveDelta.y) > 0.0001f)
        {
            moveDelta = _verticalCollisions(moveDelta);
        }
        
//        _view.transform.Translate(moveDelta);
        _view.transform.localPosition = _view.transform.localPosition + moveDelta;
        if (isOnPlatform)
        {
            _collisionInfo.below = true;
        }
    }

    private void _startFallThroughPlatformTimer()
    {
        _collisionInfo.fallingThroughPlatform = true;
        _resetPlatformTime = kFallThroughPlatformWaitDuration;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

public class PlayerController
{

    private const float kFallThroughPlatformWaitDuration = 0.4f;

    private PlayerAvatarView _view;

    private float _gravity;
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
    private CollisionInfo _collisionInfo;

    public PlayerController(PlayerAvatarView view, PlayerInput input)
    {
        _view = view;
        _view.controller = this;

        _input = input;

        _raycastController = new RaycastController(
            _view.distanceBetweenRays,
            _view.collider,
            _view.collisionMask);

    }

    // Start is called before the first frame update
    public void Start()
    {
        _collisionInfo.faceDir = 1;
        
        float timeToJumpApex = _view.timeToJumpApex;
        _gravity = -(2 * _view.maxJumpHeight) / (timeToJumpApex * timeToJumpApex);
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
    public void FixedStep(float deltaTime)
    {
//        _lastInput = _input.GetInput();
    }

    public void Step(float deltaTime)
    {
        _lastInput = _input.GetInput();
        if (_view)
        {
            int wallDirX = _collisionInfo.left ? -1 : 1;
            int inputDirX = _lastInput.horizontalMovement == 0 ? 0 : _lastInput.horizontalMovement < 0 ? -1 : 1;
            
            _handlePlatformFallThrough(deltaTime);
            _handleDirectionMovement(ref _velocity, deltaTime);

            bool isWallSliding = _handleWallSliding(ref _velocity, deltaTime, wallDirX, inputDirX);
            _handleJumping(ref _velocity, isWallSliding, wallDirX, inputDirX);

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
    
    private void _handleDirectionMovement(ref Vector3 velocity, float deltaTime)
    {
        float targetVelocityX = _lastInput.horizontalMovement * _view.speed;
        float accelerationTime = _collisionInfo.below ? _view.accelerationTimeGround : _view.accelerationTimeAir;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref _velocityXSmoothing, accelerationTime);
        velocity.y += _gravity * deltaTime;
    }
    
    private void _handleJumping(ref Vector3 velocity, bool isWallSliding, int wallDirX, int inputDirX)
    {
        float timeToJumpApex = _view.timeToJumpApex;
        _gravity = -(2 * _view.maxJumpHeight) / (timeToJumpApex * timeToJumpApex);
        float maxJumpVelocity = Mathf.Abs(_gravity) * timeToJumpApex;
        float minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(_gravity) * _view.minJumpHeight);

        if (_lastInput.jumpPressed)
        {
            
            Debug.Log("JumpPressed");
            if (isWallSliding)
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
            
            if (_collisionInfo.below)
            {
                velocity.y = maxJumpVelocity;
            }
        }

        if (_lastInput.jumpReleased)
        {
            
            Debug.Log("JumpReleased");
            if (velocity.y > minJumpVelocity)
            {
                velocity.y = minJumpVelocity;
            }
        }
    }


    private bool _handleWallSliding(ref Vector3 velocity, float deltaTime, int wallDirX, int inputDirX)
    {
        bool isWallSliding = false;
        bool wallOnSide = (_collisionInfo.left || _collisionInfo.right);
        if (wallOnSide && !_collisionInfo.below && velocity.y < 0)
        {
            isWallSliding = true;
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

        return isWallSliding;
    }

    private void _horizontalCollisions(ref Vector3 moveDelta)
    {
        float directionX = _collisionInfo.faceDir;
        float rayLength = Mathf.Abs(moveDelta.x) + _raycastController.skinWidth;

        if (Mathf.Abs(moveDelta.x) < _raycastController.skinWidth)
        {
            rayLength = 2 * _raycastController.skinWidth;
        }
        
        RaycastHit hit;
        
        for (int i = 0; i < _raycastController.horizontalRayCount; ++i)
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
                        moveDelta = _collisionInfo.oldMoveDelta;
                    }
                    float distanceToSlopeStart = 0;
                    if (slopeAngle != _collisionInfo.slopeAngleOld)
                    {
                        distanceToSlopeStart = hit.distance - _raycastController.skinWidth;
                        moveDelta.x -= distanceToSlopeStart * directionX;
                    }

                    moveDelta = _climbSlope(moveDelta, slopeAngle);
                    moveDelta.x += distanceToSlopeStart * directionX;
                }

                if (!_collisionInfo.climbingSlope || slopeAngle > _view.maxClimbAngle)
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
    
    private void _verticalCollisions(ref Vector3 moveDelta, bool isOnPlatform)
    {
        float directionY = Mathf.Sign(moveDelta.y);
        float rayLength = Mathf.Abs(moveDelta.y) + _raycastController.skinWidth;
        
        int inputDirY =  _lastInput.verticalMovement == 0 ?  0 : _lastInput.verticalMovement < 0 ? -1 : 1;

        for (int i = 0; i < _raycastController.verticalRayCount; ++i)
        {
            Vector3 rayOrigin = (directionY == -1) ? _raycastOrigins.bottomLeft : _raycastOrigins.topLeft;
            rayOrigin += Vector3.right * (_raycastController.verticalRaySpacing * i + moveDelta.x);
            
            Debug.DrawRay(rayOrigin, Vector3.up * directionY, Color.red);
            
            RaycastHit hit;
            bool isHit = Physics.Raycast(rayOrigin, Vector3.up * directionY, out hit, rayLength, _view.collisionMask);
            if (isHit)
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
                
                moveDelta.y = (hit.distance - _raycastController.skinWidth) * directionY;
                rayLength = hit.distance;

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
            
            RaycastHit hit;
            bool isHit = Physics.Raycast(rayOrigin, Vector3.right * directionX, out hit, rayLength, _view.collisionMask);
            if(isHit)
            {
                float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
                if (slopeAngle != _collisionInfo.slopeAngle)
                {
                    moveDelta.x = (hit.distance - _raycastController.skinWidth) * directionX;
                    _collisionInfo.slopeAngle = slopeAngle;
                }
            }
        }
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

    private void _decendSlope(ref Vector3 moveDelta)
    {
        float directionX = Mathf.Sign(moveDelta.x);
        RaycastHit hit;
        Vector3 rayOrigin = (directionX == -1) ? _raycastOrigins.bottomRight :  _raycastOrigins.bottomLeft;
        
        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, Mathf.Infinity, _view.collisionMask))
        {
            float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
            if (slopeAngle != 0 && slopeAngle <= _view.maxDecendAngle)
            {
                if (Mathf.Sign(hit.normal.x) == directionX)
                {
                    if (hit.distance - _raycastController.skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveDelta.x))
                    {
                        float moveDistance = Mathf.Abs(moveDelta.x);
                        float decendVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                        moveDelta.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveDelta.x);
                        moveDelta.y -= decendVelocityY;

                        _collisionInfo.slopeAngle = slopeAngle;
                        _collisionInfo.decendingSlope = true;
                        _collisionInfo.below = true;
                    }
                }
            }
        }
    }

    public void Move(Vector3 moveDelta, bool isOnPlatform)
    {
        _move(moveDelta, isOnPlatform);
    }
    
    private void _move(Vector3 moveDelta, bool isOnPlatform)
    {
        _raycastController.distanceBetweenRays = _view.distanceBetweenRays;
        _raycastController.UpdateRaycastOrigins();
        _collisionInfo.Reset();
        _collisionInfo.oldMoveDelta = moveDelta;

           
        if (moveDelta.x != 0)
        {
            _collisionInfo.faceDir = (int)Mathf.Sign(moveDelta.x);
        }
        
        if (moveDelta.y < 0)
        {
            _decendSlope(ref moveDelta);
        }
        
        _horizontalCollisions(ref moveDelta);
        _verticalCollisions(ref moveDelta, isOnPlatform);
        
    
        _view.transform.Translate(moveDelta);
//        _view.transform.localPosition = _view.transform.localPosition + moveDelta;

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
    
    struct CollisionInfo
    {
        public bool above;
        public bool below;
        public bool right;
        public bool left;
        public bool climbingSlope;
        public bool decendingSlope;
        public float slopeAngle;
        public float slopeAngleOld;
        public Vector3 oldMoveDelta;
        public int faceDir;

        public bool fallingThroughPlatform;

        public void Reset()
        {
            above = below = right = left = false;
            climbingSlope = false;
            decendingSlope = false;
            slopeAngleOld = slopeAngle;
            slopeAngle = 0;
        }
    }

}

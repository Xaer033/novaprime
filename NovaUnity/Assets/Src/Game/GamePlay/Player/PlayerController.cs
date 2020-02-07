using GhostGen;
using UnityEngine;

public class PlayerController : NotificationDispatcher
{
    private PlayerAvatarView _view;

    private float _gravity;

    private float _timeToWallUnstick;
    private float _wallSlideVelocity;
    private bool _isWallSliding;

    private float _midairJumpTimer;
    private float _coyoteJumpTimer;
    
    private Vector3 _previousPosition;
    
    private float _velocityXSmoothing;
    
    private FrameInput _lastInput;
    private PlayerInput _input;
    private int _jumpCount;
    private float _resetPlatformTime;

    private MachineGunController _machineGunController;
    private PlayerState _state;
    private GameSystems _gameSystems;
    
    public PlayerController(PlayerState state, PlayerAvatarView view, PlayerInput input)
    {
        _state = state;
        
        _view = view;
        _view.controller = this;

        _input = input;
    }

    // Start is called before the first frame update
    public void Start(GameSystems gameSystems)
    {
        _gameSystems = gameSystems;
        
        float timeToJumpApex = _view.timeToJumpApex;
        _gravity = -(2 * _view.maxJumpHeight) / (timeToJumpApex * timeToJumpApex);
        
        _machineGunController = new MachineGunController(_gameSystems, _state.machineGunState);
        _view.SetWeapon(_machineGunController.view.transform);
    }

    
    public Vector3 position
    {
        get { return _state.position; }
        set { _state.position = value; }
    }
    
    public Quaternion rotation
    {
        get { return _view.transform.rotation; }
    }

    public FrameInput lastInput
    {
        get { return _lastInput; }
    }

    
    public void Move(Vector3 moveDelta, bool isOnPlatform)
    {
        if (_view && _view.constrainer != null)
        {
            Vector3 constrainedMoveDelta = _view.constrainer.Move(moveDelta, isOnPlatform, _lastInput);

            _previousPosition = _state.position;
            _state.position = _state.position + constrainedMoveDelta;
            _view.transform.localPosition = _state.position;
        }
    }
    
    // Update is called once per frame
    public void Step(float deltaTime)
    {
        _lastInput = _input.GetInput();
        
        float alpha = (Time.time - Time.fixedTime) / Time.fixedDeltaTime;
//        _view.transform.localPosition = Vector3.Lerp(_previousPosition, _state.position, alpha);
//        _view.transform.localPosition = _state.position;
    }

    public void FixedStep(float deltaTime)
    {
        _lastInput = _input.GetInput();
        
        
        deltaTime = _lastInput.secondaryFire ? deltaTime * 0.5f : deltaTime;
        deltaTime = deltaTime * _state.timeScale;


        Vector3 aimDirection = _lastInput.cursorDirection.normalized;
        Vector3 aimPosition = _lastInput.useCusorPosition ? _lastInput.cursorPosition : _state.position + (aimDirection * 4.0f);
        Debug.DrawRay(_state.position, aimDirection, Color.cyan, 0.2f);

        const float kDebugLineSize = 0.2f;
        Debug.DrawLine(aimPosition + Vector3.down * kDebugLineSize, aimPosition + Vector3.up     * kDebugLineSize, Color.cyan, 0.2f);
        Debug.DrawLine(aimPosition + Vector3.left * kDebugLineSize, aimPosition + Vector3.right  * kDebugLineSize, Color.cyan, 0.2f);
    
        if (_view)
        {
            int wallDirX = _view.constrainer.collisionInfo.left ? -1 : 1;
            int inputDirX = Mathf.Abs(_lastInput.horizontalMovement) < 0.5f ? 0 : _lastInput.horizontalMovement < 0 ? -1 : 1;
            
            _handleDirectionMovement(ref _state.velocity, deltaTime);

            bool isWallSliding = _handleWallSliding(ref _state.velocity, deltaTime, wallDirX, inputDirX);
            _handleJumping(ref _state.velocity, deltaTime, isWallSliding, wallDirX, inputDirX);
            
            Move(_state.velocity * deltaTime, false);
            
            if (collisionInfo.below)
            {
                _coyoteJumpTimer = _view.coyoteTime;
                _jumpCount = 0;
            }
            else
            {
                if (_coyoteJumpTimer <= 0 && _jumpCount == 0 && !isWallSliding)
                {
                    _jumpCount++;
                }
            }
            
            // *Bop*
            if (collisionInfo.above || collisionInfo.below)
            {
                if (collisionInfo.slidingDownMaxSlope)
                {
                    _state.velocity.y += collisionInfo.slopeNormal.y * -_gravity * deltaTime;
                }
                else
                {
                    _state.velocity.y = 0;
                }
            }
            
            // Aim
            _view.Aim(aimPosition);
        }
        
        // Weapon Handling
        if (_machineGunController != null)
        {
            _machineGunController.FixedStep(deltaTime);

            if (_lastInput.primaryFire)
            {
                _machineGunController.Fire(aimPosition);
            }
        }

        // Clear buffered input as we handled it above (for transient button up/down events that may get missed in a fixedUpdate
        _input.Clear();

    }

    public void OnTimeWarpEnter(float timeScale)
    {
        _state.timeScale = timeScale;
    }
    
    public void OnTimeWarpExit()
    {
        _state.timeScale = 1.0f;
    }
    
    private CollisionInfo collisionInfo
    {
        get { return _view.constrainer.collisionInfo; }
    }
    

    private void _handleDirectionMovement(ref Vector3 velocity, float deltaTime)
    {
        float targetVelocityX = _lastInput.horizontalMovement * _view.speed;
        float accelerationTime = collisionInfo.below ? _view.accelerationTimeGround : _jumpCount > 1 ? _view.accelerationTimeAir * 2.5f : _view.accelerationTimeAir; 
        float inputVelocityX = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref _velocityXSmoothing, accelerationTime);

        velocity.x = inputVelocityX;
        if (velocity.y > -Mathf.Abs(_view.terminalVelocity))
        {
            velocity.y += _gravity * deltaTime;
        }
    }
    
    private void _handleJumping(ref Vector3 velocity, float deltaTime, bool isWallSliding, int wallDirX, int inputDirX)
    {
        float timeToJumpApex = _view.timeToJumpApex;
        _gravity = -(2 * _view.maxJumpHeight) / (timeToJumpApex * timeToJumpApex);
        
        float maxJumpVelocity = Mathf.Abs(_gravity) * timeToJumpApex;
        float minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(_gravity) * _view.minJumpHeight);
       
        _gravity = (_lastInput.verticalMovement < 0) ? _gravity * 1.5f : _gravity;

        if (_lastInput.jumpPressed)
        {
            _midairJumpTimer = _view.jumpRememberDelay;
        }

        bool didDoubleJump = (_jumpCount == 1 && _lastInput.jumpPressed);
        
        // This is so if the player jumps while in the air for a bit, we still jump when on floor
        if (_midairJumpTimer > 0)
        {
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

                _midairJumpTimer = 0;
            }
            
            if ( _coyoteJumpTimer > 0 || didDoubleJump)
            {
                _coyoteJumpTimer = 0;
                if (collisionInfo.slidingDownMaxSlope)
                {
                    if (inputDirX == 0 || inputDirX != -(int) Mathf.Sign(collisionInfo.slopeNormal.x))
                    {
                        Vector3 jumpDirection = (Vector3.up + collisionInfo.slopeNormal).normalized * maxJumpVelocity;
                        velocity.x = jumpDirection.x;
                        velocity.y = jumpDirection.y;
                        
                        _jumpCount++;
                    }
                }
                else 
                {
                    velocity.y = _lastInput.verticalMovement < 0 ? maxJumpVelocity * 0.75f : maxJumpVelocity;
                    if (_jumpCount > 0)
                    {
                        velocity.x = inputDirX * _view.wallJumpOff.x;
                    }
                    
                    _jumpCount++;
                }

                _midairJumpTimer = 0;
            }
        }

        if (_lastInput.jumpReleased)
        {
            
            if (velocity.y > minJumpVelocity)
            {
                velocity.y = minJumpVelocity;
            }
        }
        
        _midairJumpTimer -= deltaTime;
        _coyoteJumpTimer -= deltaTime;
    }


    private bool _handleWallSliding(ref Vector3 velocity, float deltaTime, int wallDirX, int inputDirX)
    {
        bool isWallSliding = false;
        
        bool pushingLeftWall = (collisionInfo.left && inputDirX == -1);
        bool pushingRightWall = ( collisionInfo.right && inputDirX == 1);

        bool wallOnSide = collisionInfo.left || collisionInfo.right;// pushingLeftWall || pushingRightWall;
        if (wallOnSide && !collisionInfo.below && velocity.y < 0)
        {
            isWallSliding = true;
            _jumpCount = 0;
            
            float wallSpeedDampTime = _lastInput.verticalMovement < 0 ? 0.1f : _view.wallSlideSpeedDampTime;
            
            if (velocity.y < -_view.wallSlideSpeedMax)
            {
                float targetYSpeed = -_view.wallSlideSpeedMax;
                velocity.y = Mathf.SmoothDamp(velocity.y,targetYSpeed, ref _wallSlideVelocity, wallSpeedDampTime);
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
}

using GhostGen;
using UnityEngine;

public class BruiserController : NotificationDispatcher, IAvatarController
{
    private AvatarView _view;
    
    private float _gravity;
    
    private FrameInput _lastInput;
    private IInputGenerator _inputGen;

    private EnemyState _state;
    private UnitMap.Unit _unit;
    private UnitStats _unitStats;
    private GameSystems _gameSystems;

    public BruiserController(UnitMap.Unit unit, AvatarState state, IAvatarView view, IInputGenerator inputGen)
    {
        _unit = unit;
        _unitStats = unit.stats;
        _state = state as EnemyState;
        
        _view = view as AvatarView;
        _view.controller = this;

        _inputGen = inputGen;
    }
    
    public bool isSimulating { get; set; }
    
    public IInputGenerator input
    {
        get { return _inputGen; }
        set { _inputGen = value; }
    }
    
    public UnitType GetUnitType()
    {
        return _unit.type;
    }
    
    // public void SetVelocity(Vector3 velocity)
    // {
    //     _state.velocity = velocity;
    // }

    public string uuid
    {
        get { return _state.uuid; }
    }

    public AttackResult TakeDamage(AttackData attackData)
    {
        float vBump = collisionInfo.below ? 2.0f : _state.velocity.y;
        Vector3 hitBumpForce = new Vector3(attackData.hitDirection.x * 3.0f, vBump, 0.0f);
        _state.velocity = hitBumpForce;
        
        _state.health = _state.health - attackData.potentialDamage;
        return new AttackResult(attackData, this, attackData.potentialDamage, _state.health);
    }
    
    public float health
    {
        get { return _state.health; }
    }

    public bool isDead
    {
        get { return _state.health <= 0.0f; }
    }
    
    // Start is called before the first frame update
    public void Start(GameSystems gameSystems)
    {
        _gameSystems = gameSystems;
        
        float timeToJumpApex = _unitStats.timeToJumpApex;
        _gravity = -(2 * _unitStats.maxJumpHeight) / (timeToJumpApex * timeToJumpApex);
        
        ProjectileSystem projectileSystem = gameSystems.Get<ProjectileSystem>();

    }

    public void CleanUp()
    {
    
    }
        
    public IAvatarView view
    {
        get { return _view; }
    }
    
    public AvatarState state
    {
        get { return _state; }
    }
    
    public UnitMap.Unit unit
    {
        get { return _unit; }
    }
    
    
    public void Move(Vector2 moveDelta, bool isOnPlatform)
    {
        if (_view && _view.constrainer != null)
        {
            Vector2 constrainedMoveDelta = _view.constrainer.Move(moveDelta, isOnPlatform, _lastInput);

            _state.prevPosition = _state.position;
            _state.position = _state.position + constrainedMoveDelta;

            _view._viewRoot.position = _state.prevPosition;
            _view.transform.position = _state.position;
        }
    }
    
    // Update is called once per frame
    public void Step(float deltaTime)
    {
        float alpha = (Time.time - Time.fixedTime) / Time.fixedDeltaTime;
        _view._viewRoot.position = Vector2.Lerp(_state.prevPosition, _state.position, alpha);
    }

    public void FixedStep(float deltaTime, FrameInput input)
    {
        _lastInput = input;
        
        deltaTime = deltaTime * _state.timeScale;
        
        
        _handleDirectionMovement(ref _state.velocity, deltaTime);
        _handleWallSliding(ref _state.velocity, deltaTime, 0, 0);
        _handleJumping(ref _state.velocity, deltaTime, false, 0, 0);
        
        Move(_state.velocity * deltaTime, false);
        
        // *Bop*
        if (collisionInfo.above || collisionInfo.below)
        {
            _state.velocity.y = 0;
        }

        // Vector2 aimStartPosition = _view._weaponHook.position;
        // Vector2 cursorDir = (_lastInput.cursorPosition - aimStartPosition).normalized;
        // float distance = 5.5f;
        //
        // // Aim
        // Vector2 aimDirection = _lastInput.cursorDirection.normalized;
        // Vector2 aimPosition = _lastInput.useCusorPosition ? aimStartPosition + (cursorDir * distance) : aimStartPosition + (aimDirection * distance);
        //
        // if (_view)
        // {
        //   Debug.DrawRay(aimStartPosition, aimDirection, Color.cyan, 0.2f);
        //
        //     const float kDebugLineSize = 0.2f;
        //     Debug.DrawLine(aimPosition + Vector2.down * kDebugLineSize, aimPosition + Vector2.up     * kDebugLineSize, Color.cyan, 0.2f);
        //     Debug.DrawLine(aimPosition + Vector2.left * kDebugLineSize, aimPosition + Vector2.right  * kDebugLineSize, Color.cyan, 0.2f);
        //
        //     _view.Aim(aimPosition);
        // }

       
        
        // Clear buffered input as we handled it above (for transient button up/down events that may get missed in a fixedUpdate
        if (_inputGen != null)
        {
            _inputGen.Clear();
        }
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

    private void _handleDirectionMovement(ref Vector2 velocity, float deltaTime)
    {
        float targetVelocityX = _lastInput.horizontalMovement * _unitStats.speed;
        
        float airFriction =  _state.jumpCount == 2 ? _unitStats.accelerationTimeDoubleJumpAir : _unitStats.accelerationTimeAir;
        float accelerationTime = collisionInfo.below ? _unitStats.accelerationTimeGround : airFriction; 
        float inputVelocityX = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref _state.velocityXSmoothing, accelerationTime);

        velocity.x = inputVelocityX;
        if (velocity.y > -Mathf.Abs(_unitStats.terminalVelocity))
        {
            velocity.y += _gravity * deltaTime;
        }
    }
    
    
    private bool _handleJumping(ref Vector2 velocity, float deltaTime, bool isWallSliding, int wallDirX, int inputDirX)
    {
        bool isJumping = false;
        
        float timeToJumpApex = _unitStats.timeToJumpApex;
        _gravity = -(2 * _unitStats.maxJumpHeight) / (timeToJumpApex * timeToJumpApex);
        
        float maxJumpVelocity = Mathf.Abs(_gravity) * timeToJumpApex;
        float minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(_gravity) * _unitStats.minJumpHeight);
       
        _gravity = (_lastInput.verticalMovement < 0) ? _gravity * 1.5f : _gravity;

        if (_lastInput.jumpPressed)
        {
            _state.midairJumpTimer = _unitStats.jumpRememberDelay;
        }

        bool didDoubleJump = (_state.jumpCount == 1 && _lastInput.jumpPressed);
        
        // This is so if the player jumps while in the air for a bit, we still jump when on floor
        if (_state.midairJumpTimer > 0)
        {
            if (isWallSliding)
            {
                if (wallDirX == inputDirX)
                {
                    velocity.x = -wallDirX * _unitStats.wallJumpClimb.x;
                    velocity.y = _unitStats.wallJumpClimb.y;
                }
                else if (inputDirX == 0)
                {
                    velocity.x = -wallDirX * _unitStats.wallJumpOff.x;
                    velocity.y = _unitStats.wallJumpOff.y;
                }
                else
                {
                    velocity.x = -wallDirX * _unitStats.wallJumpLeap.x;
                    velocity.y = _unitStats.wallJumpLeap.y;
                }

                isJumping = true;
                _state.jumpCount = 3;
                _state.midairJumpTimer = 0;
            }
            else
            {
                if ( _state.coyoteJumpTimer > 0 || didDoubleJump)
                {
                    _state.coyoteJumpTimer = 0;
                    if (collisionInfo.slidingDownMaxSlope)
                    {
                        if (inputDirX == 0 || inputDirX != -(int) Mathf.Sign(collisionInfo.slopeNormal.x))
                        {
                            Vector2 jumpDirection = (Vector2.up + collisionInfo.slopeNormal).normalized * maxJumpVelocity;
                            velocity.x = jumpDirection.x;
                            velocity.y = jumpDirection.y;
                        
                            isJumping = true;
                            _state.jumpCount++;
                        }
                    }
                    else 
                    {
                        velocity.y = _lastInput.verticalMovement < 0 ? maxJumpVelocity * 0.75f : maxJumpVelocity;
                        if (_state.jumpCount > 0)
                        {
                            velocity.x = inputDirX * _unitStats.wallJumpOff.x;
                        }
                    
                        isJumping = true;
                        _state.jumpCount++;
                    }

                    _state.midairJumpTimer = 0;
                }
            } 
        }

        if (_lastInput.jumpReleased)
        {
            
            if (velocity.y > minJumpVelocity)
            {
                velocity.y = minJumpVelocity;
            }
        }
        
        _state.midairJumpTimer -= deltaTime;
        _state.coyoteJumpTimer -= deltaTime;

        return isJumping;
    }

    private bool _handleWallSliding(ref Vector2 velocity, float deltaTime, int wallDirX, int inputDirX)
    {
        bool isWallSliding = false;
        
        bool wallOnSide = collisionInfo.left || collisionInfo.right;// pushingLeftWall || pushingRightWall;
        
        if (wallOnSide && !collisionInfo.below)// && velocity.y < 0)
        {
            isWallSliding = true;
            _state.jumpCount = 1;
            
            float wallSpeedDampTime = _lastInput.verticalMovement < 0 ? 0.1f : _unitStats.wallSlideSpeedDampTime;
            
            if (velocity.y < -_unitStats.wallSlideSpeedMax)
            {
                float targetYSpeed = -_unitStats.wallSlideSpeedMax;
                velocity.y = Mathf.SmoothDamp(velocity.y, targetYSpeed, ref _state.wallSlideVelocity, wallSpeedDampTime);
            }

            if (_state.timeToWallUnstick > 0)
            {
                _state.velocityXSmoothing = 0;
                velocity.x = 0;

                if (inputDirX != wallDirX && inputDirX != 0)
                {
                    _state.timeToWallUnstick -= deltaTime;
                }
                else
                {
                    _state.timeToWallUnstick = _unitStats.wallStickTime;
                }
            }
            else
            {
                _state.timeToWallUnstick = _unitStats.wallStickTime;
            }
        }

        return isWallSliding;
    }
}

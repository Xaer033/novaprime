using GhostGen;
using UnityEngine;

public class PlayerController : NotificationDispatcher, IAvatarController
{
    private AvatarView _view;

    private float _gravity;

    private FrameInput _lastInput;
    private IInputGenerator _input;
    
    private float _resetPlatformTime;

    private IWeaponController _machineGunController;
    private PlayerState _state;
    private UnitMap.Unit _unit;
    private UnitStats _unitStats;
    private GameSystems _gameSystems;
    
    
    
    public PlayerController(UnitMap.Unit unit, PlayerState state, IAvatarView view, IInputGenerator input)
    {
        _unit = unit;
        _unitStats = unit.stats;
        _state = state;
        
        _view = view as AvatarView;
        _view.controller = this;

        _input = input;
        
        _view.AddListener("onIdleEnter", onIdleEnter);
    }

    public IInputGenerator input
    {
        get { return _input; }
        set { _input = value; }
    }
    
    public Vector3 GetPosition()
    {
        return _state.position;
    }
    
    public int playerNumber
    {
        set
        {
            _state.playerNumber = value;
        }
    }
    public UnitType GetUnitType()
    {
        return _unit.type;
    }

    public string GetUUID()
    {
        return _state.uuid;
    }

    public AttackResult TakeDamage(AttackData attackData)
    {
        float vBump = collisionInfo.below ? 2.0f : _state.velocity.y;
        Vector3 hitBumpForce = new Vector3(attackData.hitDirection.x * 3.0f, vBump, 0.0f);
        SetVelocity(hitBumpForce);
        
        _state.health = _state.health - attackData.potentialDamage;
        return new AttackResult(this, attackData.potentialDamage, _state.health, !isDead );
    }
    
    public int health
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

        ProjectileSystem projectileSystem = gameSystems.GetSystem<ProjectileSystem>();
        _machineGunController = new MachineGunController(projectileSystem, _state.machineGunState, _unitStats.machineGunData);
        _view.SetWeapon(_machineGunController.view);
        
        _state.stateType = PlayerActivityType.ACTIVE;
    }

    public AvatarView GetView()
    {
        return _view;
    }
    
    public AvatarState GetState()
    {
        return _state;
    }
    
    public UnitMap.Unit GetUnit()
    {
        return _unit;
    }
    
    public void SetVelocity(Vector3 velocity)
    {
        _state.velocity = velocity;
    }
    
    public void Move(Vector3 moveDelta, bool isOnPlatform)
    {
        if (_view && _view.constrainer != null)
        {
            Vector3 constrainedMoveDelta = _view.constrainer.Move(moveDelta, isOnPlatform, _lastInput);

            _state.previousPosition = _state.position;
            _state.position = _state.position + constrainedMoveDelta;

            _view._viewRoot.localPosition = Vector3.zero;
            _view.transform.position = _state.position;
        }
    }
    
    // Update is called once per frame
    public void Step(float deltaTime)
    {
        float alpha = (Time.time - Time.fixedTime) / Time.fixedDeltaTime;
        _view._viewRoot.position = Vector3.Lerp(_state.previousPosition, _state.position, alpha);

    }

    public void FixedStep(float deltaTime, FrameInput input)
    {
        _lastInput = input;
        AnimationInfo animationInfo = new AnimationInfo();
        
        deltaTime = _lastInput.secondaryFire ? deltaTime * 0.5f : deltaTime;
        deltaTime = deltaTime * _state.timeScale;

        int wallDirX = _view.constrainer.collisionInfo.left ? -1 : 1;
        int inputDirX = Mathf.Abs(_lastInput.horizontalMovement) < 0.5f ? 0 : _lastInput.horizontalMovement < 0 ? -1 : 1;
        
        _handleDirectionMovement(ref _state.velocity, deltaTime);
        animationInfo.isRunning = Mathf.Abs(_state.velocity.x) > 0.001f;
        animationInfo.runSpeed = _state.velocity.x * _unitStats.animationRunSpeed;
       
        _state.isWallSliding     = _handleWallSliding(ref _state.velocity, deltaTime, wallDirX, inputDirX);
        animationInfo.canJump    = _handleJumping(ref _state.velocity, deltaTime, _state.isWallSliding, wallDirX, inputDirX);
        animationInfo.isWallSliding = _state.isWallSliding;
        
        Move(_state.velocity * deltaTime, false);
        
        if (collisionInfo.below)
        {
            _state.coyoteJumpTimer = _unitStats.coyoteTime;
            _state.jumpCount = 0;
            animationInfo.isGrounded = true;
        }
        else
        {
            if (_state.coyoteJumpTimer <= 0 && _state.jumpCount == 0 && !_state.isWallSliding)
            {
                _state.jumpCount++;
            }

            if (_state.velocity.y < 0)
            {
                animationInfo.isFalling = true;
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

        Vector3 aimStartPosition = _view.armHook.position;
        Vector3 cursorDir = (_lastInput.cursorPosition - aimStartPosition).normalized;
        float distance = 5.5f;
        
        // Aim
        Vector3 aimDirection = _lastInput.cursorDirection.normalized;
        Vector3 aimPosition = _lastInput.useCusorPosition ? aimStartPosition + (cursorDir * distance) : aimStartPosition + (aimDirection * distance);

        if (_view)
        {
          Debug.DrawRay(aimStartPosition, aimDirection, Color.cyan, 0.2f);

            const float kDebugLineSize = 0.2f;
            Debug.DrawLine(aimPosition + Vector3.down * kDebugLineSize, aimPosition + Vector3.up     * kDebugLineSize, Color.cyan, 0.2f);
            Debug.DrawLine(aimPosition + Vector3.left * kDebugLineSize, aimPosition + Vector3.right  * kDebugLineSize, Color.cyan, 0.2f);

            bool isFlipped = aimPosition.x < _view.transform.position.x;
            animationInfo.isBackPedaling = (isFlipped && _state.velocity.x > 0.0f) || (!isFlipped && _state.velocity.x < 0.0f);
            
            animationInfo.runSpeed *= isFlipped ? -1.0f : 1.0f;

            _view.Aim(aimPosition);
            handleAnimationStates(animationInfo);
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
        if (_input != null)
        {
            _input.Clear();
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
    

    private void _handleDirectionMovement(ref Vector3 velocity, float deltaTime)
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
    
    private bool _handleJumping(ref Vector3 velocity, float deltaTime, bool isWallSliding, int wallDirX, int inputDirX)
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
                            Vector3 jumpDirection = (Vector3.up + collisionInfo.slopeNormal).normalized * maxJumpVelocity;
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


    private bool _handleWallSliding(ref Vector3 velocity, float deltaTime, int wallDirX, int inputDirX)
    {
        bool isWallSliding = false;
        
        bool wallOnSide = collisionInfo.left || collisionInfo.right;// pushingLeftWall || pushingRightWall;
        if (wallOnSide && !collisionInfo.below && velocity.y < 0)
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

    private void handleAnimationStates(AnimationInfo animInfo)
    {
        AnimatorStateInfo state = _view.animator.GetCurrentAnimatorStateInfo(0);
        if (animInfo.canJump)
        {
            _view.PlayFootPuffFx();
            _view.animator.SetTrigger("jumpTrigger");
        }
        else if (animInfo.isGrounded)
        {
            if (animInfo.isRunning && !state.IsName("Running"))
            {
                if(state.IsName("Falling"))
                {
                    _view.PlayFootPuffFx();
                }
                _view.animator.SetTrigger("runTrigger");
            }
            else if(!animInfo.isRunning && !state.IsName("Idle"))
            {
                if(!state.IsName("Running"))
                {
                    _view.PlayFootPuffFx();
                }
                _view.animator.SetTrigger("idleTrigger");
                
            }
        }
        else if(!animInfo.isGrounded)
        {
            if (animInfo.isFalling && !state.IsName("Falling"))
            {
                _view.animator.SetTrigger("fallingTrigger");  
            }
        }

        float abs = Mathf.Abs(animInfo.runSpeed);
        float sign = Mathf.Sign(animInfo.runSpeed);
        float runSpeed = abs < 0.12f ? 0.12f * sign : animInfo.runSpeed;
        _view.animator.SetFloat("runSpeedScale", runSpeed);

    }
    
    private void onIdleEnter(GeneralEvent e)
    {
        AnimationEventDispatcher.AnimationEventData data = (AnimationEventDispatcher.AnimationEventData)e.data;
        Debug.Log("Idle Entered");
            
    }
    
    private struct AnimationInfo
    {
        public bool canJump;
        public bool isRunning;
        public bool isGrounded;
        public bool isFalling;
        public float runSpeed;
        public bool isBackPedaling;
        public bool isWallSliding;
    }
}

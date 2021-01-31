using GhostGen;
using UnityEngine;

public class PlayerController : NotificationDispatcher, IAvatarController
{
    private PlayerView _view;

    private float _gravity;

    private FrameInput _lastInput;
    // private bool _isSimulating;
    private IInputGenerator _inputGen;
    
    private float _resetPlatformTime;

    private IWeaponController _machineGunController;
    private PlayerState _state;
    private UnitMap.Unit _unit;
    private UnitStats _unitStats;
    private GameSystems _gameSystems;
    private Collider2D[] _interactColliderList;
    private PlayerActivityType _oldActivityType;
    private AttackData _killAttack;
    private int _crushedFrameCount;
    private bool _didJumpRelease;
    
    
    
    public PlayerController(UnitMap.Unit unit, AvatarState state, IAvatarView view, IInputGenerator inputGen)
    {
        _unit = unit;
        _unitStats = unit.stats;
        _state = state as PlayerState;
        _state.stateType = PlayerActivityType.ACTIVE;
        
        _view = view as PlayerView;
        _view.controller = this;

        _inputGen = inputGen;
        _interactColliderList = new Collider2D[5];
        
        _view.AddListener("onIdleEnter", onIdleEnter);
    }

    public IInputGenerator input
    {
        get { return _inputGen; }
        set { _inputGen = value; }
    }
    
    
    public bool isSimulating { get; set; }
    
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

    public string uuid
    {
        get { return _state.uuid; }
    }

    public AttackResult TakeDamage(AttackData attackData)
    {
        float vBump = collisionInfo.below ? 2.0f : _state.velocity.y;
        Vector3 hitBumpForce = new Vector3(attackData.hitDirection.x * 3.0f, vBump, 0.0f);
        SetVelocity(hitBumpForce);
        
        _state.health = _state.health - attackData.potentialDamage;
        
        if(attackData.damageType == DamageType.INSTANT)
        {
            _state.health = 0;
        }

        if(_state.health <= 0)
        {
            _state.stateType = PlayerActivityType.DEAD;
            _killAttack = attackData;
        }
        
        return new AttackResult(attackData, 
                                this, 
                                attackData.potentialDamage, 
                                _state.health);
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
        _machineGunController = new MachineGunController(projectileSystem, _state.machineGunState, _unitStats.machineGunData);
        _view.SetWeapon(uuid, _machineGunController);
        
        _state.stateType = PlayerActivityType.ACTIVE;
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
    
    public void SetVelocity(Vector3 velocity)
    {
        _state.velocity = velocity;
    }
    
    public void Move(Vector3 moveDelta, bool isOnPlatform)
    {
        _move(moveDelta, _lastInput, isOnPlatform);
    }
    
    // Update is called once per frame
    public void Step(float deltaTime)
    {
        float alpha = (Time.time - Time.fixedTime) / Time.fixedDeltaTime;
        if(view != null && view.viewRoot != null && _state != null)
        {
             view.viewRoot.position = Vector3.Lerp(_state.previousPosition, _state.position, alpha);
        }
    }

    public void FixedStep(float deltaTime, FrameInput input)
    {
        if(!isSimulating)
        {
            return;
        }
        
        _lastInput = input;
        AnimationInfo animationInfo = new AnimationInfo();
        
        deltaTime = input.secondaryFire ? deltaTime * 0.5f : deltaTime;
        deltaTime = deltaTime * _state.timeScale;

        switch(_state.stateType)
        {
            case PlayerActivityType.ACTIVE:
                handleBeingAlive(deltaTime, input, ref animationInfo);
                break;
            case PlayerActivityType.DEAD:
                handleBeingDead(deltaTime);
                break;
        }

        handleAnimationStates(animationInfo);

        _oldActivityType = _state.stateType;
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

    private void _move(Vector3 moveDelta, FrameInput input, bool isOnPlatform)
    {
        if (_view && _view.constrainer != null)
        {
            Vector3 constrainedMoveDelta = _view.constrainer.Move(moveDelta, isOnPlatform, input);

            _state.previousPosition = _state.position;
            _state.position = _state.position + constrainedMoveDelta;

            _view.transform.position = _state.position;
            _view._viewRoot.position = _state.previousPosition;
        }
    }
    
    private void _handleDirectionMovement(ref Vector3 velocity, FrameInput input, float deltaTime)
    {
        float targetVelocityX = input.horizontalMovement * _unitStats.speed;
        
        float airFriction =  _state.jumpCount == 2 ? _unitStats.accelerationTimeDoubleJumpAir : _unitStats.accelerationTimeAir;
        float accelerationTime = collisionInfo.below ? _unitStats.accelerationTimeGround : airFriction; 
        float inputVelocityX = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref _state.velocityXSmoothing, accelerationTime);

        velocity.x = inputVelocityX;
        if (velocity.y > -Mathf.Abs(_unitStats.terminalVelocity))
        {
            velocity.y += _gravity * deltaTime;
        }
    }
    
    private bool _handleJumping(ref Vector3 velocity, FrameInput input, float deltaTime, bool isWallSliding, int wallDirX, int inputDirX)
    {
        bool isJumping = false;
        
        float timeToJumpApex = _unitStats.timeToJumpApex;
        _gravity = -(2 * _unitStats.maxJumpHeight) / (timeToJumpApex * timeToJumpApex);
        
        float maxJumpVelocity = Mathf.Abs(_gravity) * timeToJumpApex;
        float minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(_gravity) * _unitStats.minJumpHeight);
       
        _gravity = (input.verticalMovement < 0) ? _gravity * 1.5f : _gravity;

        if (input.jumpPressed)
        {
            _state.midairJumpTimer = _unitStats.jumpRememberDelay;
        }

        bool didDoubleJump = (_state.jumpCount == 1 && input.jumpPressed && _didJumpRelease);
        if(didDoubleJump || collisionInfo.below)
        {
            _didJumpRelease = false;
        }
        
        // This is so if the player jumps while in the air after walking off a cliff, we still jump
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
                        velocity.y = input.verticalMovement < 0 ? maxJumpVelocity * 0.75f : maxJumpVelocity;
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

        if (input.jumpReleased)
        {
            _didJumpRelease = true;
            if (velocity.y > minJumpVelocity)
            {
                velocity.y = minJumpVelocity;
            }
        }
        
        _state.midairJumpTimer -= deltaTime;
        _state.coyoteJumpTimer -= deltaTime;

        return isJumping;
    }


    private bool _handleWallSliding(ref Vector3 velocity, FrameInput input, float deltaTime, int wallDirX, int inputDirX)
    {
        bool isWallSliding = false;
        
        bool wallOnSide = collisionInfo.left || collisionInfo.right;// pushingLeftWall || pushingRightWall;
        
        if (wallOnSide && !collisionInfo.below)// && velocity.y < 0)
        {
            isWallSliding = true;
            _state.jumpCount = 1;
            
            float wallSpeedDampTime = input.verticalMovement < 0 ? 0.1f : _unitStats.wallSlideSpeedDampTime;
            
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

    private void _handleInteract(FrameInput input)
    {
        if(!input.interactPressed)
        {
            return;
        }

        Vector3 upVector = Vector3.up * 1.0f;
        Vector3 rightVector = Vector3.right * _view.viewRoot.localScale.x * 1f;
        
        Vector3 p1 = _state.position;
        Vector3 p2 = p1 + rightVector + upVector;
        
        Debug.DrawLine(p1, p1 + upVector, Color.yellow, 1.0f);
        Debug.DrawLine(p1, p1 + rightVector, Color.yellow, 1.0f);
        Debug.DrawLine(p1 + upVector, p2, Color.yellow, 1.0f);
        Debug.DrawLine(p1 + rightVector, p2, Color.yellow, 1.0f);
        
        int hitCount = Physics2D.OverlapAreaNonAlloc(p1, p2, _interactColliderList);
        for(int i= 0; i < hitCount; ++i)
        {
            Collider2D col = _interactColliderList[i];
            
            // TODO: Optimize
            InteractTrigger trigger = col.GetComponent<InteractTrigger>();
            trigger?.Interact(this);            
        }
    }
    
    private void handleAnimationStates(AnimationInfo animInfo)
    {
        if(_view == null || _view.animator == null)
        {
            return;
        }
        
        AnimatorStateInfo animState = _view.animator.GetCurrentAnimatorStateInfo(0);
        if (animInfo.canJump)
        {
            _view.PlayFootPuffFx();
            _view.animator.SetTrigger("jumpTrigger");
        }
        else if (animInfo.isGrounded)
        {
            if (animInfo.isRunning && !animState.IsName("Running"))
            {
                if(animState.IsName("Falling"))
                {
                    _view.PlayFootPuffFx();
                }
                _view.animator.SetTrigger("runTrigger");
            }
            else if(!animInfo.isRunning && !animState.IsName("Idle"))
            {
                if(!animState.IsName("Running"))
                {
                    _view.PlayFootPuffFx();
                }
                _view.animator.SetTrigger("idleTrigger");
            }
        }
        else if(!animInfo.isGrounded)
        {
            if (animInfo.isFalling && !animState.IsName("Falling"))
            {
                _view.animator.SetTrigger("fallingTrigger");  
            }
        }

        float abs = Mathf.Abs(animInfo.runSpeed);
        float sign = Mathf.Sign(animInfo.runSpeed);
        float runSpeed = abs < 0.12f ? 0.12f * sign : animInfo.runSpeed;
        _view.animator.SetFloat("runSpeedScale", runSpeed);

        // We died
        if(_state.stateType == PlayerActivityType.DEAD && _state.stateType != _oldActivityType)
        {
            _view.deadRoot.SetParent(_view.transform.parent);
            _view.animator.SetTrigger("deathTrigger");
            _view.viewRoot.gameObject.SetActive(false);
            _view.deadRoot.gameObject.SetActive(true);
            
            _view.cameraTargetGroup.m_Targets[0].weight = 0;
            _view.cameraTargetGroup.m_Targets[1].weight = 0;
            _view.cameraTargetGroup.m_Targets[2].weight = 1;

            Rigidbody2D[] deathBodies = _view.deadRoot.GetComponentsInChildren<Rigidbody2D>();
            for(int i = 0; i < deathBodies.Length; ++i)
            {
                deathBodies[i].AddForceAtPosition(_killAttack.hitDirection * 75.0f, 
                                                  _killAttack.raycastHit.point, 
                                                  ForceMode2D.Impulse);
            }
        }
    }
    
    private void onIdleEnter(GeneralEvent e)
    {
        AnimationEventDispatcher.AnimationEventData data = (AnimationEventDispatcher.AnimationEventData)e.data;
        // Debug.Log("Idle Entered");
    }

    private void handleBeingDead(float deltaTime)
    {
        _state.previousPosition = _state.position;
    }
    
    private void handleBeingAlive(float deltaTime, FrameInput input, ref AnimationInfo animationInfo)
    {
        _handleInteract(input);
        
        int wallDirX = _view.constrainer.collisionInfo.left ? -1 : 1;
        int inputDirX = Mathf.Abs(input.horizontalMovement) < 0.5f ? 0 : input.horizontalMovement < 0 ? -1 : 1;
        
        _handleDirectionMovement(ref _state.velocity, input, deltaTime);
        animationInfo.isRunning = Mathf.Abs(_state.velocity.x) > 0.001f;
        animationInfo.runSpeed = _state.velocity.x * _unitStats.animationRunSpeed;
       
        _state.isWallSliding     = _handleWallSliding(ref _state.velocity, input, deltaTime, wallDirX, inputDirX);
        animationInfo.canJump    = _handleJumping(ref _state.velocity, input, deltaTime, _state.isWallSliding, wallDirX, inputDirX);
        animationInfo.isWallSliding = _state.isWallSliding;
        
        _move(_state.velocity * deltaTime, input, false);

        _crushedFrameCount = collisionInfo.crushed ? _crushedFrameCount + 1 : 0;
        if(collisionInfo.crushed)//if(_crushedFrameCount > 5)
        {
            AttackData attackData = createCrushDamage();
            TakeDamage(attackData);
            Debug.LogError("YOU DEAD");
        }
        
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
                _didJumpRelease = true;
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

        if (_view)
        { 
            const float kDistance = 5.5f;
            Vector3 aimStartPosition = _view._weaponHook.position;
            Vector3 cursorDir = (input.cursorPosition - aimStartPosition).normalized;
        
            // Aim
            Vector3 aimDirection = input.cursorDirection.normalized;
            Vector3 aimPosition = input.useCusorPosition ? 
                aimStartPosition + (cursorDir * kDistance) : 
                aimStartPosition + (aimDirection * kDistance);

            Debug.DrawRay(aimStartPosition, aimDirection, Color.cyan, 0.2f);

            const float kDebugLineSize = 0.2f;
            Debug.DrawLine(aimPosition + Vector3.down * kDebugLineSize, aimPosition + Vector3.up     * kDebugLineSize, Color.cyan, 0.2f);
            Debug.DrawLine(aimPosition + Vector3.left * kDebugLineSize, aimPosition + Vector3.right  * kDebugLineSize, Color.cyan, 0.2f);

            bool isFlipped = aimPosition.x < _view.transform.position.x;
            animationInfo.isBackPedaling = (isFlipped && _state.velocity.x > 0.0f) || (!isFlipped && _state.velocity.x < 0.0f);
            animationInfo.runSpeed *= isFlipped ? -1.0f : 1.0f;

            _view.Aim(aimPosition);
            
            // Weapon Handling
            if (_machineGunController != null)
            {
                _machineGunController.FixedStep(deltaTime);

                if (input.primaryFire)
                {
                    _machineGunController.Fire(aimPosition);
                }
            }
        }
    }
    
    private AttackData createCrushDamage()
    {
        AttackData attack = new AttackData(uuid,
                                           0, 
                                           DamageType.INSTANT, 
                                           -1, 
                                           Vector3.zero);
        
        return attack;
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

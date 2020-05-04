using GhostGen;
using UnityEngine;

public class GruntController : NotificationDispatcher, IAvatarController
{
    private AvatarView _view;

    private float _gravity;
    
    private FrameInput _lastInput;
    private IInputGenerator _input;
    private float _resetPlatformTime;

    private MachineGunController _machineGunController;
    private EnemyState _state;
    private UnitMap.Unit _unit;
    private UnitStats _unitStats;
    private GameSystems _gameSystems;
    
    public GruntController(UnitMap.Unit unit, EnemyState state, AvatarView view, IInputGenerator input)
    {
        _unit = unit;
        _unitStats = unit.stats;
        _state = state;
        
        _view = view;
        _view.controller = this;

        _input = input;
        
        view.AddListener("onAnimDeathComplete", onAnimDeathComplete);
    }

    public IInputGenerator GetInput()
    {
        return _input;
    }

    public void SetInput(IInputGenerator input)
    {
        _input = input;
    }
    
    public UnitType GetUnitType()
    {
        return _unit.type;
    }
    
    public Vector3 GetPosition()
    {
        return _state.position;
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
    
    public string GetUUID()
    {
        return _state.uuid;
    }

    public void SetVelocity(Vector3 velocity)
    {
        _state.velocity = velocity;
    }
    
    public AttackResult TakeDamage(AttackData attackData)
    {
        if (_state.health > 0)
        {
            _state.health = _state.health - attackData.potentialDamage;

            if (isDead)
            {
                _view.SetAnimationTrigger("deathTrigger");
            }
        }
        
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
        
        _machineGunController = new MachineGunController(_gameSystems, _state.machineGunState, _unitStats.machineGunData);
        _view.SetWeapon(_machineGunController.view.transform);
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

            _state.position = _state.position + constrainedMoveDelta;
            _view.transform.localPosition = _state.position;
        }
    }
    
    // Update is called once per frame
    public void Step(float deltaTime)
    {
        
    }

    public void FixedStep(float deltaTime, FrameInput input)
    {
        _lastInput = input;
        
        deltaTime = _lastInput.secondaryFire ? deltaTime * 0.5f : deltaTime;
        deltaTime = deltaTime * _state.timeScale;


        Vector3 aimDirection = _lastInput.cursorDirection.normalized;
        Vector3 aimPosition = _lastInput.useCusorPosition ? _lastInput.cursorPosition : _state.position + (aimDirection * 4.0f);
        Debug.DrawRay(_state.position, aimDirection, Color.cyan, 0.2f);

        const float kDebugLineSize = 0.2f;
        Debug.DrawLine(aimPosition + Vector3.down * kDebugLineSize, aimPosition + Vector3.up     * kDebugLineSize, Color.cyan, 0.2f);
        Debug.DrawLine(aimPosition + Vector3.left * kDebugLineSize, aimPosition + Vector3.right  * kDebugLineSize, Color.cyan, 0.2f);
    
        
        int wallDirX = _view.constrainer.collisionInfo.left ? -1 : 1;
        int inputDirX = Mathf.Abs(_lastInput.horizontalMovement) < 0.5f ? 0 : _lastInput.horizontalMovement < 0 ? -1 : 1;
        
        _handleDirectionMovement(ref _state.velocity, deltaTime);

        _state.isWallSliding = _handleWallSliding(ref _state.velocity, deltaTime, wallDirX, inputDirX);
        _handleJumping(ref _state.velocity, deltaTime, _state.isWallSliding, wallDirX, inputDirX);
        
        Move(_state.velocity * deltaTime, false);
        
        if (collisionInfo.below)
        {
            _state.coyoteJumpTimer = _unitStats.coyoteTime;
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

        if (isDead)
        {
            _view.constrainer.maxSlopeAngle = 10.0f;
        }
        
        if (_view)
        {
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
        
        float accelerationTime = collisionInfo.below ? _unitStats.accelerationTimeGround : _unitStats.accelerationTimeAir; 
        float inputVelocityX = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref  _state.velocityXSmoothing, accelerationTime);

        velocity.x = inputVelocityX;
        if (velocity.y > -Mathf.Abs(_unitStats.terminalVelocity))
        {
            velocity.y += _gravity * deltaTime;
        }
    }
    
    private void _handleJumping(ref Vector3 velocity, float deltaTime, bool isWallSliding, int wallDirX, int inputDirX)
    {
        float timeToJumpApex = _unitStats.timeToJumpApex;
        _gravity = -(2 * _unitStats.maxJumpHeight) / (timeToJumpApex * timeToJumpApex);
        
        float maxJumpVelocity = Mathf.Abs(_gravity) * timeToJumpApex;
        float minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(_gravity) * _unitStats.minJumpHeight);
       
        _gravity = (_lastInput.verticalMovement < 0) ? _gravity * 1.5f : _gravity;

        if (_lastInput.jumpPressed)
        {
            _state.midairJumpTimer = _unitStats.jumpRememberDelay;
        }

        // This is so if the player jumps while in the air for a bit, we still jump when on floor
        if ( _state.midairJumpTimer > 0)
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

                _state.midairJumpTimer = 0;
            }
            else
            {
                if (  _state.coyoteJumpTimer > 0)
                {
                    _state.coyoteJumpTimer = 0;
                    if (collisionInfo.slidingDownMaxSlope)
                    {
                        if (inputDirX == 0 || inputDirX != -(int) Mathf.Sign(collisionInfo.slopeNormal.x))
                        {
                            Vector3 jumpDirection = (Vector3.up + collisionInfo.slopeNormal).normalized * maxJumpVelocity;
                            velocity.x = jumpDirection.x;
                            velocity.y = jumpDirection.y;
                        }
                    }
                    else 
                    {
                        velocity.y = _lastInput.verticalMovement < 0 ? maxJumpVelocity * 0.75f : maxJumpVelocity;
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
    }


    private bool _handleWallSliding(ref Vector3 velocity, float deltaTime, int wallDirX, int inputDirX)
    {
        bool isWallSliding = false;
        
        bool wallOnSide = collisionInfo.left || collisionInfo.right;// pushingLeftWall || pushingRightWall;
        if (wallOnSide && !collisionInfo.below && velocity.y < 0)
        {
            isWallSliding = true;
            
            float wallSpeedDampTime = _lastInput.verticalMovement < 0 ? 0.1f : _unitStats.wallSlideSpeedDampTime;
            
            if (velocity.y < -_unitStats.wallSlideSpeedMax)
            {
                float targetYSpeed = -_unitStats.wallSlideSpeedMax;
                velocity.y = Mathf.SmoothDamp(velocity.y, targetYSpeed, ref  _state.wallSlideVelocity, wallSpeedDampTime);
            }

            if ( _state.timeToWallUnstick > 0)
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

    private void onAnimDeathComplete(GeneralEvent e)
    {
        AnimationEventDispatcher.AnimationEventData data = (AnimationEventDispatcher.AnimationEventData)e.data;
        _view.DeathFadeOut(() => { _view.constrainer.collisionCollider.enabled = false; });
        
    }
}

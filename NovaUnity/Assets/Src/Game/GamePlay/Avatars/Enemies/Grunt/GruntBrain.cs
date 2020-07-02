using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GruntBrain : IInputGenerator
{
    private const float kTickRate = 0.1f;
        
    private UnitStats _unitStats;
    private EnemyState _state;
    private GameSystems _gameSystems;
    private AvatarSystem _avatarSystem;
    private IAvatarController _targetController;

    private FrameInput _lastInput;
    private double _lastUpdate;
    private float _currentTickRate;
    private RaycastHit2D[] _raycastHits;
    
    public GruntBrain(GameSystems gameSystems, UnitStats stats, EnemyState state)
    {
        _gameSystems = gameSystems;
        _unitStats = stats;
        _state = state;

        _state.aiState = AiState.TARGETING;
        _currentTickRate = kTickRate;

        _avatarSystem = _gameSystems.Get<AvatarSystem>();
        
        _raycastHits = new RaycastHit2D[1];
    }

    public FrameInput GetInput()
    {
        double now = Time.fixedTime;
        double delta = now - _lastUpdate;
        
        if (delta < _currentTickRate)
        {
            return _lastInput;
        }

        _lastUpdate = now;
        _currentTickRate = kTickRate + Random.Range(0.0f, 0.15f);
        
        FrameInput frameInput = new FrameInput();
        
        if (_state.health <= 0)
        {
            _state.aiState = AiState.DEAD;
        }
        
        switch (_state.aiState)
        {
            case AiState.NONE:
            case AiState.IDLE:
                _handleIdleState();
                break;
            
            case AiState.TARGETING:
                _handleTargetingState();
                break;
            
            case AiState.ATTACKING:
                _handleAttackingState(ref frameInput);
                break;
            
            case AiState.DEAD: break;
        }

        _lastInput = frameInput;
        return frameInput;
    }

    public void Clear()
    {

    }

    private bool _findTarget(out string uuid, out IAvatarController c)
    {
        uuid = "";
        c = null;

        const float findRadius = 5.0f;
        
        Debug.DrawLine(_state.position, _state.position + Vector3.left  * findRadius, Color.blue, 0.5f);
        Debug.DrawLine(_state.position, _state.position + Vector3.right * findRadius, Color.blue, 0.5f);
        Debug.DrawLine(_state.position, _state.position + Vector3.up    * findRadius, Color.blue, 0.5f);
        Debug.DrawLine(_state.position, _state.position + Vector3.down  * findRadius, Color.blue, 0.5f);
        
        Collider2D[] colliderList = Physics2D.OverlapCircleAll(_state.position, findRadius, _unitStats.targetLayerMask);
        for (int i = 0; i < colliderList.Length; ++i)
        {
            IAvatarView view = colliderList[i].GetComponent<IAvatarView>();
            if (view != null && view.controller != null && view.controller.state.health > 0)
            {
                uuid = view.controller.uuid;
                c = view.controller;
                return true;
            }
        }
        
        return false;
    }

    private void _handleIdleState()
    {

    }

    private void _handleTargetingState()
    {
        string uuid;
        IAvatarController c;

        if (_findTarget(out uuid, out c))
        {
            _state.targetUUID = uuid;
            _targetController = c;
            _state.aiState = AiState.ATTACKING;
        }
    }

    private void _handleAttackingState(ref FrameInput input)
    {
        _targetController = _avatarSystem.GetController(_state.targetUUID);
        if(_targetController == null)
        {
            _state.aiState = AiState.IDLE;
            return;
        }

        if(_targetController.state.health <= 0)
        {
            _state.aiState = AiState.TARGETING;
            return;
        }
        
        Vector3 targetPosition = (_targetController.state.position + Vector3.up * 0.8f) + (_state.velocity * Time.fixedDeltaTime);
        Vector3 startPosition = _state.position + Vector3.up * 0.85f;
        
        Vector3 dirToTarget = targetPosition - startPosition;

        float distance = dirToTarget.magnitude;
        if (distance > Random.Range(4, 7))
        {
            input.horizontalMovement = dirToTarget.normalized.x;
            
        }
        else if (distance < Random.Range(2, 5))
        {
            input.horizontalMovement = -dirToTarget.normalized.x;
        }

        RaycastHit2D hit;
        Vector3 movementDir = new Vector3(input.horizontalMovement, 0, 0);

        Vector3 fellowGruntCheckDir = (Vector3.left *  movementDir.x).normalized * 0.1f;
        Debug.DrawRay(startPosition, fellowGruntCheckDir, Color.green, 0.4f);

        // int enemyHitCount = Physics2D.RaycastNonAlloc(startPosition, fellowGruntCheckDir, _raycastHits, 2.5f, LayerMask.GetMask(new []{"enemies"}) );
        // if (enemyHitCount > 0)
        // {
        //     hit = _raycastHits[0];
        //     IAvatarController fellowGrunt = _avatarSystem.GetController(hit.transform.name);
        //     if (fellowGrunt.health > 0)
        //     {
        //         input.horizontalMovement =  hit.distance * Mathf.Sign(hit.normal.x) * 0.5f;
        //     }
        // }
        
        int wallHitCount = Physics2D.RaycastNonAlloc(startPosition, movementDir.normalized, _raycastHits, 1.0f, LayerMask.GetMask(new []{"obsticals"}) );
        if (wallHitCount > 0 || (Mathf.Abs(_state.velocity.x) < 0.001f && _state.isWallSliding) )
        {
            input.jumpPressed = true;
        }
        else
        {
            if (_lastInput.downPressed && !_lastInput.downReleased)
            {
                input.downReleased = true;
            }
        }
        
        input.useCusorPosition = true;
        Debug.DrawRay(startPosition, dirToTarget.normalized * 8.0f, Color.green, 0.4f);

        int hitPlayerCount = Physics2D.RaycastNonAlloc(startPosition, dirToTarget.normalized, _raycastHits, 8.0f, LayerMask.GetMask(new []{"obsticals", "player"}) );
        if (hitPlayerCount > 0 && _raycastHits[0].collider.gameObject.layer == LayerMask.NameToLayer("player"))
        {
            input.cursorPosition = targetPosition;
            input.primaryFire = true;
        }
        else
        {
            input.cursorPosition = _lastInput.cursorPosition;
            input.primaryFire = false;
        }
    }
}

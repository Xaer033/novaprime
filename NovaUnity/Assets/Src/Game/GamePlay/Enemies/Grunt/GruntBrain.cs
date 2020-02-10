using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GruntBrain : IInputGenerator
{
    private const float kTickRate = 0.1f;
        
    private UnitData _unitData;
    private EnemyState _state;
    private GameSystems _gameSystems;
    private IAvatarController _targetController;

    private FrameInput _lastInput;
    private double _lastUpdate;
    
    public GruntBrain(GameSystems gameSystems, UnitData data, EnemyState state)
    {
        _gameSystems = gameSystems;
        _unitData = data;
        _state = state;

        _state.aiState = AiState.TARGETING;
    }

    public FrameInput GetInput()
    {
        double now = Time.fixedTime;
        double delta = now - _lastUpdate;
        
        if (delta < kTickRate)
        {
            return _lastInput;
        }

        _lastUpdate = now;
        FrameInput frameInput = new FrameInput();

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

        Collider[] colliderList = Physics.OverlapSphere(_state.position, 3.0f, _unitData.targetLayerMask);
        for (int i = 0; i < colliderList.Length; ++i)
        {
            AvatarView view = colliderList[i].GetComponent<AvatarView>();
            if (view != null)
            {
                uuid = view.controller.GetUUID();
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
        Vector3 targetPosition = _targetController.GetPosition() + Vector3.up * 0.2f;
        Vector3 startPosition = _state.position + Vector3.up * 0.85f;
        
        Vector3 dirToTarget = targetPosition - startPosition;

        float distance = dirToTarget.magnitude;
        if (distance > 5)
        {
            input.horizontalMovement = dirToTarget.normalized.x;
            
        }
        else if (distance < 3)
        {
            input.horizontalMovement = -dirToTarget.normalized.x;
        }

        RaycastHit hit;
        
        Vector3 movementDir = new Vector3(input.horizontalMovement, 0, 0);
        
        Debug.DrawRay(startPosition, movementDir.normalized * 1.0f, Color.green, 0.4f);
        bool isHit = Physics.Raycast(startPosition, movementDir.normalized, out hit, 1.0f, LayerMask.GetMask(new []{"obsticals"}) );
        if (isHit)
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
        Debug.DrawRay(startPosition, dirToTarget.normalized * 8.0f, Color.green, 0.4f);

        bool canHitPlayer = Physics.Raycast(startPosition, dirToTarget.normalized ,out hit, 8.0f, LayerMask.GetMask(new []{"obsticals", "player"}) );
        if (canHitPlayer && hit.collider.gameObject.layer == LayerMask.NameToLayer("player"))
        {
            input.useCusorPosition = true;
            input.cursorPosition = targetPosition;

            input.primaryFire = true;
        }
        else
        {
            input.useCusorPosition = true;
            input.cursorPosition = _lastInput.cursorPosition;

            input.primaryFire = false;
        }
    }
}

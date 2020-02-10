using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GruntBrain : IInputGenerator
{
    private const float kTickRate = 0.4f;
        
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

        Collider[] colliderList = Physics.OverlapSphere(_state.position, 6.0f, _unitData.targetLayerMask);
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
        Vector3 targetPosition = _targetController.GetPosition();
        Vector3 direction = targetPosition - _state.position;

        input.horizontalMovement = direction.normalized.x;

        if (direction.magnitude < 2.0f)
        {
            Vector3 hitForce = new Vector3(direction.x * 5.0f, 2.0f, 0.0f);
            _targetController.SetVelocity(hitForce);
        }

    }
}

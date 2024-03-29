using System;
using Cinemachine;
using GhostGen;
using Mirror;
using UnityEngine;
using UnityEngine.Animations;

public class AvatarView : EventDispatcherBehavior, 
                            IAvatarView, 
                            IPlatformPassenger, 
                            ITimeWarpTarget, 
                            IAttackTarget
{
    
    // [BoxGroup("Hooks")]
    public Transform _viewRoot;
    
    
    // [BoxGroup("Hooks")]
    public Transform _cursorTarget;
    // [BoxGroup("Hooks")]
    public Transform _weaponHook;
    // [BoxGroup("Hooks")]
    public Transform _healthPositionHook;
    // [BoxGroup("Hooks")]
    public CinemachineTargetGroup cameraTargetGroup;
    
    // [BoxGroup("Limb Hooks")]
    public ParentConstraint _leftHandConstraint;
    // [BoxGroup("Limb Hooks")]
    public ParentConstraint _rightHandConstraint;
    
    public AvatarConstrainer constrainer;
    public NetworkEntity _netEntity;    
    public NetworkIdentity _networkIdentity;
    public Animator _animator;



    private Quaternion _weaponRotation;
    private Quaternion _prevWeaponRotation;
    
    
    private void Update()
    {
        float alpha = (Time.time - Time.fixedTime) / Time.fixedDeltaTime;
        if(_weaponHook != null)
        {
            _weaponHook.rotation = Quaternion.Slerp(_prevWeaponRotation, _weaponRotation, alpha);
        }
    }
    
    public void SetWeapon(string ownerUUID, IWeaponController weaponController)
    {
        if (weaponController != null && _weaponHook != null)
        {
            weaponController.Attach(ownerUUID, _weaponHook, _leftHandConstraint, _rightHandConstraint);
        }
    }
    
    public void Aim(Vector2 cursorPosition)
    {
        if(_cursorTarget != null)
        {
            _cursorTarget.position = cursorPosition;
        }
        
        if (_weaponHook != null)
        {
            Vector2 delta = (cursorPosition - (Vector2)_weaponHook.position + (Vector2.left * 0.001f)).normalized;
            
            _prevWeaponRotation = _weaponRotation;
            _weaponRotation = Quaternion.LookRotation(delta, Vector3.up);
        }

        if (viewRoot != null)
        {
            bool isFlipped = cursorPosition.x < transform.position.x;
            Vector3 localScale = viewRoot.localScale;
            localScale.x = isFlipped ?  -Mathf.Abs(localScale.x) : Mathf.Abs(localScale.x);
            _viewRoot.localScale = localScale;
        }
    }

    public Transform cameraTarget
    {
        get { return cameraTargetGroup.transform; }
    }
    
    public NetworkIdentity netIdentity
    {
        get => _networkIdentity;
    }

    public NetworkEntity netEntity
    {
        get => _netEntity;
    }
    
    public Animator animator
    {
        get { return _animator; }
        set { _animator = value; }
    }

    public Transform viewRoot
    {
        get { return _viewRoot; }
    }

    public void DeathFadeOut(Action onComplete)
    {
        Debug.Log("Fade this bitch out");
        if (onComplete != null)
        {
            onComplete();
        }
    }

    public void RequestMovement(PassengerMovement movement)
    {
        controller?.Move(movement.velocity, movement.isOnPlatform);
    }

    public void OnTimeWarpEnter(float timeScale)
    {
        controller?.OnTimeWarpEnter(timeScale);
    }

    public void OnTimeWarpExit()
    {
        controller?.OnTimeWarpExit();
    }

    public AttackResult TakeDamage(AttackData attackData)
    {
        if (controller != null)
        {
            return controller.TakeDamage(attackData);
        }

        return default(AttackResult);
    }
    
    public float     health
    {
        get { return controller != null ? controller.health : 0; }
    }
    
    public IAvatarController controller { get; set; }

    public Vector3 GetHealthPosition()
    {
        return _healthPositionHook != null ? _healthPositionHook.position : Vector3.zero;
    } 
}

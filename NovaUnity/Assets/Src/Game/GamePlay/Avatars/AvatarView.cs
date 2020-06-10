using System;
using Cinemachine;
using GhostGen;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Animations;

public class AvatarView : EventDispatcherBehavior, IPlatformPassenger, ITimeWarpTarget, IAttackTarget
{
    public CinemachineTargetGroup cameraTargetGroup;
    public AvatarConstrainer constrainer;
    public Transform armHook;
    public Transform _cursorTarget;
    public Transform _healthPositionHook;
    public Transform _viewRoot;
    public Transform _leftFootHook;
    public Transform _rightFootHook;
    public ParentConstraint _leftHandConstraint;
    public ParentConstraint _rightHandConstraint;
    public ParticleSystem _jumpPuffFXPrefab;
    public NetworkEntity _networkEntity;
    public Animator _animator;


    private ParticleSystem _leftFootPuffFx;
    private ParticleSystem _rightFootPuffFx;
    
    private void Awake()
    {
        if(_jumpPuffFXPrefab != null)
        {
            if(_leftFootHook != null)
            {
                _leftFootPuffFx = GameObject.Instantiate<ParticleSystem>(_jumpPuffFXPrefab, _viewRoot);
                _leftFootPuffFx.transform.localPosition = Vector3.zero;
                _leftFootPuffFx.transform.localRotation = quaternion.identity;
            }

            if(_rightFootHook != null)
            {
                _rightFootPuffFx = GameObject.Instantiate<ParticleSystem>(_jumpPuffFXPrefab, _viewRoot);
                _rightFootPuffFx.transform.localPosition = Vector3.zero;
                _rightFootPuffFx.transform.localRotation = quaternion.identity;
            }
        }
    }
    
    public void Aim(Vector3 cursorPosition)
    {
        if(_cursorTarget != null)
        {
            _cursorTarget.position = cursorPosition;
        }
        
        if (armHook != null)
        {
            Vector3 delta = (cursorPosition - armHook.position).normalized;
            armHook.rotation = Quaternion.LookRotation(delta, Vector3.up);
        }

        if (_viewRoot != null)
        {
            bool isFlipped = cursorPosition.x < transform.position.x;
            Vector3 localScale = _viewRoot.localScale;
            localScale.x = isFlipped ?  -Mathf.Abs(localScale.x) : Mathf.Abs(localScale.x);
            _viewRoot.localScale = localScale;
        }
    }

    public NetworkEntity netEntity
    {
        get => _networkEntity;
        set => _networkEntity = value;
    }

    public Animator animator
    {
        get { return _animator; }
        set { _animator = value; }
    }
    
    public void SetWeapon(IWeaponView weaponView)
    {
        if (weaponView != null && armHook != null)
        {
            weaponView.Attach(armHook, _leftHandConstraint, _rightHandConstraint);
            // weaponTransform.localRotation = Quaternion.identity;
            // weaponTransform.localScale = Vector3.one;
        }
    }

    public void SetAnimationTrigger(string animName)
    {
        if (_animator != null)
        {
            _animator.SetTrigger(animName);
        }
    }

    public void DeathFadeOut(Action onComplete)
    {
        Debug.Log("Fade this bitch out");
        if (onComplete != null)
        {
            onComplete();
        }
    }

    public void PlayFootPuffFx()
    {
        if(!_leftFootPuffFx.isPlaying)
        {
            _leftFootPuffFx.Clear();
            _leftFootPuffFx.Play();
        }
        else
        {
            _rightFootPuffFx.Clear();
            _rightFootPuffFx.Play();
        }
    }
    
    public void RequestMovement(PassengerMovement movement)
    {
        if (controller != null)
        {
            controller.Move(movement.velocity, movement.isOnPlatform);
        }
    }

    public void OnTimeWarpEnter(float timeScale)
    {
        if (controller != null)
        {
            controller.OnTimeWarpEnter(timeScale);
        }
    }

    public void OnTimeWarpExit()
    {
        if (controller != null)
        {
            controller.OnTimeWarpExit();
        }
    }

    public AttackResult TakeDamage(AttackData attackData)
    {
        if (controller != null)
        {
            return controller.TakeDamage(attackData);
        }

        return default(AttackResult);
    }
    
    public int     health
    {
        get { return controller != null ? controller.health : 0; }
    }
    public bool    isDead 
    { 
        get { return controller != null ? controller.isDead : false; } 
    }
    
    public IAvatarController controller { get; set; }

    public Vector3 GetHealthPosition()
    {
        return _healthPositionHook.position;
    }
}

using System;
using GhostGen;
using UnityEngine;

public class AvatarView : EventDispatcherBehavior, IPlatformPassenger, ITimeWarpTarget, IAttackTarget
{
    public AvatarConstrainer constrainer;
    public Transform armHook;
    public Transform _healthPositionHook;
    public Transform _viewRoot;
    
    [SerializeField]
    private Animator _animator;
    
    public void Aim(Vector3 cursorPosition)
    {
        if (armHook)
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

    public Animator animator
    {
        get { return _animator; }
        set { _animator = value; }
    }
    
    public void SetWeapon(Transform weaponTransform)
    {
        if (weaponTransform && armHook)
        {
            weaponTransform.SetParent(armHook);
            weaponTransform.localPosition = Vector3.zero;
            weaponTransform.localRotation = Quaternion.identity;
            weaponTransform.localScale = Vector3.one;
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

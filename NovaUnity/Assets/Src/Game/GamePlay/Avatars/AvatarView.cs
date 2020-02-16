using UnityEngine;

public class AvatarView : MonoBehaviour, IPlatformPassenger, ITimeWarpTarget, IAttackTarget
{
    public AvatarConstrainer constrainer;
    public Transform armHook;
    public Transform _healthPositionHook;

    public void Aim(Vector3 cursorPosition)
    {
        if (armHook)
        {
            Vector3 delta = (cursorPosition - armHook.position).normalized;
            armHook.rotation = Quaternion.LookRotation(delta, Vector3.up);
        }
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

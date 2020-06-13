using GhostGen;
using UnityEngine;

public interface IAvatarView : IEventDispatcher
{
    void Aim(Vector3 cursorPosition);
    void SetWeapon(IWeaponView weaponView);
    void RequestMovement(PassengerMovement movement);
    
    IAvatarController controller { get; }
}

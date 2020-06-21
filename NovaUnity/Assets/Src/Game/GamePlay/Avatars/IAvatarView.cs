using GhostGen;
using UnityEngine;

public interface IAvatarView : IEventDispatcher
{
    void Aim(Vector3 cursorPosition);
    void SetWeapon(string ownerUUID, IWeaponController weaponController);
    void RequestMovement(PassengerMovement movement);
    
    IAvatarController controller { get; }
}

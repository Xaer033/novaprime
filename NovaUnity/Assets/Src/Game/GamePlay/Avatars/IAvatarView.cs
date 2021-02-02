using GhostGen;
using Mirror;
using UnityEngine;

public interface IAvatarView : IEventDispatcher
{
    void Aim(Vector3 cursorPosition);
    void SetWeapon(string ownerUUID, IWeaponController weaponController);
    
    Transform transform { get; }
    Transform viewRoot { get; }
    GameObject gameObject { get; }
    Transform cameraTarget { get; }
    IAvatarController controller { get; }

    NetworkEntity netEntity { get; }
    NetworkIdentity netIdentity { get; }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.U2D;


[CreateAssetMenu(menuName ="Nova/Gameplay Resources")]
public class GameplayResources : ScriptableObject
{
    public GameplayCamera gameplayCamera;
    public PlayerAvatarView playerAvatar;
    public MachineGunView machineGunView;
    public BulletView bulletView;

    public ProjectileData bulletData;
    public InputActionAsset p1Input;
}

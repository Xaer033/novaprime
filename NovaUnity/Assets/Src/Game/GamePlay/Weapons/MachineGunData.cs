using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Nova/Weapons/MachineGunData")]
public class MachineGunData : ScriptableObject
{
    public LayerMask targetLayerMask;
    public MachineGunView view;
    public ProjectileData projectileData;
    public float fireCooldownSeconds;
}

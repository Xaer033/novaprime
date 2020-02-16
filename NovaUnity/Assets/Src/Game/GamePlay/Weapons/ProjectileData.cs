using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Nova/Projectile Data")]
public class ProjectileData : ScriptableObject
{
    public LayerMask targetMask;
    public DamageType damageType;
    public AttackType attackType;
    public float speed;
    public int damage;
    public float deathTimer;
}

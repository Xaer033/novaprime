using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Nova/Projectile Data")]
public class ProjectileData : ScriptableObject
{
    public ProjectileType type;
    public float speed;
    public int damage;
    public float deathTimer;
}

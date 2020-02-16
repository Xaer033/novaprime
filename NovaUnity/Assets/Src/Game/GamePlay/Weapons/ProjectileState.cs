using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileState
{
    public bool isActive;
    public Vector3 position;
    public Vector3 velocity;
    public float speed;
    public float timeScale;
    public int damage;
    public float deathTimer;

    public ProjectileData data;
}

public enum ProjectileType
{
    NONE = 0,
    BULLET,
    GRENADE
}
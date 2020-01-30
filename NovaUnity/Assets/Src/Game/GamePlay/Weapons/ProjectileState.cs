using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileState
{
    public bool isActive;
    public ProjectileType type;
    public Vector3 position;
    public Vector3 velocity;
    public float speed;
    public float timeScale;
    public int damage;
    public float deathTimer;

    public void SetActive(ProjectileData data)
    {
        isActive = true;
        type = data.type;
        speed = data.speed;
        damage = data.damage;
        deathTimer = data.deathTimer;
    }
}

public enum ProjectileType
{
    NONE = 0,
    BULLET,
    GRENADE
}
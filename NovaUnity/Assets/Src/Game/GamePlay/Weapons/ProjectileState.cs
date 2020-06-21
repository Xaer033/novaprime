using UnityEngine;

public class ProjectileState
{
    public bool isActive;
    public Vector3 position;
    public Vector3 prevPosition;
    public Vector3 velocity;
    public float speed;
    public float timeScale;
    public int damage;
    public float deathTimer;
    public string ownerUUID;

    public ProjectileData data;
}

public enum ProjectileType
{
    NONE = 0,
    BULLET,
    GRENADE
}

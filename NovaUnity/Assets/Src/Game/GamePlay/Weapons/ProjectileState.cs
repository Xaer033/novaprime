using System;
using UnityEngine;

public struct ProjectileState
{

    public uint    netId;
    public bool    isActive;
    public Vector2 position;
    public Vector2 prevPosition;
    public Vector2 velocity;
    public float   angle;
    public float   speed;
    public float   timeScale;
    public int     damage;
    public double  deathTime;
    public string  ownerUUID;

    public ProjectileData data;

    [Serializable]
    public struct NetSnapshot
    {
    
        public uint    netId;
        public Vector2 position;
        public float   angle;
    }
}

public enum ProjectileType
{
    NONE = 0,
    BULLET,
    GRENADE
}

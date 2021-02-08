using System;
using UnityEngine;

[Serializable]
public struct NetPlayerState   
{
    public Vector2 position;
    public Vector2 velocity;
    public Vector2 aimPosition;
    public uint netId;
}

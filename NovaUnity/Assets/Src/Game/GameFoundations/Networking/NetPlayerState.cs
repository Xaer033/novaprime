using System;
using UnityEngine;

[Serializable]
public struct NetPlayerState   
{
    public Vector2 position;
    public Vector2 velocity;
    public Vector2 aimPosition;
    public uint netId;
    public uint sequence;
    public uint ackTick;

    public static NetPlayerState Create(PlayerState pState)
    {
        NetPlayerState result = new NetPlayerState();
        
        result.position = pState.position;
        result.velocity = pState.velocity;
        result.aimPosition = pState.aimPosition;

        return result;
    }
}

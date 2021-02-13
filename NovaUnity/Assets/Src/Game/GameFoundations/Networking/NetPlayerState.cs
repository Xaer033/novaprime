using System;
using UnityEngine;

[Serializable]
public struct NetPlayerState   
{
    public Vector2 position;
    public Vector2 velocity;
    public Vector2 aimPosition;
    public uint netId;
    public uint tick;

    public static NetPlayerState Create(PlayerState pState, uint tick, uint netId)
    {
        NetPlayerState result = new NetPlayerState();
        
        result.position = pState.position;
        result.velocity = pState.velocity;
        result.aimPosition = pState.aimPosition;
        result.netId = netId;
        result.tick = tick;

        return result;
    }
}

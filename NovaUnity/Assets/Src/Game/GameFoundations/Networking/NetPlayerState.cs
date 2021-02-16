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
    public uint ackSequence;

    public static NetPlayerState Create(PlayerState pState, uint netId, uint ackSequence)
    {
        NetPlayerState result = new NetPlayerState();
        
        result.position = pState.position;
        result.velocity = pState.velocity;
        result.aimPosition = pState.aimPosition;
        result.netId = netId;
        result.ackSequence = ackSequence;

        return result;
    }
}

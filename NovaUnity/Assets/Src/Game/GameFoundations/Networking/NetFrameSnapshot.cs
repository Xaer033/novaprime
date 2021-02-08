using System;
using Mirror;

[Serializable]
public struct NetFrameSnapshot : NetworkMessage
{
    public uint frameTick;
    public double timestamp;
    public int playerCount;
    public NetPlayerState[] playerStateList;
}

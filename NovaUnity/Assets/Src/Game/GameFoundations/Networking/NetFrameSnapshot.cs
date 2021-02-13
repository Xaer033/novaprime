using System;
using Mirror;

[Serializable]
public struct NetFrameSnapshot : NetworkMessage
{
    public NetChannelHeader header;
    public uint ackBitfield;
    
    public uint frameTick;
    public double sendTime;
    public int playerCount;
    public NetPlayerState[] playerStateList;
}

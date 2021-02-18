using System;
using System.Collections.Generic;
using Mirror;

[Serializable]
public struct NetFrameSnapshot : NetworkMessage
{
    public NetChannelHeader header;
    public uint ackBitfield;
    
    public uint frameTick;
    public double sendTime;
    
    public List<NetPlayerState> playerStateList;
}

using System;
using System.Collections.Generic;
using Mirage;

[Serializable]
[NetworkMessage]
public struct NetFrameSnapshot
{
    public NetChannelHeader header;
    public uint ackBitfield;
    
    public uint frameTick;
    public double sendTime;
    
    public List<NetPlayerState> playerStateList;
}

using System.Collections.Generic;
using Mirage;

[NetworkMessage]
public struct SendPlayerInput 
{
    public NetChannelHeader header;
    
    public uint frameTick;
    public double sentTime;
    // public FrameInput input;

    public List<PlayerInputTickPair> inputList;
}

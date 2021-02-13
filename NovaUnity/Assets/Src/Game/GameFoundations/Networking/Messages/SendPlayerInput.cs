using System.Collections.Generic;
using Mirror;

public struct SendPlayerInput : NetworkMessage
{
    public NetChannelHeader header;
    
    public uint frameTick;
    public double sentTime;
    // public FrameInput input;

    public uint inputCount;
    public List<PlayerInputTickPair> inputList;
}

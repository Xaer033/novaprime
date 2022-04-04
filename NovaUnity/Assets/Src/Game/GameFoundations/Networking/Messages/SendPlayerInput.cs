using System.Collections.Generic;
public struct SendPlayerInput
{
    public NetChannelHeader header;
    public List<PlayerInputTickPair> inputList;
}

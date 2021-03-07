using System.Collections.Generic;
using Mirror;

public struct SendPlayerInput : NetworkMessage
{
    public NetChannelHeader header;
    public List<PlayerInputTickPair> inputList;
}

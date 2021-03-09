using System;
using System.Collections.Generic;
using Mirror;

[Serializable]
public struct NetFrameSnapshot : NetworkMessage
{
    public NetChannelHeader header;
    public GameState.Snapshot snapshot;
}

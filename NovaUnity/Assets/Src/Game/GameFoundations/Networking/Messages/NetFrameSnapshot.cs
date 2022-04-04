using System;

[Serializable]
public struct NetFrameSnapshot
{
    public NetChannelHeader header;
    public GameState.Snapshot snapshot;
}

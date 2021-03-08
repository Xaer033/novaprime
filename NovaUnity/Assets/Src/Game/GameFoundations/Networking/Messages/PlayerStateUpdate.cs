using Mirror;

public struct PlayerStateUpdate : NetworkMessage
{
    public uint netId;

    public NetChannelHeader    header;
    public PlayerStateSnapshot snapshot;
}

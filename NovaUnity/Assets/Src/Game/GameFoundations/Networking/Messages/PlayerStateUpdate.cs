
public struct PlayerStateUpdate 
{
    public uint netId;

    public NetChannelHeader    header;
    public PlayerInputStateSnapshot playerInputStateSnapshot;
}

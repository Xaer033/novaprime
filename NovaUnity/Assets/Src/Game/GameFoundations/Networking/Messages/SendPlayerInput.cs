using Mirror;

public struct SendPlayerInput : NetworkMessage
{
    public uint frameTick;
    public double sentTime;
    public FrameInput input;
}

using Fusion;
public struct NetPlayer
{
    public PlayerRef playerRef;
    public PlayerSlot playerSlot;
    public string nickName;
    public bool isReadyUp;
    public bool isMatchReady;
}

public enum PlayerSlot 
{
    NONE = -1,
    P1,
    P2,
    P3,
    P4, 
    MAX_PLAYERS
}

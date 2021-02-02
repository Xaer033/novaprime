public struct NetPlayer
{
    public int connectionId;
    public PlayerSlot playerSlot;
    public string nickName;
    public bool isReadyUp;
    public bool isMatchReady;
    
    public NetPlayer(
                int connId, 
                PlayerSlot pSlot = PlayerSlot.NONE, 
                string name = "", 
                bool ready = false, 
                bool matchReady = false)
    {
        connectionId = connId;
        playerSlot = pSlot;
        nickName = name;
        isReadyUp = ready;
        isMatchReady = matchReady;
    }
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

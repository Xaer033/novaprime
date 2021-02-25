using System;
using Mirage;

public struct NetPlayer
{
    public INetworkConnection connection { get; private set; }
    
    public PlayerSlot playerSlot;
    public string nickName;
    public bool isReadyUp;
    public bool isMatchReady;
    
    public NetPlayer(
                INetworkConnection conn, 
                PlayerSlot pSlot = PlayerSlot.NONE, 
                string name = "", 
                bool ready = false, 
                bool matchReady = false)
    {
        connection = conn;
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

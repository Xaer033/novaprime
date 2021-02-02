using Mirror;

public struct ConfirmReadyUp : NetworkMessage
{
    public PlayerSlot playerSlot;
    public bool isReady;
    public bool allPlayersReady;
    public ConfirmReadyUp(PlayerSlot pSlot, bool ready, bool arePlayersAreReady)
    {
        playerSlot = pSlot;
        isReady = ready;
        allPlayersReady = arePlayersAreReady;
    }
}

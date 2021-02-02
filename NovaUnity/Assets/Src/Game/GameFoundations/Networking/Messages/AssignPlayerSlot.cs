using Mirror;

public struct AssignPlayerSlot : NetworkMessage
{
    public PlayerSlot playerSlot;

    public AssignPlayerSlot(PlayerSlot pSlot)
    {
        playerSlot = pSlot;
    }
}

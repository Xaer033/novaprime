using Mirage;

[NetworkMessage]
public struct AssignPlayerSlot
{
    public PlayerSlot playerSlot;

    public AssignPlayerSlot(PlayerSlot pSlot)
    {
        playerSlot = pSlot;
    }
}

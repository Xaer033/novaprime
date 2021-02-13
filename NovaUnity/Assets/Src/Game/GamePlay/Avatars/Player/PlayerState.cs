using CircularBuffer;
using UnityEngine;

public class PlayerState : AvatarState
{
    public const int MAX_INPUTS = 32;
    
    public static PlayerState Create(string uuid,  UnitStats stats, Vector3 position)
    {
        PlayerState state = new PlayerState();
        state.playerSlot = PlayerSlot.NONE;
        state.position = position;
        state.previousPosition = position;
        state.health = stats.maxHealth;
        state.velocity = Vector3.zero;
        state.timeScale = 1.0f;
        state.machineGunState = new MachineGunState();
        state.stateType = PlayerActivityType.NONE;
        state.uuid = uuid;
        state.stats = stats;
        
        return state;
    }

    public PlayerSlot playerSlot;
    public PlayerActivityType stateType;

    public uint lastAck;
    public RingBuffer<PlayerInputTickPair> nonAckInputBuffer = new RingBuffer<PlayerInputTickPair>(MAX_INPUTS);
    public RingBuffer<NetPlayerState> nonAckStateBuffer = new RingBuffer<NetPlayerState>(MAX_INPUTS);
    
    public UnitStats stats;
    public MachineGunState machineGunState;
}

public enum PlayerActivityType
{
    NONE = 0,
    ACTIVE,
    DEAD
}

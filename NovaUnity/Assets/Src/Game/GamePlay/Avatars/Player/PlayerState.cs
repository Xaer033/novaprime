using UnityEngine;

public class PlayerState : AvatarState
{
    public const int MAX_INPUTS = 32;
    
    public static PlayerState Create(string uuid,  UnitStats stats, Vector2 position)
    {
        PlayerState state = new PlayerState();
        state.playerSlot = PlayerSlot.NONE;
        state.position = position;
        state.previousPosition = position;
        state.health = stats.maxHealth;
        state.velocity = Vector2.zero;
        state.timeScale = 1.0f;
        state.machineGunState = new MachineGunState();
        state.stateType = PlayerActivityType.NONE;
        state.uuid = uuid;
        state.stats = stats;
        
        return state;
    }

    public PlayerStateSnapshot Snapshot()
    {
        return new PlayerStateSnapshot(this);
    }

    public void SetFromSnapshot(PlayerStateSnapshot snapshot)
    {
        position             = snapshot.position;
        previousPosition     = snapshot.previousPosition;
        velocity             = snapshot.velocity;
        health               = snapshot.health;
        timeScale            = snapshot.timeScale;
        aimPosition          = snapshot.aimPosition;
        jumpCount            = snapshot.jumpCount;
        timeToWallUnstick    = snapshot.timeToWallUnstick;
        wallSlideVelocity    = snapshot.wallSlideVelocity;
        isWallSliding        = snapshot.isWallSliding;
        midairJumpTimer      = snapshot.midairJumpTimer;
        coyoteJumpTimer      = snapshot.coyoteJumpTimer;
        velocityXSmoothing   = snapshot.velocityXSmoothing;
        stateType            = snapshot.stateType;
    }

    public PlayerSlot playerSlot;
    public PlayerActivityType stateType;

    
    public uint sequence;
    public uint ackSequence;
    public PlayerInputTickPair latestInput;
    public RingBuffer<PlayerInputTickPair> nonAckInputBuffer = new RingBuffer<PlayerInputTickPair>(MAX_INPUTS);
    public RingBuffer<PlayerStateSnapshot> nonAckStateBuffer = new RingBuffer<PlayerStateSnapshot>(MAX_INPUTS);
    
    public UnitStats stats;
    public MachineGunState machineGunState;
}

public enum PlayerActivityType
{
    NONE = 0,
    ACTIVE,
    DEAD
}

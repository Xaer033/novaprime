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

    public PlayerStateSnapshot Snapshot()
    {
        PlayerStateSnapshot result = new PlayerStateSnapshot();
        
        result.position             = position;
        result.previousPosition     = previousPosition;
        result.velocity             = velocity;
        result.health               = health;
        result.timeScale            = timeScale;
        result.aimPosition          = aimPosition;
        result.jumpCount            = jumpCount;
        result.timeToWallUnstick    = timeToWallUnstick;
        result.wallSlideVelocity    = wallSlideVelocity;
        result.isWallSliding        = isWallSliding;
        result.midairJumpTimer      = midairJumpTimer;
        result.coyoteJumpTimer      = coyoteJumpTimer;
        result.velocityXSmoothing   = velocityXSmoothing;
        result.sequence             = sequence;
        result.ackSequence          = ackSequence;
        result.latestInput          = latestInput;
        result.stateType            = stateType;

        return result;
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
        sequence             = snapshot.sequence;
        ackSequence          = snapshot.ackSequence;
        latestInput          = snapshot.latestInput;
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

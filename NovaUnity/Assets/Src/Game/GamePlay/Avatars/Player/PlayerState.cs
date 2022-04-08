using System;
using Fusion;
using UnityEngine;


public struct Pstate : INetworkStruct
{
    
}
public class PlayerState : AvatarState
{
    public const int MAX_PLAYERS = 16;
    public const int MAX_INPUTS = 128;
    
    public PlayerSlot          playerSlot;
    public PlayerActivityType  stateType;
    public uint                sequence;
    public uint                ackSequence;
    public PlayerInputTickPair latestInput;
    public UnitStats           stats;
    public MachineGunState     machineGunState;
    
    // public RingBuffer<PlayerInputTickPair> nonAckInputBuffer    = new RingBuffer<PlayerInputTickPair>(MAX_INPUTS);
    // public RingBuffer<PlayerStateSnapshot> nonAckStateBuffer    = new RingBuffer<PlayerStateSnapshot>(MAX_INPUTS);
    public PlayerInputStateSnapshot[]      nonAckSnapshotBuffer = new PlayerInputStateSnapshot[MAX_INPUTS];
    
    
    public static PlayerState Create(string uuid,  UnitStats stats, Vector2 position)
    {
        PlayerState state = new PlayerState();
        state.playerSlot = PlayerSlot.NONE;
        state.position = position;
        state.prevPosition = position;
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
        position           = snapshot.position;
        prevPosition       = snapshot.prevPosition;
        velocity           = snapshot.velocity;
        health             = snapshot.health;
        timeScale          = snapshot.timeScale;
        aimPosition        = snapshot.aimPosition;
        jumpCount          = snapshot.jumpCount;
        timeToWallUnstick  = snapshot.timeToWallUnstick;
        wallSlideVelocity  = snapshot.wallSlideVelocity;
        isWallSliding      = snapshot.isWallSliding;
        midairJumpTimer    = snapshot.midairJumpTimer;
        coyoteJumpTimer    = snapshot.coyoteJumpTimer;
        velocityXSmoothing = snapshot.velocityXSmoothing;
        stateType          = snapshot.stateType;
    }

    [Serializable]
    public struct NetSnapshot
    {
        public uint    netId;
        public Vector2 position;
        public Vector2 aimPosition;
    }
}

public enum PlayerActivityType
{
    NONE = 0,
    ACTIVE,
    DEAD
}

using UnityEngine;

public struct PlayerStateSnapshot 
{
    public Vector3 position;
    public Vector3 previousPosition;
    public Vector3 velocity;
    public float health;
    public float timeScale;
    public Vector3 aimPosition;
    
    public int jumpCount;
    public float timeToWallUnstick;
    public float wallSlideVelocity;
    public bool  isWallSliding;
    public float midairJumpTimer;
    public float coyoteJumpTimer;
    public float velocityXSmoothing;
    
    public uint sequence;
    public uint ackSequence;
    
    public PlayerInputTickPair latestInput;
    public PlayerActivityType stateType;
}

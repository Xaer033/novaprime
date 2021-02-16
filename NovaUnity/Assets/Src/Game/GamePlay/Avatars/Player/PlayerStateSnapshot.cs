using UnityEngine;

public struct PlayerStateSnapshot 
{
    public Vector2 position;
    public Vector2 previousPosition;
    public Vector2 velocity;
    public Vector2 aimPosition;
    
    public float health;
    public float timeScale;
    
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

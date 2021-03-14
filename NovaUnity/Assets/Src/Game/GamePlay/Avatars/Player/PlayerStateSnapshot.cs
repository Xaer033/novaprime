using UnityEngine;

public struct PlayerInputStateSnapshot
{
    public PlayerStateSnapshot snapshot;
    public FrameInput          input;
}

public struct PlayerStateSnapshot 
{
    public readonly Vector2 position;
    public readonly Vector2 prevPosition;
    public readonly Vector2 velocity;
    public readonly Vector2 aimPosition;
    
    public readonly float   health;
    public readonly float   timeScale;
    public readonly int     jumpCount;
    public readonly float   timeToWallUnstick;
    public readonly float   wallSlideVelocity;
    public readonly bool    isWallSliding;
    public readonly float   midairJumpTimer;
    public readonly float   coyoteJumpTimer;
    public readonly float   velocityXSmoothing;
    
    public readonly PlayerActivityType stateType;

    public PlayerStateSnapshot(PlayerState stateToCopy)
    {
        position           = stateToCopy.position;
        prevPosition       = stateToCopy.prevPosition;
        velocity           = stateToCopy.velocity;
        health             = stateToCopy.health;
        timeScale          = stateToCopy.timeScale;
        aimPosition        = stateToCopy.aimPosition;
        jumpCount          = stateToCopy.jumpCount;
        timeToWallUnstick  = stateToCopy.timeToWallUnstick;
        wallSlideVelocity  = stateToCopy.wallSlideVelocity;
        isWallSliding      = stateToCopy.isWallSliding;
        midairJumpTimer    = stateToCopy.midairJumpTimer;
        coyoteJumpTimer    = stateToCopy.coyoteJumpTimer;
        velocityXSmoothing = stateToCopy.velocityXSmoothing;
        stateType          = stateToCopy.stateType;
    }
}

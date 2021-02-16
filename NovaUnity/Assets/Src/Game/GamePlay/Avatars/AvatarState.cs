using UnityEngine;

public class AvatarState 
{
    public Vector2 position;
    public Vector2 previousPosition;
    public Vector2 velocity;
    public Vector2 aimPosition;
    public string uuid;
    public float health;
    public float timeScale;
    
    public int jumpCount;
    public float timeToWallUnstick;
    public float wallSlideVelocity;
    public bool  isWallSliding;
    public float midairJumpTimer;
    public float coyoteJumpTimer;
    public float velocityXSmoothing;
}

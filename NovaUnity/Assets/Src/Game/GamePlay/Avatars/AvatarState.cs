using UnityEngine;

public class AvatarState 
{
    public Vector3 position;
    public Vector3 previousPosition;
    public Vector3 velocity;
    public string uuid;
    public int health;
    public float timeScale;
    
    public int jumpCount;
    public float timeToWallUnstick;
    public float wallSlideVelocity;
    public bool  isWallSliding;
    public float midairJumpTimer;
    public float coyoteJumpTimer;
    public float velocityXSmoothing;
}

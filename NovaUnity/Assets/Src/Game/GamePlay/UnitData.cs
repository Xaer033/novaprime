using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Nova/Ground Unit Data")]
public class UnitData : ScriptableObject
{
    public float maxJumpHeight = 2.5f;
    public float minJumpHeight = 0.28f;
    public float timeToJumpApex = 0.4f;
    public float coyoteTime = 0.1f;
    public float jumpRememberDelay = 0.0895f;

    public float wallStickTime = 0.2f;
    public float wallSlideSpeedMax = 0.0f;
    public float wallSlideSpeedDampTime = 0.0f;
    
    public Vector2 wallJumpClimb;
    public Vector2 wallJumpOff;
    public Vector2 wallJumpLeap;
    
    public float speed = 4.5f;
    public float terminalVelocity = 15.0f;
    public float accelerationTimeAir = 0.15f;
    public float accelerationTimeDoubleJumpAir = 0.375f;
    public float accelerationTimeGround = 0.05f;

    public LayerMask targetLayerMask;
    public MachineGunData machineGunData;
}

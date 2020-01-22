using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAvatarView : MonoBehaviour, IPlatformPassenger
{
    public LayerMask collisionMask;
    public float distanceBetweenRays = 0.2f;

    public float maxJumpHeight = 4;
    public float minJumpHeight = 1.0f;
    public float timeToJumpApex = 0.4f;

    public float wallStickTime = .15f;
    
    public Vector2 wallJumpClimb;
    public Vector2 wallJumpOff;
    public Vector2 wallJumpLeap;
    
    public float speed;
    public float accelerationTimeAir = 0.2f;
    public float accelerationTimeGround = 0.1f;
    public float maxSlopeAngle = 80.0f;
    public float wallSlideSpeedMax = 3.0f;
    
    public Collider collider;

    public void RequestMovement(PassengerMovement movement)
    {
        if (controller != null)
        {
            controller.Move(movement.velocity, movement.isOnPlatform);
        }
    }
    
    public PlayerController controller { get; set; }
}

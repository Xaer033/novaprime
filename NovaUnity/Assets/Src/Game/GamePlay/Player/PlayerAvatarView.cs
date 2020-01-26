using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlayerAvatarView : MonoBehaviour, IPlatformPassenger, ITimeWarpTarget
{
    public float maxJumpHeight = 4;
    public float minJumpHeight = 1.0f;
    public float timeToJumpApex = 0.4f;
    public float coyoteTime = 0.15f;
    public float jumpRememberDelay = .2f;

    public float wallStickTime = .15f;
    public float wallSlideSpeedMax = 3.0f;
    public float wallSlideSpeedDampTime = 0.1f;
    
    public Vector2 wallJumpClimb;
    public Vector2 wallJumpOff;
    public Vector2 wallJumpLeap;
    
    public float speed;
    public float terminalVelocity = 20.0f;
    public float accelerationTimeAir = 0.2f;
    public float accelerationTimeGround = 0.1f;

    public AvatarConstrainer constrainer;

     
    public Transform armHook;
    

    public void Aim(Vector3 cursorPosition)
    {
        if (armHook)
        {
            Vector3 delta = (cursorPosition - armHook.position).normalized;
            armHook.rotation = Quaternion.LookRotation(delta, Vector3.up);
        }
    }

    public void SetWeapon(Transform weaponTransform)
    {
        if (weaponTransform && armHook)
        {
            weaponTransform.SetParent(armHook);
            weaponTransform.localPosition = Vector3.zero;
            weaponTransform.localRotation = Quaternion.identity;
            weaponTransform.localScale = Vector3.one;
        }
    }
    
    public void RequestMovement(PassengerMovement movement)
    {
        if (controller != null)
        {
            controller.Move(movement.velocity, movement.isOnPlatform);
        }
    }

    public void OnTimeWarpEnter(float timeScale)
    {
        if (controller != null)
        {
            controller.OnTimeWarpEnter(timeScale);
        }
    }

    public void OnTimeWarpExit()
    {
        if (controller != null)
        {
            controller.OnTimeWarpExit();
        }
    }
    public PlayerController controller { get; set; }
}

using System.Collections;
using System.Collections.Generic;
using Bolt;
using UdpKit;
using UnityEngine;

public struct FrameInput : IProtocolToken
{
    // Range of -1 to 1
    public float horizontalMovement;
    public float verticalMovement;
    
    public bool downPressed;
    public bool downReleased;
    
    public bool jumpPressed;
    public bool jumpReleased;

    public bool interactPressed;
    
    public bool primaryFire;
    public bool secondaryFire;
    public Vector3 cursorPosition;
    public Vector3 cursorDirection;
    public bool useCusorPosition;

    public void Write(UdpPacket packet)
    {
        
    }
    
    public void Read(UdpPacket packet)
    {
        
    }

}

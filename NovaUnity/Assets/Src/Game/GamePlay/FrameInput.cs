﻿using UnityEngine;

public struct FrameInput 
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
    public bool useCusorPosition;
    
    public Vector3 cursorPosition;
    public Vector3 cursorDirection;
}

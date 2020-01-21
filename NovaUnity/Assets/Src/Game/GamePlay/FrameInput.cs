using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct FrameInput
{
    // Range of -1 to 1
    public float horizontalMovement;
    public bool jumpPressed;
    public bool jumpReleased;
    public bool primaryFire;
    public bool secondaryFire;
    public Vector3 cursorPosition;
}

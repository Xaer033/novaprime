using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerInput
{
    public static FrameInput GetInput(int playerNumber)
    {
        FrameInput input = new FrameInput();
        input.horizontalMovement = Input.GetAxis("playerHorizontal");
        input.jump = Input.GetKey(KeyCode.Space);
        input.primaryFire = Input.GetMouseButton(0);
        input.secondaryFire = Input.GetMouseButton(1);
        
        return input;
    }
}

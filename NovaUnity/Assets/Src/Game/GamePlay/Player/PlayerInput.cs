using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerInput
{
    public int playerNumber { get; set; }

    private bool _jumpPressed;
    private bool _jumpRelease;
    
    public PlayerInput(int pNumber)
    {
        playerNumber = pNumber;
    }
    public FrameInput GetInput()
    {
        _jumpPressed = !_jumpPressed ? Input.GetKeyDown(KeyCode.Space) : _jumpPressed;
        _jumpRelease = !_jumpRelease ? Input.GetKeyUp(KeyCode.Space) : _jumpRelease;
        
        FrameInput input = new FrameInput();
        input.horizontalMovement = Input.GetAxis("playerHorizontal");
        input.verticalMovement = Input.GetAxis("playerVertical");
        input.jumpPressed = _jumpPressed;
        input.jumpReleased = _jumpRelease;
        input.primaryFire = Input.GetMouseButton(0);
        input.secondaryFire = Input.GetMouseButton(1);
        
        return input;
    }

    public void Clear()
    {
        _jumpPressed = false;
        _jumpRelease = false;
    }
}

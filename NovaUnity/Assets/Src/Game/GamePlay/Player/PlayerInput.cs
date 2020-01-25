using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerInput
{
    public int playerNumber { get; set; }

    private GameplayCamera _camera;
    private bool _jumpPressed;
    private bool _jumpRelease;
    
    
    private bool _downPressed;
    private bool _downRelease;
    
    public PlayerInput(int pNumber, GameplayCamera playerCamera)
    {
        playerNumber = pNumber;
        _camera = playerCamera;
    }
    
    public FrameInput GetInput()
    {
        _jumpPressed = !_jumpPressed ? Input.GetKeyDown(KeyCode.Space) : _jumpPressed;
        _jumpRelease = !_jumpRelease ? Input.GetKeyUp(KeyCode.Space) : _jumpRelease;
        
        _downPressed = !_downPressed ? Input.GetKeyDown(KeyCode.S)   : _downPressed;
        _downRelease = !_downRelease ? Input.GetKeyUp(KeyCode.S)     : _downRelease;
        
        
        FrameInput input = new FrameInput();
        input.horizontalMovement = Input.GetAxis("playerHorizontal");
        input.verticalMovement = Input.GetAxis("playerVertical");
        
        input.jumpPressed = _jumpPressed;
        input.jumpReleased = _jumpRelease;
        
        input.downPressed = _downPressed;
        input.downReleased = _downRelease;
        
        input.primaryFire = Input.GetMouseButton(0);
        input.secondaryFire = Input.GetMouseButton(1);
        
        input.cursorPosition = _camera.camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Mathf.Abs(_camera.transform.position.z)));
        input.cursorPosition.z = 0;
        
        return input;
    }

    public void Clear()
    {
        _jumpPressed = false;
        _jumpRelease = false;
        
        _downPressed = false;
        _downRelease = false;
    }
}

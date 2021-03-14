using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputGenerator : IInputGenerator
{
    public PlayerSlot playerSlot { get; set; }

    private Camera _camera;
    private bool _jumpPressed;
    private bool _jumpRelease;

    private bool _interactPressed;
    
    private Vector3 _lastAimDirection;
    private bool _lastJumpAxis;
    
    private PlayerActions _pAction;
    private bool _useGamePad;
    
    public PlayerInputGenerator(PlayerSlot pSlot, Camera playerCamera)
    {
        playerSlot = pSlot;
        _camera = playerCamera;
        
        _pAction = new PlayerActions();
        int count = InputSystem.devices.Count;
        for(int i = 0; i < count; ++i)
        {
            InputDevice device = InputSystem.devices[i];
            Debug.Log("Input: " + device.deviceId + ": " + device.description.ToJson());
        }
        _pAction.Gameplay.Enable();
    }
    
    public FrameInput GetInput()
    {
        float horizontalMovement = _pAction.Gameplay.horizontalMovement.ReadValue<float>();// gamepad.leftStick.ReadValue();
        float verticalMovement = _pAction.Gameplay.verticalMovement.ReadValue<float>();// gamepad.leftStick.ReadValue();

        Vector2 aimStick = _pAction.Gameplay.aimDirection.ReadValue<Vector2>();
        Vector2 aimPosition = _pAction.Gameplay.aimAbsolute.ReadValue<Vector2>();//gamepad.rightStick.ReadValue();
        

        bool interactPressed = _pAction.Gameplay.action.triggered;
        _interactPressed = !_interactPressed ? interactPressed : _interactPressed;

        bool jumpAction = _pAction.Gameplay.jump.ReadValue<float>() > 0.01f;
        bool jumpAxisPressed = jumpAction && jumpAction != _lastJumpAxis;
        bool jumpAxisReleased = !jumpAction && jumpAction != _lastJumpAxis;

        _jumpPressed = !_jumpPressed ? jumpAxisPressed    : _jumpPressed;
        _jumpRelease = !_jumpRelease ? jumpAxisReleased   : _jumpRelease;
        _lastJumpAxis = jumpAction;

        if (_pAction.Gameplay.forcePad.triggered)
        {
            _useGamePad = !_useGamePad;
        }

        Vector2 direction = aimStick;// new Vector3(aimStick.x, aimStick.y, 0);

        if (direction.sqrMagnitude >= 0.1f)
        {
            _lastAimDirection = direction;
        }
        
        
        FrameInput input = new FrameInput();
        input.horizontalMovement = horizontalMovement;
        input.verticalMovement = verticalMovement;
        
        input.interactPressed = _interactPressed;
        
        input.jumpPressed = _jumpPressed;
        input.jumpReleased = _jumpRelease;
        
        input.primaryFire = _pAction.Gameplay.primaryFire.ReadValue<float>() > 0.01f;
        input.secondaryFire = Mouse.current.rightButton.isPressed;

        Vector3 worldPosition = new Vector3(aimPosition.x, aimPosition.y, Mathf.Abs(_camera.transform.position.z));
        input.cursorPosition = _camera != null ? _camera.ScreenToWorldPoint(worldPosition) : Vector3.zero;

        input.useCusorPosition =  !_useGamePad;
        input.cursorDirection = _lastAimDirection;
        return input;
    }

    public void Clear()
    {
        _jumpPressed = false;
        _jumpRelease = false;
        _interactPressed = false;
    }
}

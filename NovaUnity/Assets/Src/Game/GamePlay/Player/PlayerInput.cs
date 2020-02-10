using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class PlayerInput : IInputGenerator
{
    public int playerNumber { get; set; }

    private Camera _camera;
    private bool _jumpPressed;
    private bool _jumpRelease;
    
    private Vector3 _lastAimDirection;
    private bool _lastJumpAxis;
    
    private PlayerActions _pAction;
    private bool _useGamePad;
    
    public PlayerInput(int pNumber, Camera playerCamera)
    {
        playerNumber = pNumber;
        _camera = playerCamera;

        
        _pAction = new PlayerActions();
        int count = InputSystem.devices.Count;
//        _pAction.devices = new ReadOnlyArray<InputDevice>(InputSystem.devices.ToArray(), 4, 1);
        _pAction.Gameplay.Enable();
    }
    
    public FrameInput GetInput()
    {
//       pAction.FindAction("movement").ReadValue<>().movement;
        float horizontalMovement = _pAction.Gameplay.horizontalMovement.ReadValue<float>();// gamepad.leftStick.ReadValue();
        float verticalMovement = _pAction.Gameplay.verticalMovement.ReadValue<float>();// gamepad.leftStick.ReadValue();

        Vector2 aimStick = _pAction.Gameplay.aimDirection.ReadValue<Vector2>();
        Vector2 aimPosition = _pAction.Gameplay.aimAbsolute.ReadValue<Vector2>();//gamepad.rightStick.ReadValue();
//        pAction = Singleton.instance.gameplayResources.p1Input;
        
        FrameInput input = new FrameInput();
        input.horizontalMovement = horizontalMovement;
        input.verticalMovement = verticalMovement;

        bool jumpAction = _pAction.Gameplay.jump.ReadValue<float>() > 0.01f;
        bool jumpAxisPressed = jumpAction && jumpAction != _lastJumpAxis;
        bool jumpAxisReleased = !jumpAction && jumpAction != _lastJumpAxis;

        _jumpPressed = !_jumpPressed ? jumpAxisPressed    : _jumpPressed;
        _jumpRelease = !_jumpRelease ? jumpAxisReleased   : _jumpRelease;

        input.jumpPressed = _jumpPressed;
        input.jumpReleased = _jumpRelease;

        _lastJumpAxis = jumpAction;

        input.primaryFire = _pAction.Gameplay.primaryFire.ReadValue<float>() > 0.01f;
        input.secondaryFire = Input.GetMouseButton(1);
        
        input.cursorPosition = _camera != null ? _camera.ScreenToWorldPoint(new Vector3(aimPosition.x, aimPosition.y, Mathf.Abs(_camera.transform.position.z))) : Vector3.zero;
        input.cursorPosition.z = 0;

        if (_pAction.Gameplay.forcePad.triggered)
        {
            _useGamePad = !_useGamePad;
        }
        
        Vector3 direction = new Vector3(aimStick.x, aimStick.y, 0);
        
        input.useCusorPosition =  !_useGamePad;
        
        if (direction.sqrMagnitude >= 0.1f)
        {
            _lastAimDirection = direction;
        }
        
        input.cursorDirection = _lastAimDirection;
        return input;
    }

    public void Clear()
    {
        _jumpPressed = false;
        _jumpRelease = false;
    }
}

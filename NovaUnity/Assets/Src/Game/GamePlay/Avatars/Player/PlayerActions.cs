// GENERATED AUTOMATICALLY FROM 'Assets/GameplayResources/Avatar/Player/PlayerActions.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @PlayerActions : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerActions()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerActions"",
    ""maps"": [
        {
            ""name"": ""Gameplay"",
            ""id"": ""81637846-6df6-46ab-b855-7d4bb0632550"",
            ""actions"": [
                {
                    ""name"": ""primaryFire"",
                    ""type"": ""Value"",
                    ""id"": ""1e5b3e30-6ae5-45d9-8107-768877c65c2a"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""jump"",
                    ""type"": ""Value"",
                    ""id"": ""b7a4321e-b150-40b3-bf73-64749dae1e2f"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""horizontalMovement"",
                    ""type"": ""Value"",
                    ""id"": ""607a4039-78ae-4469-9c16-c8abb334fef7"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": ""NormalizeVector2"",
                    ""interactions"": """"
                },
                {
                    ""name"": ""verticalMovement"",
                    ""type"": ""Value"",
                    ""id"": ""0edc80b8-b88a-4930-8017-3507414797e8"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": ""NormalizeVector2"",
                    ""interactions"": """"
                },
                {
                    ""name"": ""forcePad"",
                    ""type"": ""Button"",
                    ""id"": ""8eb7cd73-d6f8-4c3f-821f-4e0e765a9719"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""aimAbsolute"",
                    ""type"": ""Value"",
                    ""id"": ""37f667c5-08be-4544-bb33-2188f3e4ad8a"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""aimDirection"",
                    ""type"": ""Value"",
                    ""id"": ""1e0c5b10-3b0e-43a1-b55d-aa5e630c4afb"",
                    ""expectedControlType"": ""Stick"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""exit"",
                    ""type"": ""Button"",
                    ""id"": ""80c9640c-df71-402a-9234-283b434bef95"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""reset"",
                    ""type"": ""Button"",
                    ""id"": ""753534df-cdb7-4c91-9f8e-1fb16a6e8d14"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""b922145f-8a50-4109-acf7-0784853c7cca"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""primaryFire"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""92a5d0c4-d143-4624-97bc-897f5849e5ba"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""primaryFire"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""59654f35-d8c7-41ef-bf64-6f647aa63cc8"",
                    ""path"": ""<Gamepad>/leftTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7358f0ba-f644-4e90-aad9-c851eba76514"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f92247d6-bf61-4990-bed4-e3dbeb3c0eba"",
                    ""path"": ""<Gamepad>/leftStick/x"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""horizontalMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""1D Axis"",
                    ""id"": ""33323cad-dba7-44f0-9ff7-1f61ff65956e"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""horizontalMovement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""acbc816b-e0bd-48c6-9cc0-0cd4ada84ce1"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""horizontalMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""14ad9f41-0c56-483f-99f6-da46ab6d20e3"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""horizontalMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""185744af-25e4-4301-9ca9-3c03b8a5aef5"",
                    ""path"": ""<Gamepad>/leftStick/y"",
                    ""interactions"": """",
                    ""processors"": ""AxisDeadzone(min=0.6)"",
                    ""groups"": ""Gamepad"",
                    ""action"": ""verticalMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""1D Axis"",
                    ""id"": ""b497877d-e029-4433-8c4d-e6bcd2339dfc"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""verticalMovement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""0181de2f-1516-46ae-9832-a4446c6bcd57"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""verticalMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""435b0a26-dd2e-4d3f-8be2-3e067d6edf9b"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""verticalMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""edf069ee-6cd1-4861-b1e4-6f083653b322"",
                    ""path"": ""<Gamepad>/rightStickPress"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""forcePad"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""aadd428f-d904-4338-a1b0-7c70afff9a92"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""forcePad"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""03666a13-e9b7-43f4-910c-97fd17e93969"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""aimAbsolute"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1b788965-a9a0-41ba-aa2e-d6bf85ce52b1"",
                    ""path"": ""<Gamepad>/select"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""exit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""fcae5523-f663-4cd3-b8cc-2896aecf1acb"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""exit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""68c7fd17-3efe-4380-903c-dc4f6bbb1a7e"",
                    ""path"": ""<Keyboard>/f1"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""reset"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""137474e8-a7ac-4a2b-8dcf-024ac8cb17bd"",
                    ""path"": ""<Gamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""reset"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f01c000f-0b6d-4dea-9d43-2d908347424d"",
                    ""path"": ""*/{Secondary2DMotion}"",
                    ""interactions"": """",
                    ""processors"": ""StickDeadzone(min=0.25),NormalizeVector2"",
                    ""groups"": ""Gamepad"",
                    ""action"": ""aimDirection"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Menu"",
            ""id"": ""906d759b-6622-4efb-b58a-f728b70252e6"",
            ""actions"": [],
            ""bindings"": []
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Gamepad"",
            ""bindingGroup"": ""Gamepad"",
            ""devices"": [
                {
                    ""devicePath"": ""<Gamepad>"",
                    ""isOptional"": true,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""KeyboardMouse"",
            ""bindingGroup"": ""KeyboardMouse"",
            ""devices"": []
        }
    ]
}");
        // Gameplay
        m_Gameplay = asset.FindActionMap("Gameplay", throwIfNotFound: true);
        m_Gameplay_primaryFire = m_Gameplay.FindAction("primaryFire", throwIfNotFound: true);
        m_Gameplay_jump = m_Gameplay.FindAction("jump", throwIfNotFound: true);
        m_Gameplay_horizontalMovement = m_Gameplay.FindAction("horizontalMovement", throwIfNotFound: true);
        m_Gameplay_verticalMovement = m_Gameplay.FindAction("verticalMovement", throwIfNotFound: true);
        m_Gameplay_forcePad = m_Gameplay.FindAction("forcePad", throwIfNotFound: true);
        m_Gameplay_aimAbsolute = m_Gameplay.FindAction("aimAbsolute", throwIfNotFound: true);
        m_Gameplay_aimDirection = m_Gameplay.FindAction("aimDirection", throwIfNotFound: true);
        m_Gameplay_exit = m_Gameplay.FindAction("exit", throwIfNotFound: true);
        m_Gameplay_reset = m_Gameplay.FindAction("reset", throwIfNotFound: true);
        // Menu
        m_Menu = asset.FindActionMap("Menu", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // Gameplay
    private readonly InputActionMap m_Gameplay;
    private IGameplayActions m_GameplayActionsCallbackInterface;
    private readonly InputAction m_Gameplay_primaryFire;
    private readonly InputAction m_Gameplay_jump;
    private readonly InputAction m_Gameplay_horizontalMovement;
    private readonly InputAction m_Gameplay_verticalMovement;
    private readonly InputAction m_Gameplay_forcePad;
    private readonly InputAction m_Gameplay_aimAbsolute;
    private readonly InputAction m_Gameplay_aimDirection;
    private readonly InputAction m_Gameplay_exit;
    private readonly InputAction m_Gameplay_reset;
    public struct GameplayActions
    {
        private @PlayerActions m_Wrapper;
        public GameplayActions(@PlayerActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @primaryFire => m_Wrapper.m_Gameplay_primaryFire;
        public InputAction @jump => m_Wrapper.m_Gameplay_jump;
        public InputAction @horizontalMovement => m_Wrapper.m_Gameplay_horizontalMovement;
        public InputAction @verticalMovement => m_Wrapper.m_Gameplay_verticalMovement;
        public InputAction @forcePad => m_Wrapper.m_Gameplay_forcePad;
        public InputAction @aimAbsolute => m_Wrapper.m_Gameplay_aimAbsolute;
        public InputAction @aimDirection => m_Wrapper.m_Gameplay_aimDirection;
        public InputAction @exit => m_Wrapper.m_Gameplay_exit;
        public InputAction @reset => m_Wrapper.m_Gameplay_reset;
        public InputActionMap Get() { return m_Wrapper.m_Gameplay; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(GameplayActions set) { return set.Get(); }
        public void SetCallbacks(IGameplayActions instance)
        {
            if (m_Wrapper.m_GameplayActionsCallbackInterface != null)
            {
                @primaryFire.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnPrimaryFire;
                @primaryFire.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnPrimaryFire;
                @primaryFire.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnPrimaryFire;
                @jump.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnJump;
                @jump.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnJump;
                @jump.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnJump;
                @horizontalMovement.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnHorizontalMovement;
                @horizontalMovement.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnHorizontalMovement;
                @horizontalMovement.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnHorizontalMovement;
                @verticalMovement.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnVerticalMovement;
                @verticalMovement.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnVerticalMovement;
                @verticalMovement.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnVerticalMovement;
                @forcePad.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnForcePad;
                @forcePad.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnForcePad;
                @forcePad.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnForcePad;
                @aimAbsolute.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnAimAbsolute;
                @aimAbsolute.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnAimAbsolute;
                @aimAbsolute.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnAimAbsolute;
                @aimDirection.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnAimDirection;
                @aimDirection.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnAimDirection;
                @aimDirection.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnAimDirection;
                @exit.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnExit;
                @exit.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnExit;
                @exit.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnExit;
                @reset.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnReset;
                @reset.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnReset;
                @reset.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnReset;
            }
            m_Wrapper.m_GameplayActionsCallbackInterface = instance;
            if (instance != null)
            {
                @primaryFire.started += instance.OnPrimaryFire;
                @primaryFire.performed += instance.OnPrimaryFire;
                @primaryFire.canceled += instance.OnPrimaryFire;
                @jump.started += instance.OnJump;
                @jump.performed += instance.OnJump;
                @jump.canceled += instance.OnJump;
                @horizontalMovement.started += instance.OnHorizontalMovement;
                @horizontalMovement.performed += instance.OnHorizontalMovement;
                @horizontalMovement.canceled += instance.OnHorizontalMovement;
                @verticalMovement.started += instance.OnVerticalMovement;
                @verticalMovement.performed += instance.OnVerticalMovement;
                @verticalMovement.canceled += instance.OnVerticalMovement;
                @forcePad.started += instance.OnForcePad;
                @forcePad.performed += instance.OnForcePad;
                @forcePad.canceled += instance.OnForcePad;
                @aimAbsolute.started += instance.OnAimAbsolute;
                @aimAbsolute.performed += instance.OnAimAbsolute;
                @aimAbsolute.canceled += instance.OnAimAbsolute;
                @aimDirection.started += instance.OnAimDirection;
                @aimDirection.performed += instance.OnAimDirection;
                @aimDirection.canceled += instance.OnAimDirection;
                @exit.started += instance.OnExit;
                @exit.performed += instance.OnExit;
                @exit.canceled += instance.OnExit;
                @reset.started += instance.OnReset;
                @reset.performed += instance.OnReset;
                @reset.canceled += instance.OnReset;
            }
        }
    }
    public GameplayActions @Gameplay => new GameplayActions(this);

    // Menu
    private readonly InputActionMap m_Menu;
    private IMenuActions m_MenuActionsCallbackInterface;
    public struct MenuActions
    {
        private @PlayerActions m_Wrapper;
        public MenuActions(@PlayerActions wrapper) { m_Wrapper = wrapper; }
        public InputActionMap Get() { return m_Wrapper.m_Menu; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MenuActions set) { return set.Get(); }
        public void SetCallbacks(IMenuActions instance)
        {
            if (m_Wrapper.m_MenuActionsCallbackInterface != null)
            {
            }
            m_Wrapper.m_MenuActionsCallbackInterface = instance;
            if (instance != null)
            {
            }
        }
    }
    public MenuActions @Menu => new MenuActions(this);
    private int m_GamepadSchemeIndex = -1;
    public InputControlScheme GamepadScheme
    {
        get
        {
            if (m_GamepadSchemeIndex == -1) m_GamepadSchemeIndex = asset.FindControlSchemeIndex("Gamepad");
            return asset.controlSchemes[m_GamepadSchemeIndex];
        }
    }
    private int m_KeyboardMouseSchemeIndex = -1;
    public InputControlScheme KeyboardMouseScheme
    {
        get
        {
            if (m_KeyboardMouseSchemeIndex == -1) m_KeyboardMouseSchemeIndex = asset.FindControlSchemeIndex("KeyboardMouse");
            return asset.controlSchemes[m_KeyboardMouseSchemeIndex];
        }
    }
    public interface IGameplayActions
    {
        void OnPrimaryFire(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnHorizontalMovement(InputAction.CallbackContext context);
        void OnVerticalMovement(InputAction.CallbackContext context);
        void OnForcePad(InputAction.CallbackContext context);
        void OnAimAbsolute(InputAction.CallbackContext context);
        void OnAimDirection(InputAction.CallbackContext context);
        void OnExit(InputAction.CallbackContext context);
        void OnReset(InputAction.CallbackContext context);
    }
    public interface IMenuActions
    {
    }
}

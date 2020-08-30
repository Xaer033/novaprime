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
                    ""interactions"": ""Press""
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
                },
                {
                    ""name"": ""action"",
                    ""type"": ""Button"",
                    ""id"": ""6fb0df71-7383-4455-9825-b43df4fd99c1"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press""
                },
                {
                    ""name"": ""possess"",
                    ""type"": ""Button"",
                    ""id"": ""91e5242e-2b3e-452d-a49f-3d47d612a075"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press,Hold""
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
                    ""interactions"": """",
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
                },
                {
                    ""name"": """",
                    ""id"": ""4df7fa91-c913-4b87-ac93-1e1d51537ec4"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""action"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b89db55d-2755-4278-993c-69b0840305b9"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""action"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1285c5be-47b3-4a1d-ba6c-dcf8c460f679"",
                    ""path"": ""<Keyboard>/f"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""possess"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f5a83758-67d7-4fdf-aec0-47c42e973154"",
                    ""path"": ""<Gamepad>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""possess"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Menu"",
            ""id"": ""b84258fc-55b5-46ab-9a34-d4fd1bf709d1"",
            ""actions"": [
                {
                    ""name"": ""point"",
                    ""type"": ""Value"",
                    ""id"": ""e387573a-49de-41f1-8698-8139ce200639"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""left_click"",
                    ""type"": ""Button"",
                    ""id"": ""e14410d9-0193-42de-800c-f0dad5ac98b6"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""middle_click"",
                    ""type"": ""Button"",
                    ""id"": ""1eb7e6c6-d66e-4a40-a34c-ebe852ee6b77"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""right_click"",
                    ""type"": ""Button"",
                    ""id"": ""26cbfb54-9a7a-4cf5-ad22-7ac165c4dd52"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""scroll_wheel"",
                    ""type"": ""Value"",
                    ""id"": ""af270934-de6a-446e-8296-6c4dd5472564"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""move"",
                    ""type"": ""Value"",
                    ""id"": ""74acc224-bc45-438e-af1a-eaf1879d2894"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""submit"",
                    ""type"": ""Button"",
                    ""id"": ""d69f00cf-999d-4e32-97a1-c7406c61a979"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""cancel"",
                    ""type"": ""Button"",
                    ""id"": ""22b5248c-7f32-4b8f-b2c3-be90d1055497"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""ce9f6ae3-71df-4b30-aca3-1177f6179476"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""left_click"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""53e90585-3944-4f92-83fa-69c11a27b053"",
                    ""path"": ""<Mouse>/middleButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""middle_click"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f61e6e0c-7e9c-40d7-866b-aa5282d16e3d"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""right_click"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8adb8975-5043-4650-ab86-a3f59d588a90"",
                    ""path"": ""<Mouse>/scroll/y"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""scroll_wheel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0c485633-1b9c-4712-9409-fe63b2b07fd5"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""submit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9fa9c671-bf1a-4fc6-8e88-da049e69ab25"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""submit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6dfe80e7-2baf-4efb-b73d-48328bdf8714"",
                    ""path"": ""<Keyboard>/enter"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""submit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""07f1a03e-742c-49a8-80fc-04592e608d54"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""cancel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""aacb8044-275a-472d-a06f-9172df8167a0"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""cancel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""d-pad"",
                    ""id"": ""7c4240f6-2361-4319-aa60-427555322c1c"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""510209d3-42e4-493f-9fdd-70746b34acdf"",
                    ""path"": ""<Gamepad>/dpad/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""257f510b-5025-4b45-aaab-fd339544384b"",
                    ""path"": ""<Gamepad>/dpad/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""bc0fdd6f-5645-43e6-8ec5-e8c1de1923b7"",
                    ""path"": ""<Gamepad>/dpad/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""9878cfcc-8e6d-45c5-bf10-e64c3c082a5d"",
                    ""path"": ""<Gamepad>/dpad/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""analog-stick"",
                    ""id"": ""57cd5a91-8d88-4d74-bf6a-30dc277f8e8a"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""fe8f6eeb-b22f-43be-a2ab-76d819808c06"",
                    ""path"": ""<Gamepad>/leftStick/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""77394201-dd7a-4d5d-9fd4-6b94cc174a4c"",
                    ""path"": ""<Gamepad>/leftStick/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""8a575ef6-c863-4b7e-8b06-dbbf44405566"",
                    ""path"": ""<Gamepad>/leftStick/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""614c5d1a-f1ba-41b3-b086-ed51087a298c"",
                    ""path"": ""<Gamepad>/leftStick/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""arrow-keys"",
                    ""id"": ""3e16b839-8c0a-4423-8ab7-54d3cd3ed65e"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""971cd5a8-a05c-4d79-904d-7a8e5789da7b"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""8ae878da-0407-4b51-ad7e-edfcc58d3bf3"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""bb71a1a6-563f-4cd0-a276-f2f4a956f4fd"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""c1223406-f25b-49e2-aa42-e84a15e06331"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""wasd"",
                    ""id"": ""793beb46-810e-4f06-9a69-c4beb309a966"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""1502e8ad-9822-4a4f-98f5-2268977e30ea"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""7cab411a-bf7b-42d4-b895-9658f598eae4"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""e7cc0e19-fe1e-429b-ac24-ca96be492ac0"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""e47d1029-f8ca-4f0c-bca0-6b62ad799759"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
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
        m_Gameplay_action = m_Gameplay.FindAction("action", throwIfNotFound: true);
        m_Gameplay_possess = m_Gameplay.FindAction("possess", throwIfNotFound: true);
        // Menu
        m_Menu = asset.FindActionMap("Menu", throwIfNotFound: true);
        m_Menu_point = m_Menu.FindAction("point", throwIfNotFound: true);
        m_Menu_left_click = m_Menu.FindAction("left_click", throwIfNotFound: true);
        m_Menu_middle_click = m_Menu.FindAction("middle_click", throwIfNotFound: true);
        m_Menu_right_click = m_Menu.FindAction("right_click", throwIfNotFound: true);
        m_Menu_scroll_wheel = m_Menu.FindAction("scroll_wheel", throwIfNotFound: true);
        m_Menu_move = m_Menu.FindAction("move", throwIfNotFound: true);
        m_Menu_submit = m_Menu.FindAction("submit", throwIfNotFound: true);
        m_Menu_cancel = m_Menu.FindAction("cancel", throwIfNotFound: true);
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
    private readonly InputAction m_Gameplay_action;
    private readonly InputAction m_Gameplay_possess;
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
        public InputAction @action => m_Wrapper.m_Gameplay_action;
        public InputAction @possess => m_Wrapper.m_Gameplay_possess;
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
                @action.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnAction;
                @action.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnAction;
                @action.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnAction;
                @possess.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnPossess;
                @possess.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnPossess;
                @possess.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnPossess;
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
                @action.started += instance.OnAction;
                @action.performed += instance.OnAction;
                @action.canceled += instance.OnAction;
                @possess.started += instance.OnPossess;
                @possess.performed += instance.OnPossess;
                @possess.canceled += instance.OnPossess;
            }
        }
    }
    public GameplayActions @Gameplay => new GameplayActions(this);

    // Menu
    private readonly InputActionMap m_Menu;
    private IMenuActions m_MenuActionsCallbackInterface;
    private readonly InputAction m_Menu_point;
    private readonly InputAction m_Menu_left_click;
    private readonly InputAction m_Menu_middle_click;
    private readonly InputAction m_Menu_right_click;
    private readonly InputAction m_Menu_scroll_wheel;
    private readonly InputAction m_Menu_move;
    private readonly InputAction m_Menu_submit;
    private readonly InputAction m_Menu_cancel;
    public struct MenuActions
    {
        private @PlayerActions m_Wrapper;
        public MenuActions(@PlayerActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @point => m_Wrapper.m_Menu_point;
        public InputAction @left_click => m_Wrapper.m_Menu_left_click;
        public InputAction @middle_click => m_Wrapper.m_Menu_middle_click;
        public InputAction @right_click => m_Wrapper.m_Menu_right_click;
        public InputAction @scroll_wheel => m_Wrapper.m_Menu_scroll_wheel;
        public InputAction @move => m_Wrapper.m_Menu_move;
        public InputAction @submit => m_Wrapper.m_Menu_submit;
        public InputAction @cancel => m_Wrapper.m_Menu_cancel;
        public InputActionMap Get() { return m_Wrapper.m_Menu; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MenuActions set) { return set.Get(); }
        public void SetCallbacks(IMenuActions instance)
        {
            if (m_Wrapper.m_MenuActionsCallbackInterface != null)
            {
                @point.started -= m_Wrapper.m_MenuActionsCallbackInterface.OnPoint;
                @point.performed -= m_Wrapper.m_MenuActionsCallbackInterface.OnPoint;
                @point.canceled -= m_Wrapper.m_MenuActionsCallbackInterface.OnPoint;
                @left_click.started -= m_Wrapper.m_MenuActionsCallbackInterface.OnLeft_click;
                @left_click.performed -= m_Wrapper.m_MenuActionsCallbackInterface.OnLeft_click;
                @left_click.canceled -= m_Wrapper.m_MenuActionsCallbackInterface.OnLeft_click;
                @middle_click.started -= m_Wrapper.m_MenuActionsCallbackInterface.OnMiddle_click;
                @middle_click.performed -= m_Wrapper.m_MenuActionsCallbackInterface.OnMiddle_click;
                @middle_click.canceled -= m_Wrapper.m_MenuActionsCallbackInterface.OnMiddle_click;
                @right_click.started -= m_Wrapper.m_MenuActionsCallbackInterface.OnRight_click;
                @right_click.performed -= m_Wrapper.m_MenuActionsCallbackInterface.OnRight_click;
                @right_click.canceled -= m_Wrapper.m_MenuActionsCallbackInterface.OnRight_click;
                @scroll_wheel.started -= m_Wrapper.m_MenuActionsCallbackInterface.OnScroll_wheel;
                @scroll_wheel.performed -= m_Wrapper.m_MenuActionsCallbackInterface.OnScroll_wheel;
                @scroll_wheel.canceled -= m_Wrapper.m_MenuActionsCallbackInterface.OnScroll_wheel;
                @move.started -= m_Wrapper.m_MenuActionsCallbackInterface.OnMove;
                @move.performed -= m_Wrapper.m_MenuActionsCallbackInterface.OnMove;
                @move.canceled -= m_Wrapper.m_MenuActionsCallbackInterface.OnMove;
                @submit.started -= m_Wrapper.m_MenuActionsCallbackInterface.OnSubmit;
                @submit.performed -= m_Wrapper.m_MenuActionsCallbackInterface.OnSubmit;
                @submit.canceled -= m_Wrapper.m_MenuActionsCallbackInterface.OnSubmit;
                @cancel.started -= m_Wrapper.m_MenuActionsCallbackInterface.OnCancel;
                @cancel.performed -= m_Wrapper.m_MenuActionsCallbackInterface.OnCancel;
                @cancel.canceled -= m_Wrapper.m_MenuActionsCallbackInterface.OnCancel;
            }
            m_Wrapper.m_MenuActionsCallbackInterface = instance;
            if (instance != null)
            {
                @point.started += instance.OnPoint;
                @point.performed += instance.OnPoint;
                @point.canceled += instance.OnPoint;
                @left_click.started += instance.OnLeft_click;
                @left_click.performed += instance.OnLeft_click;
                @left_click.canceled += instance.OnLeft_click;
                @middle_click.started += instance.OnMiddle_click;
                @middle_click.performed += instance.OnMiddle_click;
                @middle_click.canceled += instance.OnMiddle_click;
                @right_click.started += instance.OnRight_click;
                @right_click.performed += instance.OnRight_click;
                @right_click.canceled += instance.OnRight_click;
                @scroll_wheel.started += instance.OnScroll_wheel;
                @scroll_wheel.performed += instance.OnScroll_wheel;
                @scroll_wheel.canceled += instance.OnScroll_wheel;
                @move.started += instance.OnMove;
                @move.performed += instance.OnMove;
                @move.canceled += instance.OnMove;
                @submit.started += instance.OnSubmit;
                @submit.performed += instance.OnSubmit;
                @submit.canceled += instance.OnSubmit;
                @cancel.started += instance.OnCancel;
                @cancel.performed += instance.OnCancel;
                @cancel.canceled += instance.OnCancel;
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
        void OnAction(InputAction.CallbackContext context);
        void OnPossess(InputAction.CallbackContext context);
    }
    public interface IMenuActions
    {
        void OnPoint(InputAction.CallbackContext context);
        void OnLeft_click(InputAction.CallbackContext context);
        void OnMiddle_click(InputAction.CallbackContext context);
        void OnRight_click(InputAction.CallbackContext context);
        void OnScroll_wheel(InputAction.CallbackContext context);
        void OnMove(InputAction.CallbackContext context);
        void OnSubmit(InputAction.CallbackContext context);
        void OnCancel(InputAction.CallbackContext context);
    }
}

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
                    ""name"": ""TrackedDeviceOrientation"",
                    ""type"": ""PassThrough"",
                    ""id"": ""52ecc0de-e343-41fa-a0de-2fb01870902a"",
                    ""expectedControlType"": ""Quaternion"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""TrackedDevicePosition"",
                    ""type"": ""PassThrough"",
                    ""id"": ""d9f608a8-b2e5-4db3-9560-5bc6457e61aa"",
                    ""expectedControlType"": ""Vector3"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""RightClick"",
                    ""type"": ""PassThrough"",
                    ""id"": ""887410bd-44f6-4ce3-8c48-cd5bd7817ad2"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""MiddleClick"",
                    ""type"": ""PassThrough"",
                    ""id"": ""4d3a8b5c-7e92-4fa2-b0d2-2eb41fddfb1f"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ScrollWheel"",
                    ""type"": ""PassThrough"",
                    ""id"": ""347225f3-86cb-4ff1-96d4-80ca3c5e69f8"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Click"",
                    ""type"": ""PassThrough"",
                    ""id"": ""3654cf77-9a2a-4c8f-9ae8-70b06d1ebde0"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Point"",
                    ""type"": ""PassThrough"",
                    ""id"": ""0f1807ef-334b-4177-846d-560024e06d1d"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Cancel"",
                    ""type"": ""Button"",
                    ""id"": ""b6443f7e-af6c-407b-a1e7-86eaf61980cd"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Submit"",
                    ""type"": ""Button"",
                    ""id"": ""8ea544c4-c3b0-463a-b363-41a5713f3fe0"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Navigate"",
                    ""type"": ""Value"",
                    ""id"": ""d4b221d6-cdb2-4fb9-8162-b831bc2016d4"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""Gamepad"",
                    ""id"": ""95328237-85f0-4c39-8625-f071c1a5e89f"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Navigate"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""576aa1fd-c3eb-42df-9ef7-12583d5dd193"",
                    ""path"": ""<Gamepad>/leftStick/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Gamepad"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""up"",
                    ""id"": ""d3265840-d29a-407e-919d-707e7b651388"",
                    ""path"": ""<Gamepad>/rightStick/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Gamepad"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""9bac18df-f3b1-45fc-80c5-50beea820cae"",
                    ""path"": ""<Gamepad>/leftStick/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Gamepad"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""3501784a-5438-41e8-abe6-1a926f086d29"",
                    ""path"": ""<Gamepad>/rightStick/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Gamepad"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""36216b91-3a6a-4eb8-83d7-a8e78fcc1a09"",
                    ""path"": ""<Gamepad>/leftStick/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Gamepad"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""cf6d4bd2-90bb-46d3-93f1-1e0cddd74840"",
                    ""path"": ""<Gamepad>/rightStick/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Gamepad"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""b0d7eadb-e5f8-437e-b8d5-6355bf383232"",
                    ""path"": ""<Gamepad>/leftStick/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Gamepad"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""91cdbfd0-47c6-492c-ba33-7f9e20e00ff8"",
                    ""path"": ""<Gamepad>/rightStick/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Gamepad"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""221e4f06-1853-41cd-b0d2-e216cdad7486"",
                    ""path"": ""<Gamepad>/dpad"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Gamepad"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Joystick"",
                    ""id"": ""9d56a986-7a2d-4b51-8e56-a1ede68d2e42"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Navigate"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""486f54fa-8018-489f-8ef5-cfe5ee5c0df2"",
                    ""path"": ""<Joystick>/stick/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Joystick"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""17a1eef5-e8fc-41f1-9510-cdda3952cd7d"",
                    ""path"": ""<Joystick>/stick/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Joystick"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""a981950c-b3c5-4ed2-a559-9cf95e840fb8"",
                    ""path"": ""<Joystick>/stick/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Joystick"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""fd28a119-eed6-4624-afad-a8ea346825bb"",
                    ""path"": ""<Joystick>/stick/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Joystick"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Keyboard"",
                    ""id"": ""6ff3320e-df74-4fca-a08d-2c5cc3dec4e3"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Navigate"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""d6a4e304-bd67-44d1-8db8-474617498300"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""up"",
                    ""id"": ""9f342253-ae50-4ab5-93e7-2c8e57eabbca"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""050674b9-98da-4aee-a22e-92c87fa50a7d"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""1eeabc86-3c2b-41e3-ac33-d78d2b238e81"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""63429e56-f8e7-41ac-86be-4b97e5764681"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""f225ff64-00de-4b16-b2dc-f4ea345db021"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""953fb436-925d-4084-ac4d-0e6eab1674e6"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""42d9f451-394d-45ca-af02-2572f6a40f38"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""9e425ebb-a675-4721-ae4e-0b715b13aadb"",
                    ""path"": ""*/{Submit}"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Submit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b6f80c4c-fbf6-4106-bdf9-5d687d9d713c"",
                    ""path"": ""*/{Cancel}"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Cancel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d7d64a46-f8b0-40d0-8082-6be0eb645855"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""Point"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a14d74c5-b997-4763-8d2a-63cc1c2c95d3"",
                    ""path"": ""<Pen>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""Point"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f4885abf-fe46-4cca-9e72-5f12363bf950"",
                    ""path"": ""<Touchscreen>/touch*/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Touch"",
                    ""action"": ""Point"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1ce2c57e-ec27-433a-b246-769c00cc7042"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Keyboard&Mouse"",
                    ""action"": ""Click"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d3c0d929-b10f-4c57-975c-96bf820fc5cd"",
                    ""path"": ""<Pen>/tip"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Keyboard&Mouse"",
                    ""action"": ""Click"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c150f5d8-8a74-45a0-9c0c-59acb3e8cdfc"",
                    ""path"": ""<Touchscreen>/touch*/press"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Touch"",
                    ""action"": ""Click"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""551ca4f4-9baf-4ab0-a624-362f8d82771f"",
                    ""path"": ""<XRController>/trigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""XR"",
                    ""action"": ""Click"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""29804aff-54e3-40a7-94d6-3d99ef5e7a35"",
                    ""path"": ""<Mouse>/scroll"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Keyboard&Mouse"",
                    ""action"": ""ScrollWheel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3d8873ae-fab9-46d7-9804-c8b377f83ed0"",
                    ""path"": ""<Mouse>/middleButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Keyboard&Mouse"",
                    ""action"": ""MiddleClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b6498793-79bd-4731-8f99-611095d82304"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Keyboard&Mouse"",
                    ""action"": ""RightClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a1bf42e1-1ef6-4f86-876d-e9da21ac05d1"",
                    ""path"": ""<XRController>/devicePosition"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""XR"",
                    ""action"": ""TrackedDevicePosition"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""03118311-b125-4ac2-b3c6-7e817258b4dc"",
                    ""path"": ""<XRController>/deviceRotation"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""XR"",
                    ""action"": ""TrackedDeviceOrientation"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
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
        m_Menu_TrackedDeviceOrientation = m_Menu.FindAction("TrackedDeviceOrientation", throwIfNotFound: true);
        m_Menu_TrackedDevicePosition = m_Menu.FindAction("TrackedDevicePosition", throwIfNotFound: true);
        m_Menu_RightClick = m_Menu.FindAction("RightClick", throwIfNotFound: true);
        m_Menu_MiddleClick = m_Menu.FindAction("MiddleClick", throwIfNotFound: true);
        m_Menu_ScrollWheel = m_Menu.FindAction("ScrollWheel", throwIfNotFound: true);
        m_Menu_Click = m_Menu.FindAction("Click", throwIfNotFound: true);
        m_Menu_Point = m_Menu.FindAction("Point", throwIfNotFound: true);
        m_Menu_Cancel = m_Menu.FindAction("Cancel", throwIfNotFound: true);
        m_Menu_Submit = m_Menu.FindAction("Submit", throwIfNotFound: true);
        m_Menu_Navigate = m_Menu.FindAction("Navigate", throwIfNotFound: true);
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
    private readonly InputAction m_Menu_TrackedDeviceOrientation;
    private readonly InputAction m_Menu_TrackedDevicePosition;
    private readonly InputAction m_Menu_RightClick;
    private readonly InputAction m_Menu_MiddleClick;
    private readonly InputAction m_Menu_ScrollWheel;
    private readonly InputAction m_Menu_Click;
    private readonly InputAction m_Menu_Point;
    private readonly InputAction m_Menu_Cancel;
    private readonly InputAction m_Menu_Submit;
    private readonly InputAction m_Menu_Navigate;
    public struct MenuActions
    {
        private @PlayerActions m_Wrapper;
        public MenuActions(@PlayerActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @TrackedDeviceOrientation => m_Wrapper.m_Menu_TrackedDeviceOrientation;
        public InputAction @TrackedDevicePosition => m_Wrapper.m_Menu_TrackedDevicePosition;
        public InputAction @RightClick => m_Wrapper.m_Menu_RightClick;
        public InputAction @MiddleClick => m_Wrapper.m_Menu_MiddleClick;
        public InputAction @ScrollWheel => m_Wrapper.m_Menu_ScrollWheel;
        public InputAction @Click => m_Wrapper.m_Menu_Click;
        public InputAction @Point => m_Wrapper.m_Menu_Point;
        public InputAction @Cancel => m_Wrapper.m_Menu_Cancel;
        public InputAction @Submit => m_Wrapper.m_Menu_Submit;
        public InputAction @Navigate => m_Wrapper.m_Menu_Navigate;
        public InputActionMap Get() { return m_Wrapper.m_Menu; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MenuActions set) { return set.Get(); }
        public void SetCallbacks(IMenuActions instance)
        {
            if (m_Wrapper.m_MenuActionsCallbackInterface != null)
            {
                @TrackedDeviceOrientation.started -= m_Wrapper.m_MenuActionsCallbackInterface.OnTrackedDeviceOrientation;
                @TrackedDeviceOrientation.performed -= m_Wrapper.m_MenuActionsCallbackInterface.OnTrackedDeviceOrientation;
                @TrackedDeviceOrientation.canceled -= m_Wrapper.m_MenuActionsCallbackInterface.OnTrackedDeviceOrientation;
                @TrackedDevicePosition.started -= m_Wrapper.m_MenuActionsCallbackInterface.OnTrackedDevicePosition;
                @TrackedDevicePosition.performed -= m_Wrapper.m_MenuActionsCallbackInterface.OnTrackedDevicePosition;
                @TrackedDevicePosition.canceled -= m_Wrapper.m_MenuActionsCallbackInterface.OnTrackedDevicePosition;
                @RightClick.started -= m_Wrapper.m_MenuActionsCallbackInterface.OnRightClick;
                @RightClick.performed -= m_Wrapper.m_MenuActionsCallbackInterface.OnRightClick;
                @RightClick.canceled -= m_Wrapper.m_MenuActionsCallbackInterface.OnRightClick;
                @MiddleClick.started -= m_Wrapper.m_MenuActionsCallbackInterface.OnMiddleClick;
                @MiddleClick.performed -= m_Wrapper.m_MenuActionsCallbackInterface.OnMiddleClick;
                @MiddleClick.canceled -= m_Wrapper.m_MenuActionsCallbackInterface.OnMiddleClick;
                @ScrollWheel.started -= m_Wrapper.m_MenuActionsCallbackInterface.OnScrollWheel;
                @ScrollWheel.performed -= m_Wrapper.m_MenuActionsCallbackInterface.OnScrollWheel;
                @ScrollWheel.canceled -= m_Wrapper.m_MenuActionsCallbackInterface.OnScrollWheel;
                @Click.started -= m_Wrapper.m_MenuActionsCallbackInterface.OnClick;
                @Click.performed -= m_Wrapper.m_MenuActionsCallbackInterface.OnClick;
                @Click.canceled -= m_Wrapper.m_MenuActionsCallbackInterface.OnClick;
                @Point.started -= m_Wrapper.m_MenuActionsCallbackInterface.OnPoint;
                @Point.performed -= m_Wrapper.m_MenuActionsCallbackInterface.OnPoint;
                @Point.canceled -= m_Wrapper.m_MenuActionsCallbackInterface.OnPoint;
                @Cancel.started -= m_Wrapper.m_MenuActionsCallbackInterface.OnCancel;
                @Cancel.performed -= m_Wrapper.m_MenuActionsCallbackInterface.OnCancel;
                @Cancel.canceled -= m_Wrapper.m_MenuActionsCallbackInterface.OnCancel;
                @Submit.started -= m_Wrapper.m_MenuActionsCallbackInterface.OnSubmit;
                @Submit.performed -= m_Wrapper.m_MenuActionsCallbackInterface.OnSubmit;
                @Submit.canceled -= m_Wrapper.m_MenuActionsCallbackInterface.OnSubmit;
                @Navigate.started -= m_Wrapper.m_MenuActionsCallbackInterface.OnNavigate;
                @Navigate.performed -= m_Wrapper.m_MenuActionsCallbackInterface.OnNavigate;
                @Navigate.canceled -= m_Wrapper.m_MenuActionsCallbackInterface.OnNavigate;
            }
            m_Wrapper.m_MenuActionsCallbackInterface = instance;
            if (instance != null)
            {
                @TrackedDeviceOrientation.started += instance.OnTrackedDeviceOrientation;
                @TrackedDeviceOrientation.performed += instance.OnTrackedDeviceOrientation;
                @TrackedDeviceOrientation.canceled += instance.OnTrackedDeviceOrientation;
                @TrackedDevicePosition.started += instance.OnTrackedDevicePosition;
                @TrackedDevicePosition.performed += instance.OnTrackedDevicePosition;
                @TrackedDevicePosition.canceled += instance.OnTrackedDevicePosition;
                @RightClick.started += instance.OnRightClick;
                @RightClick.performed += instance.OnRightClick;
                @RightClick.canceled += instance.OnRightClick;
                @MiddleClick.started += instance.OnMiddleClick;
                @MiddleClick.performed += instance.OnMiddleClick;
                @MiddleClick.canceled += instance.OnMiddleClick;
                @ScrollWheel.started += instance.OnScrollWheel;
                @ScrollWheel.performed += instance.OnScrollWheel;
                @ScrollWheel.canceled += instance.OnScrollWheel;
                @Click.started += instance.OnClick;
                @Click.performed += instance.OnClick;
                @Click.canceled += instance.OnClick;
                @Point.started += instance.OnPoint;
                @Point.performed += instance.OnPoint;
                @Point.canceled += instance.OnPoint;
                @Cancel.started += instance.OnCancel;
                @Cancel.performed += instance.OnCancel;
                @Cancel.canceled += instance.OnCancel;
                @Submit.started += instance.OnSubmit;
                @Submit.performed += instance.OnSubmit;
                @Submit.canceled += instance.OnSubmit;
                @Navigate.started += instance.OnNavigate;
                @Navigate.performed += instance.OnNavigate;
                @Navigate.canceled += instance.OnNavigate;
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
        void OnTrackedDeviceOrientation(InputAction.CallbackContext context);
        void OnTrackedDevicePosition(InputAction.CallbackContext context);
        void OnRightClick(InputAction.CallbackContext context);
        void OnMiddleClick(InputAction.CallbackContext context);
        void OnScrollWheel(InputAction.CallbackContext context);
        void OnClick(InputAction.CallbackContext context);
        void OnPoint(InputAction.CallbackContext context);
        void OnCancel(InputAction.CallbackContext context);
        void OnSubmit(InputAction.CallbackContext context);
        void OnNavigate(InputAction.CallbackContext context);
    }
}

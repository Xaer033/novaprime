using Fusion;
using UnityEngine;

public struct FrameInput : INetworkInput
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
    
    public Vector2 cursorPosition;
    public Vector2 cursorDirection;

    public override string ToString()
    {
        return $"HorizontalMovement:{horizontalMovement}|" +
               $"VerticalMovement:{verticalMovement}|" +
               $"downPressed:{downPressed}|" +
               $"downReleased:{downReleased}|" +
               $"jumpPressed:{jumpPressed}|" +
               $"jumpReleased:{jumpReleased}|" +
               $"interactPressed:{interactPressed}|" +
               $"primaryFire:{primaryFire}|" +
               $"secondaryFire:{secondaryFire}|" +
               $"useCusorPosition:{useCusorPosition}|" +
               $"cursorPosition:{cursorPosition}|" +
               $"cursorDirection:{cursorDirection}";
    }
}

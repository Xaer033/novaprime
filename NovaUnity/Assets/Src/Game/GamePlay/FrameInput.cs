using UnityEngine;

public struct FrameInput 
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
    
    public Vector3 cursorPosition;
    public Vector3 cursorDirection;
    //
    // public void Write(UdpPacket packet)
    // {
    //     packet.WriteFloat(horizontalMovement);
    //     packet.WriteFloat(verticalMovement);
    //     packet.WriteBool(downPressed);
    //     packet.WriteBool(downReleased);
    //     packet.WriteBool(jumpPressed);
    //     packet.WriteBool(jumpReleased);
    //     packet.WriteBool(interactPressed);
    //     packet.WriteBool(primaryFire);
    //     packet.WriteBool(secondaryFire);
    //     packet.WriteBool(useCusorPosition);
    //     packet.WriteVector3(cursorPosition);
    //     packet.WriteVector3(cursorDirection);
    // }
    //
    // public void Read(UdpPacket packet)
    // {
    //     horizontalMovement  = packet.ReadFloat();
    //     verticalMovement    = packet.ReadFloat();
    //     downReleased        = packet.ReadBool();
    //     downPressed         = packet.ReadBool();
    //     jumpPressed         = packet.ReadBool();
    //     jumpReleased        = packet.ReadBool();
    //     interactPressed     = packet.ReadBool();
    //     primaryFire         = packet.ReadBool();
    //     secondaryFire       = packet.ReadBool();
    //     useCusorPosition    = packet.ReadBool();
    //     cursorPosition      = packet.ReadVector3();
    //     cursorDirection     = packet.ReadVector3();
    // }

}

using System;

[Serializable]
public struct NetChannelHeader
{
    public uint   sequence;
    public uint   ackSequence;
    public uint   frameTick;
    public double sendTime;
    public double deliveryTime;
}

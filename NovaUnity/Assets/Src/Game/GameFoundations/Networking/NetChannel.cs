public class NetChannel
{
    public uint sequence;
    public uint ackSequence;

    public void Reset()
    {
        sequence = 0;
        ackSequence = 0;
    }
}

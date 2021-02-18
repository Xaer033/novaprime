using Mirage;

[NetworkMessage]
public struct RequestReadyUp
{
    public bool isReady;
    
    public RequestReadyUp(bool ready)
    {
        isReady = ready;
    }
}

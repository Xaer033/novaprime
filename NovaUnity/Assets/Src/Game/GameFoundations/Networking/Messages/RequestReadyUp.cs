using Mirror;

public struct RequestReadyUp : NetworkMessage
{
    public bool isReady;
    
    public RequestReadyUp(bool ready)
    {
        isReady = ready;
    }
}

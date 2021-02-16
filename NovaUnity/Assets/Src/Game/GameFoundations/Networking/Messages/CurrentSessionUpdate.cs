using Mirror;

public struct CurrentSessionUpdate : NetworkMessage
{
    public NetworkManager.SessionState sessionState;
    
    public CurrentSessionUpdate(NetworkManager.SessionState state)
    {
        sessionState = state;
    }
}

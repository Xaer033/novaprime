
public struct CurrentSessionUpdate
{
    public NetworkManager.SessionState sessionState;
    
    public CurrentSessionUpdate(NetworkManager.SessionState state)
    {
        sessionState = state;
    }
}

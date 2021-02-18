using Mirage;

[NetworkMessage]
public struct CurrentSessionUpdate
{
    public SessionState sessionState;
    
    public CurrentSessionUpdate(SessionState state)
    {
        sessionState = state;
    }
}

public enum SessionState
{
    NONE = 0,
    IN_LOBBY,
    IN_GAME
}

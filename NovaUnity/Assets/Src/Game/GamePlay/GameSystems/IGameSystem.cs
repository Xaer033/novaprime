using GhostGen;

public interface IGameSystem : IEventDispatcher
{
    int priority { get; set; }
    
    // Start is called before the first frame update
    void Start(bool hasAuthority, GameSystems gameSystems, GameState gameState);
    void CleanUp();
}

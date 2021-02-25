using GhostGen;

public interface IGameSystem : IEventDispatcher
{
    int priority { get; set; }
    
    
    void Start(GameSystems gameSystems, GameState gameState);
    void CleanUp();
}

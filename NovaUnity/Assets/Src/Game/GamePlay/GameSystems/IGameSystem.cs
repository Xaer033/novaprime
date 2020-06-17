using GhostGen;

public interface IGameSystem : IEventDispatcher
{
    int priority { get; set; }
    
    // Start is called before the first frame update
    void Start(GameSystems gameSystems, GameState gameState);
    void FixedStep(float fixedDeltaTime);
    void Step(float deltaTime);
    void LateStep(float deltaTime);
    void CleanUp();
}

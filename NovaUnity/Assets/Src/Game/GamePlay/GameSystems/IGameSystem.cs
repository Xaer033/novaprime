using GhostGen;

public interface IGameSystem : IEventDispatcher
{
    // Start is called before the first frame update
    void Start(GameSystems gameSystems, GameState gameState);
    void FixedStep(float deltaTime);
    void Step(float deltaTime);
    void LateStep(float deltaTime);
    void CleanUp();
}

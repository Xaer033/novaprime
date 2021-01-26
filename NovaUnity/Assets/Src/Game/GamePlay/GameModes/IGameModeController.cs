using GhostGen;

public interface IGameModeController : IEventDispatcher
{
    void Start(object context);
    void Step(float deltaTime);
    void FixedStep(float fixedDeltaTime);
    void LateStep(float deltaTime);
    void CleanUp();
}

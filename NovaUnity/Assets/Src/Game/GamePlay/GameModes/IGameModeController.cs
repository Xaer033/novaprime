using System;

public interface IGameModeController : GhostGen.IEventDispatcher
{
    void Start(object context);
    void Step(float deltaTime);
    void FixedStep(float fixedDeltaTime);
    void CleanUp();
}

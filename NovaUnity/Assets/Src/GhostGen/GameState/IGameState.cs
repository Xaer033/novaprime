namespace GhostGen
{
    public interface IGameState
	{
		void Init(GameStateMachine stateMatchine, object changeStateData);

		void Step(float deltaTime);
		void FixedStep(float fixedDeltaTime);

		void Exit();
	}
}

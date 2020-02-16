using UnityEngine;

public interface IAvatarController : ITimeWarpTarget, IAttackTarget
{
    UnitType GetUnitType();
    IInputGenerator GetInput();
    void SetInput(IInputGenerator input);

    string GetUUID();

    Vector3 GetPosition();

    AvatarView GetView();
    AvatarState GetState();

    UnitStats GetStats();
    
    void Move(Vector3 moveDelta, bool isOnPlatform);

    void SetVelocity(Vector3 velocity);
    
    void Start(GameSystems gameSystems);
    void FixedStep(float deltaTime, FrameInput input);
    void Step(float deltaTime);

}

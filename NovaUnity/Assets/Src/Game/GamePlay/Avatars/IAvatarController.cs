using UnityEngine;

public interface IAvatarController : ITimeWarpTarget, IAttackTarget
{
    UnitType GetUnitType();
    
    IInputGenerator input { get; set; }

    string uuid { get; }

    IAvatarView view { get; }
    AvatarState state { get; }
    UnitMap.Unit unit { get; }
    
    bool isSimulating { get; set; }
    
    void Move(Vector2 moveDelta, bool isOnPlatform);

    // void SetVelocity(Vector3 velocity);
    
    void Start(GameSystems gameSystems);
    void FixedStep(float deltaTime, FrameInput input);
    void Step(float deltaTime);

    void CleanUp();

}

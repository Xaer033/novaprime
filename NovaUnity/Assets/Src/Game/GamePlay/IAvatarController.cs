using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAvatarController : ITimeWarpTarget
{
    UnitType GetUnitType();
    IInputGenerator GetInput();

    string GetUUID();

    Vector3 GetPosition();
    
    void Move(Vector3 moveDelta, bool isOnPlatform);

    void SetVelocity(Vector3 velocity);
    
    void Start(GameSystems gameSystems);
    void FixedStep(float deltaTime);
    void Step(float deltaTime);

}

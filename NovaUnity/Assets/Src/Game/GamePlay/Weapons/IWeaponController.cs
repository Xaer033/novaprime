using UnityEngine;

public interface IWeaponController
{
    void FixedStep(float deltaTime);
    void Fire(Vector3 targetPos);
    
    IWeaponView view { get; }
}

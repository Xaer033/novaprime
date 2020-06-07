using UnityEngine;
using UnityEngine.Animations;

public interface IWeaponController
{
    void FixedStep(float deltaTime);
    void Fire(Vector3 targetPos);

    void Attach(Transform bodyParentHook, ParentConstraint leftHandConstraint, ParentConstraint rightHandConstraint);
    
    IWeaponView view { get; }
}

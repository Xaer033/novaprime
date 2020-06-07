using UnityEngine;
using UnityEngine.Animations;

public interface IWeaponView : ITimeWarpTarget 
{
    Transform leftHandHook { get; }
    Transform rightHandHook { get; }

    Transform barrelHook { get; }
    Transform transform { get; }

    
    void Attach(Transform bodyParentHook, ParentConstraint leftHandConstraint, ParentConstraint rightHandConstraint);
    
    void Fire(Vector3 target, float speedX);
}

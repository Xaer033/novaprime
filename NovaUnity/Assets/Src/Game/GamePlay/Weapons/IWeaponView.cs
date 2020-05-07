using UnityEngine;

public interface IWeaponView : ITimeWarpTarget 
{
    Transform barrelHook { get; }
    Transform transform { get; }
    
    void Fire(Vector3 target, float speedX);
}

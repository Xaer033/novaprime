using UnityEngine;

public struct AttackData
{
    public readonly string attackerUUID;
    public readonly int layer;
    public readonly int potentialDamage;
    public readonly DamageType damageType;
    public readonly Vector3 hitDirection;
    public readonly RaycastHit2D raycastHit;
    
    public AttackData(string instigatorUUID, int attackLayer, DamageType type, int damage, Vector3 direction, RaycastHit2D potentialHit = default(RaycastHit2D))
    {
        attackerUUID = instigatorUUID;
        layer = attackLayer;
        damageType = type;
        potentialDamage = damage;
        hitDirection = direction;
        raycastHit = potentialHit;
    }

}

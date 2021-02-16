using UnityEngine;

public struct AttackData
{
    public readonly string attackerUUID;
    public readonly int layer;
    public readonly float potentialDamage;
    public readonly DamageType damageType;
    public readonly Vector2 hitDirection;
    public readonly RaycastHit2D raycastHit;
    
    public AttackData(string instigatorUUID, int attackLayer, DamageType type, float damage, Vector2 direction, RaycastHit2D potentialHit = default(RaycastHit2D))
    {
        attackerUUID = instigatorUUID;
        layer = attackLayer;
        damageType = type;
        potentialDamage = damage;
        hitDirection = direction;
        raycastHit = potentialHit;
    }

}

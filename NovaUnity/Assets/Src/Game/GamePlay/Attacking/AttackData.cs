using UnityEngine;

public struct AttackData 
{
    public readonly int potentialDamage;
    public readonly DamageType damageType;
    public readonly Vector3 hitDirection;
    
    public AttackData(DamageType type, int damage, Vector3 direction)
    {
        damageType = type;
        potentialDamage = damage;
        hitDirection = direction;
    }

}

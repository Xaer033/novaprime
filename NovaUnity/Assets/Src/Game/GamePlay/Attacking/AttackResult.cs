public struct AttackResult
{
    public readonly AttackData attackData;
    public readonly IAttackTarget target;
    public readonly float totalDamageDelt;
    public readonly float targetHealthRemaining;
    // public readonly bool hasKilledTarget;

    public AttackResult(AttackData attackInfo, IAttackTarget attackTarget, float damage, float healthRemaining)
    {
        attackData = attackInfo;
        target = attackTarget;
        totalDamageDelt = damage;
        targetHealthRemaining = healthRemaining;
    }
}

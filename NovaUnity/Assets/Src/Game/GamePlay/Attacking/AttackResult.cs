public struct AttackResult
{
    public readonly AttackData attackData;
    public readonly IAttackTarget target;
    public readonly int totalDamageDelt;
    public readonly int targetHealthRemaining;
    // public readonly bool hasKilledTarget;

    public AttackResult(AttackData attackInfo, IAttackTarget attackTarget, int damage, int healthRemaining)
    {
        attackData = attackInfo;
        target = attackTarget;
        totalDamageDelt = damage;
        targetHealthRemaining = healthRemaining;
    }
}

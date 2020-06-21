public interface IAttackTarget
{
    AttackResult TakeDamage(AttackData attackData);

    int     health { get; }
}

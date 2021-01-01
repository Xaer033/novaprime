public interface IAttackTarget
{
    AttackResult TakeDamage(AttackData attackData);

    float     health { get; }
}

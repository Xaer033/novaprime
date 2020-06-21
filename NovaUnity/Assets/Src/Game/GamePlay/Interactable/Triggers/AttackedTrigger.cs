using System.Collections;
using Boo.Lang;

public class AttackedTrigger : Trigger, IAttackTarget
{
    private Hashtable _triggerData;

    private void Awake()
    {
        _triggerData = new Hashtable();
        _triggerData["tag"] = triggerTag;
    }
    
    public AttackResult TakeDamage(AttackData attackData)
    {
        AttackResult result = new AttackResult(attackData, this, attackData.potentialDamage, health );

        if(checkTriggerCondition(type, attackData.layer))
        {
            _triggerData["attackData"] = attackData;
            InvokeTrigger(TriggerEventType.ATTACKED, _triggerData);
        }
        return result;
    }

    public int     health
    {
        get { return 1; }
    }
}

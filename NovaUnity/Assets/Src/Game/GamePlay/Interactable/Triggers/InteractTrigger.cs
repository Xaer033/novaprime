using System.Collections;

public class InteractTrigger : Trigger
{
    private Hashtable _triggerData = new Hashtable();

    private void Awake()
    {
        _triggerData = new Hashtable();
        _triggerData["tag"] = triggerTag;
    }

    public void Interact(IAvatarController controller)
    {
        _triggerData["controller"] = controller;
        InvokeTrigger(TriggerEventType.ACTION, _triggerData);
    }
}

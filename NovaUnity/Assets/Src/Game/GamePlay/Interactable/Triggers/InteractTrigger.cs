using System.Collections;

public class InteractTrigger : Trigger
{
    public bool DebugSwitch;
    
    private Hashtable _triggerData = new Hashtable();

    private void Awake()
    {
        _triggerData = new Hashtable();
        _triggerData["tag"] = triggerTag;
    }
    
    #if UNITY_EDITOR
    void Update()
    {
        if (DebugSwitch)
        {
            DebugSwitch = false;
            Interact(null);
        }
    }
    #endif

    public void Interact(IAvatarController controller)
    {
        _triggerData["controller"] = controller;
        InvokeTrigger(TriggerEventType.ACTION, _triggerData);
    }
}

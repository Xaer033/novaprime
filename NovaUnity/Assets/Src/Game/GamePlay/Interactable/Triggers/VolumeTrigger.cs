using System;
using System.Collections;
using UnityEngine;

public class VolumeTrigger : Trigger
{
    private Hashtable _triggerData;
    
    private void Awake()
    {
        _triggerData = new Hashtable();
        _triggerData["tag"] = triggerTag;
    }

    protected void OnTriggerEnter2D(Collider2D other)
    {
        bool shouldTrigger = checkTriggerCondition(TriggerEventType.ENTER, other.gameObject.layer);
        if(shouldTrigger)
        {
            _triggerData["collider"] = other;
            InvokeTrigger(TriggerEventType.ENTER, _triggerData);
        }
    }
    
    protected void OnTriggerExit2D(Collider2D other)
    {
        bool shouldTrigger = checkTriggerCondition(TriggerEventType.EXIT, other.gameObject.layer);
        if(shouldTrigger)
        {
            _triggerData["collider"] = other;
            InvokeTrigger(TriggerEventType.ENTER, _triggerData);
        }
    }
}

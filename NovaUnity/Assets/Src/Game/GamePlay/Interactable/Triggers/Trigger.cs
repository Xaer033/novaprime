using System;
using UnityEngine;

public class Trigger : MonoBehaviour, ITrigger
{
    public event Action<TriggerEventType, object> onTriggerEvent;

    protected void InvokeEvent(TriggerEventType type, object customData)
    {
        if(onTriggerEvent != null)
        {
            onTriggerEvent(type, customData);
        }
    }
    
}

using System;
using GhostGen;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Trigger : MonoBehaviour, ITrigger
{
    public event Action<TriggerEventType, object> onTriggerEvent;

    public TriggerEventType type;
    public LayerMask affectedLayers;
    public string triggerTag;
    
    protected bool checkTriggerCondition(TriggerEventType testType, int testLayer)
    {
        bool layerCondition = affectedLayers == (affectedLayers | (1 << testLayer));
        bool triggerCondition = type.IsFlagSet(testType);
        
        return layerCondition && triggerCondition;
    }
    
    protected void InvokeTrigger(TriggerEventType triggertype, object customData)
    {
        onTriggerEvent?.Invoke(triggertype, customData);
    }
}

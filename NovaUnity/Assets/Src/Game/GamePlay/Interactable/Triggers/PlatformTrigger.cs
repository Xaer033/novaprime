using GhostGen;
using UnityEngine;

public class PlatformTrigger : Trigger
{
    public TriggerEventType type;
    public string triggerTag;
    public LayerMask affectedLayers;
    
    protected void OnTriggerEnter2D(Collider2D other)
    {
        checkTriggerCondition(TriggerEventType.ENTER, other.gameObject.layer);
    }
    
    protected void OnTriggerExit2D(Collider2D other)
    {
        checkTriggerCondition(TriggerEventType.EXIT, other.gameObject.layer);
    }

    private void checkTriggerCondition(TriggerEventType testType, int testLayer)
    {
        bool layerCondition = affectedLayers == (affectedLayers | (1 << testLayer));
        bool triggerCondition = type.IsFlagSet(testType);
        
        if(layerCondition && triggerCondition)
        {
            InvokeEvent(testType, triggerTag);
        }
    }
}

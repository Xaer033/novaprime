using System;

public interface ITrigger
{
    event Action<TriggerEventType, object> onTriggerEvent;
}


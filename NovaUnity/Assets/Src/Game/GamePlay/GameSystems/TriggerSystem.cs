using System.Collections.Generic;
using GhostGen;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TriggerSystem : NotificationDispatcher, IGameSystem
{
    private List<Trigger> _triggerList;
    
    // For quick lookup
    private HashSet<Trigger> _triggerSet;
    
    public int priority { get; set; }


    public TriggerSystem()
    {
        _triggerList = new List<Trigger>(100);
        _triggerSet = new HashSet<Trigger>();
    }
    
    public void Start(GameSystems gameSystems, GameState gameState)
    {
        addSceneTriggers();

        SceneManager.sceneLoaded += onSceneLoaded;
    }
    
    public void FixedStep(float fixedDeltaTime)
    {
        
    }
    
    public void Step(float deltaTime)
    {
        
    }
    
    public void LateStep(float deltaTime)
    {
        
    }
    
    public void CleanUp()
    {
        SceneManager.sceneLoaded -= onSceneLoaded;

        for(int i = _triggerList.Count - 1; i >= 0; --i)
        {
            removeTrigger(_triggerList[i]);
        }
            
        if(_triggerList != null)
        {
            _triggerList.Clear();
        }

        if(_triggerSet != null)
        {
            _triggerSet.Clear();
        }
    }

    private void addSceneTriggers()
    {
        Trigger[] rawArray = GameObject.FindObjectsOfType<Trigger>();
        for(int i = 0; i < rawArray.Length; ++i)
        {
            addTrigger(rawArray[i]);
        }
    }

    private void addTrigger(Trigger t)
    {
        // Trigger[] rawArray = GameObject.FindObjectsOfType<Trigger>();
       if(_triggerSet.Contains(t))
       {
           return;
       }

       t.onTriggerEvent += onTriggerForward;

       _triggerSet.Add(t);
       _triggerList.Add(t);
    }

    private void removeTrigger(Trigger t)
    {
        if(!_triggerSet.Contains(t))
        {
            return;
        }

        t.onTriggerEvent -= onTriggerForward;
       
        _triggerSet.Remove(t);
        _triggerList.Remove(t);
    }
    private void onSceneLoaded(Scene scene, LoadSceneMode node)
    {
        addSceneTriggers();
    }

    private void onTriggerForward(TriggerEventType type, object customData)
    {
        DispatchEvent(type.ToString(), false, customData);
    }
}

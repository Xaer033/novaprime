using System.Collections;
using System.Collections.Generic;
using GhostGen;
using UnityEngine;

public class PlatformSystem : NotificationDispatcher, IGameSystem
{
    private TimePlatformController _timePlatformController;
    private TriggerPlatformController _triggerPlatformController;
    
    private PlatformView[] _platformViewList;

    private GameSystems _gameSystems;
    private GameState _gameState;
    
    private TriggerSystem _triggerSystem;
    private Dictionary<string, List<PlatformState>> _triggerPlatforms;
    
    public int priority { get; set; }
    
    public PlatformSystem()
    {
        _triggerPlatforms = new Dictionary<string, List<PlatformState>>();
        
        _timePlatformController = new TimePlatformController();
        _triggerPlatformController = new TriggerPlatformController();
    }


    // Start is called before the first frame update
    public void Start(GameSystems gameSystems, GameState gameState)
    {
        _gameSystems = gameSystems;
        _gameState = gameState;

        _gameSystems.onFixedStep += FixedStep;
        _gameSystems.onStep += Step;
        
        _triggerSystem = gameSystems.Get<TriggerSystem>();
        _triggerSystem.AddListener(TriggerEventType.ENTER.ToString(), onTrigger);
        _triggerSystem.AddListener(TriggerEventType.EXIT.ToString(), onTrigger);
        _triggerSystem.AddListener(TriggerEventType.ACTION.ToString(), onTrigger);
        _triggerSystem.AddListener(TriggerEventType.ATTACKED.ToString(), onTrigger);
        
        _platformViewList = GameObject.FindObjectsOfType<PlatformView>();

        // No Platforms in the level
        if(_platformViewList == null)
        {
            return;
        }

        if(_gameState != null)
        {
            _gameState.platformStateList.Clear();
            for(int i = 0; i < _platformViewList.Length; ++i)
            {
                PlatformView view = _platformViewList[i];
                PlatformState state = new PlatformState(view.startPosition, 10, view.localWaypoints);
                view.state = state;
                _gameState.platformStateList.Add(state);

                if(view.platformType == PlatformType.TRIGGER)
                {
                    addTriggerPlatform(view.triggerTag, state);
                }
            }
        }
    }
    
    public void FixedStep(float deltaTime)
    {
        float time = Time.fixedTime;
        
        for(int i = 0; i < _platformViewList.Length; ++i)
        {
            PlatformView view = _platformViewList[i];
            PlatformState state = _gameState.platformStateList[i];
            
            float adjustedDeltaTime = deltaTime * state.timeScale;
            
            // can use different controllers for different platforms
            switch(view.platformType)
            { 
                case PlatformType.AUTO_TIME:
                    _timePlatformController.UpdatePlatform(state, view, adjustedDeltaTime, time);
                    break;
                
                case PlatformType.TRIGGER:
                    _triggerPlatformController.UpdatePlatform(state, view, adjustedDeltaTime, time);
                    break;
            }
        }
    }
    
    public void Step(float deltaTime)
    {
        if(_gameState == null)
        {
            Debug.LogError("GameState reference is null!");
            return;
        }

        //Don't need to log this, there just aren't any platforms in the scene
        if(_platformViewList == null)
        {
            return;
        }
        
        for(int i = 0; i < _platformViewList.Length; ++i)
        {
            PlatformView view = _platformViewList[i];
            PlatformState state = _gameState.platformStateList[i];
            
            float alpha = (Time.time - Time.fixedTime) / Time.fixedDeltaTime;
            view.viewRoot.position = Vector3.Lerp(state.prevPosition, state.position, alpha);
        }
    }

    public void CleanUp()
    {
        _gameSystems.onFixedStep -= FixedStep;
        _gameSystems.onStep -= Step;

        if(_gameState != null)
        {
            _gameState.platformStateList.Clear();
        }

        if(_triggerPlatforms != null)
        { 
            _triggerPlatforms.Clear();
        }
        
        if(_triggerSystem != null)
        {
            _triggerSystem.RemoveListener(TriggerEventType.ENTER.ToString(), onTrigger);
            _triggerSystem.RemoveListener(TriggerEventType.EXIT.ToString(), onTrigger);
            _triggerSystem.RemoveListener(TriggerEventType.ACTION.ToString(), onTrigger);
            _triggerSystem.RemoveListener(TriggerEventType.ATTACKED.ToString(), onTrigger);
        }
    }


    private void onTrigger(GeneralEvent e)
    {
        Hashtable table = (Hashtable) e.data;
        string triggerTag = table["tag"] as string;
        
        List<PlatformState> triggerPlatList;
        if(_triggerPlatforms.TryGetValue(triggerTag, out triggerPlatList))
        {
            for(int i = 0; i < triggerPlatList.Count; ++i)
            {
                triggerPlatList[i].wasTriggered = true;
            }
        }
    }

    private void addTriggerPlatform(string triggerTag, PlatformState state)
    {
        List<PlatformState> triggerPlatList;
        if(!_triggerPlatforms.TryGetValue(triggerTag, out triggerPlatList))
        {
            triggerPlatList = new List<PlatformState>(4);
            _triggerPlatforms.Add(triggerTag, triggerPlatList);
        }
        
        triggerPlatList.Add(state);
    }

}

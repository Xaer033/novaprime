using System;
using System.Collections;
using System.Collections.Generic;
using GhostGen;
using UnityEngine;

public class PlatformSystem : NotificationDispatcher, IGameSystem
{
    private const int BAD_JUJU = 666666666;
    
    private       TimePlatformController    _timePlatformController;
    private       TriggerPlatformController _triggerPlatformController;
    
    private PlatformView[] _platformViewList;

    private GameSystems       _gameSystems;
    private GameState         _gameState;
    private NetSnapshotSystem _netSnapshotSystem;
    private TriggerSystem     _triggerSystem;
    
    private Dictionary<string, List<PlatformState>> _triggerPlatforms;
    
    public int priority { get; set; }
    
    public PlatformSystem()
    {
        _triggerPlatforms          = new Dictionary<string, List<PlatformState>>();
        _timePlatformController    = new TimePlatformController();
        _triggerPlatformController = new TriggerPlatformController();
    }


    // Start is called before the first frame update
    public void Start(GameSystems gameSystems, GameState gameState)
    {
        _gameSystems = gameSystems;
        _gameState   = gameState;

        _gameSystems.onFixedStep += FixedStep;
        _gameSystems.onStep += Step;

        _netSnapshotSystem = gameSystems.Get<NetSnapshotSystem>();
        _netSnapshotSystem.onInterpolationUpdate += onSnapshotInterpolationUpdate;
        
        _triggerSystem = gameSystems.Get<TriggerSystem>();
        _triggerSystem.AddListener(TriggerEventType.ENTER.ToString(), onTrigger);
        _triggerSystem.AddListener(TriggerEventType.EXIT.ToString(), onTrigger);
        _triggerSystem.AddListener(TriggerEventType.ACTION.ToString(), onTrigger);
        _triggerSystem.AddListener(TriggerEventType.ATTACKED.ToString(), onTrigger);
        
        _platformViewList = GameObject.FindObjectsOfType<PlatformView>(true);

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
                PlatformView  view  = _platformViewList[i];
                PlatformState state = new PlatformState(view.startPosition, 10, view.localWaypoints);

                state.netId = view.netId;
                view.state  = state;
                view.index  = i;
                view.onClientStart += onClientStart;
                
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

            if(state.netId == BAD_JUJU && view.netIdentity.netId != 0)
            {
                state.netId = view.netIdentity.netId;
                _gameState.platformStateList[i] = state;
            }
            
            // Gross, figure out a better way to do this
            // if(Singleton.instance.networkManager.isPureClient)
            // {
            //     continue;
            // }
            
            float adjustedDeltaTime = deltaTime * state.timeScale;
            
            // can use different controllers for different platforms
            switch(view.platformType)
            { 
                case PlatformType.AUTO_TIME:
                    _timePlatformController.UpdatePlatform(ref state, view, adjustedDeltaTime, time);
                    break;
                
                case PlatformType.TRIGGER:
                    _triggerPlatformController.UpdatePlatform(ref state, view, adjustedDeltaTime, time);
                    break;
            }

            _gameState.platformStateList[i] = state;
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

        // Gross, figure out a better way to do this
        if(Singleton.instance.networkManager.isPureClient)
        {
            return;
        }
        
        for(int i = 0; i < _platformViewList.Length; ++i)
        {
            PlatformView  view  = _platformViewList[i];
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

        if(_netSnapshotSystem != null)
        {
            _netSnapshotSystem.onInterpolationUpdate -= onSnapshotInterpolationUpdate;
        }
    }

    private void onSnapshotInterpolationUpdate(float alpha, GameState.Snapshot from, GameState.Snapshot to)
    {
        var fromPlatformList = from.platformStateList;
        fromPlatformList.Sort((a, by) =>  { return a.netId.CompareTo(by.netId);  });
        
        var toPlatformList   = to.platformStateList;
        toPlatformList.Sort((a, by) =>  { return a.netId.CompareTo(by.netId);  });

        int maxCount = Math.Max(toPlatformList.Count, fromPlatformList.Count);

        for(int i = 0; i < maxCount; ++i)
        {
            if( i >= toPlatformList.Count     || 
                i >= fromPlatformList.Count   || 
                i >= _gameState.platformStateList.Count)
            {
                break;
            }

            var state = _gameState.platformStateList[i];
            var view  = _platformViewList[i];

            int fromIndex = getStateIndexForList(state.netId, fromPlatformList);
            int toIndex   = getStateIndexForList(state.netId, toPlatformList);
            
            if(fromIndex < 0 || toIndex < 0)
            {
                continue;
            }
            
            var fromSnapshot = fromPlatformList[fromIndex];
            var toSnapshot   = toPlatformList[toIndex];

            state.prevPosition      = state.position;
            state.position          = Vector2.Lerp(fromSnapshot.position, toSnapshot.position, alpha);
            state.velocity          = Vector2.Lerp(fromSnapshot.velocity, toSnapshot.velocity, alpha);
            view.transform.position = state.position;
            view.viewRoot.position  = state.position;
            
            view._raycastController.UpdateRaycastOrigins();
        }
    }

    private int getStateIndexForList(uint netId, List<PlatformState.NetSnapshot> snapshotList)
    {
        int result = -1;
        
        for(int i = 0; i < snapshotList.Count; ++i)
        {
            if(snapshotList[i].netId == netId)
            {
                result = i;
                break;
            }
        }

        return result;
    }

    private void onClientStart(PlatformView view)
    {
        var state = _gameState.platformStateList[view.index];
        state.netId = view.netId;
        _gameState.platformStateList[view.index] = state;
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

using System.Collections.Generic;
using GhostGen;
using UnityEngine;

public class PlatformSystem : NotificationDispatcher, IGameSystem
{
    private TimePlatformController _timePlatformController;
    private PlatformView[] _platformViewList;

    private GameState _gameState;
    private TriggerSystem _triggerSystem;
    private Dictionary<string, PlatformState> _triggerPlatforms;
    
    public int priority { get; set; }
    
    public PlatformSystem()
    {
        _triggerPlatforms = new Dictionary<string, PlatformState>();
        
        _timePlatformController = new TimePlatformController();
    }


    // Start is called before the first frame update
    public void Start(GameSystems gameSystems, GameState gameState)
    {
        _gameState = gameState;

        _triggerSystem = gameSystems.Get<TriggerSystem>();
        _triggerSystem.AddListener(TriggerEventType.ENTER.ToString(), onTriggerEnter);
        
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
            _timePlatformController.UpdatePlatform(state, view, adjustedDeltaTime, time);
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
    
    public void LateStep(float deltaTime)
    {
    
    }
    
    public void CleanUp()
    {
        if(_gameState != null)
        {
            _gameState.projectileStateList.Clear();
        }

        if(_triggerSystem != null)
        {
            _triggerSystem.RemoveListener(TriggerEventType.ENTER.ToString(), onTriggerEnter);
        }
    }


    private void onTriggerEnter(GeneralEvent e)
    {
        string triggerTag = (string) e.data;
        
        if(triggerTag == "e1")
        {
            Debug.Log("Got dat e1 boi");
        }
    }
}

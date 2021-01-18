using System;
using System.Collections.Generic;
using GhostGen;
using UnityEngine;

public class GameSystems : NotificationDispatcher
{
    private GameState _gameState;
    private GameplayResources _gameplayResources;
    
    private Dictionary<Type, IGameSystem> _gameSystemMap = new Dictionary<Type, IGameSystem>();
    private List<IGameSystem> _sortedSystemList = new List<IGameSystem>(20);

    public event Action<float> onStep;
    public event Action<float> onFixedStep;
    public event Action<float> onLateStep;
    
    public T Get<T>() where T : IGameSystem
    {
        IGameSystem system;
        if (!_gameSystemMap.TryGetValue(typeof(T), out system))
        {
            Debug.LogError("Game System: " + typeof(T) + " not found!");
            return default(T);
        }

        return (T)system;
    }
    
    public GameSystems(GameState gameState, GameplayResources gameplayResources)
    {
        _gameState = gameState;
        
        IGameSystem projectileSystem     = new ProjectileSystem(gameplayResources, 125);
        IGameSystem avatarSystem         = new AvatarSystem(gameplayResources);
        IGameSystem healthUiSystem       = new HealthUISystem(gameplayResources);
        IGameSystem spawnPointSystem     = new SpawnPointSystem();
        IGameSystem platformSystems      = new PlatformSystem();
        IGameSystem triggerSystems       = new TriggerSystem(); 
 
        // Higher priority value goes first
        _addSystem(50, triggerSystems);
        _addSystem(40, avatarSystem);
        _addSystem(30, spawnPointSystem);
        _addSystem(20, platformSystems);
        _addSystem(10, projectileSystem);
        _addSystem( 0, healthUiSystem);
        
        _sortedSystemList.Sort(_sortSystems);
    }

    public void Start()
    {
        for(int i = 0; i < _sortedSystemList.Count; ++i)
        {
            _sortedSystemList[i].Start(this, _gameState);
        }
    }
    
    public void FixedStep(float fixedDeltaTime)
    {
        // Physics2D.SyncTransforms();
        
        if(onFixedStep != null)
        {
            onFixedStep(fixedDeltaTime);
        }

        // Physics2D.Simulate(fixedDeltaTime);
        // for(int i = 0; i < _sortedSystemList.Count; ++i)
        // {
        //     _sortedSystemList[i].FixedStep(fixedDeltaTime);
        // }
    }

    public void Step(float deltaTime)
    {
        if(onStep != null)
        {
            onStep(deltaTime);
        }
        
        // for(int i = 0; i < _sortedSystemList.Count; ++i)
        // {
        //     _sortedSystemList[i].Step(deltaTime);
        // }
    }

    public void LateStep(float deltaTime)
    {
        if(onLateStep != null)
        {
            onLateStep(deltaTime);
        }
        
        // for(int i = 0; i < _sortedSystemList.Count; ++i)
        // {
        //     _sortedSystemList[i].LateStep(deltaTime);
        // }
    }
    
    public void CleanUp()
    {
        for(int i = 0; i < _sortedSystemList.Count; ++i)
        {
            _sortedSystemList[i].CleanUp();
        }
        
        _sortedSystemList.Clear();
        _gameSystemMap.Clear();
    }

    private void _addSystem(int priority, IGameSystem gameSystem)
    {
        gameSystem.priority = priority;
        _gameSystemMap.Add(gameSystem.GetType(), gameSystem);
        _sortedSystemList.Add(gameSystem);
    }

    private int _sortSystems(IGameSystem a, IGameSystem b)
    {
        return b.priority.CompareTo(a.priority);
    }
}

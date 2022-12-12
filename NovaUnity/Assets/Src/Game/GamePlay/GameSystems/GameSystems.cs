using System;
using System.Collections.Generic;
using GhostGen;
using UnityEngine;

public class GameSystems : NotificationDispatcher
{
    private GameState         _gameState;
    private GameplayResources _gameplayResources;
    
    private Dictionary<Type, IGameSystem> _gameSystemMap    = new Dictionary<Type, IGameSystem>();
    private List<IGameSystem>             _sortedSystemList = new List<IGameSystem>(20);

    public event Action<float> onStep;
    public event Action<float> onFixedStep;
    public event Action<float> onLateStep;
    
    public T Get<T>() 
    {
        IGameSystem system;
        if (!_gameSystemMap.TryGetValue(typeof(T), out system))
        {
            Debug.LogError("Game System: " + typeof(T) + " not found!");
            return default(T);
        }

        return (T)system;
    }

    public bool isAuthoritive { get; private set; }
    
    public GameSystems(GameState gameState, GameplayResources gameplayResources, bool isTheAuthority)
    {
        _gameState    = gameState;
        isAuthoritive = isTheAuthority;
        
        // Higher priority value goes first
        _addSystem(600,     new NetSnapshotSystem());
        _addSystem(500,     new TriggerSystem());
        _addSystem(400,     new AvatarSystem(gameplayResources));
        _addSystem(300,     new SpawnPointSystem());
        _addSystem(200,     new PlatformSystem());
        _addSystem(100,     new ProjectileSystem(gameplayResources, 256));
        _addSystem(100,     new HealthUISystem(gameplayResources));
        _addSystem( 50,     new NetworkSystem(gameplayResources.unitMap));
        
        
        _sortedSystemList.Sort(_sortSystems);
    }

    public void Start()
    {
        for(int i = 0; i < _sortedSystemList.Count; ++i)
        {
            _sortedSystemList[i]?.Start(isAuthoritive, this, _gameState);
        }
    }
    
    public void FixedStep(float fixedDeltaTime)
    {
        onFixedStep?.Invoke(fixedDeltaTime);
    }

    public void Step(float deltaTime)
    {
        onStep?.Invoke(deltaTime);
    }

    public void LateStep(float deltaTime)
    {
        onLateStep?.Invoke(deltaTime);
    }
    
    public void CleanUp()
    {
        for(int i = 0; i < _sortedSystemList.Count; ++i)
        {
            _sortedSystemList[i]?.CleanUp();
        }
        
        _sortedSystemList?.Clear();
        _gameSystemMap?.Clear();
    }

    private void _addSystem(int priority, IGameSystem gameSystem)
    {
        gameSystem.priority = priority;
        _gameSystemMap?.Add(gameSystem.GetType(), gameSystem);
        _sortedSystemList?.Add(gameSystem);
    }

    private int _sortSystems(IGameSystem a, IGameSystem b)
    {
        return b.priority.CompareTo(a.priority);
    }
}

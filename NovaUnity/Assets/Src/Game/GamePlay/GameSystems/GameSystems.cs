using System;
using System.Collections.Generic;
using Fusion;
using GhostGen;
using UnityEngine;

public class GameSystems : NotificationDispatcher
{
    private GameState         _gameState;
    private GameplayResources _gameplayResources;

    private NetSimulator _netSimulator;
    private GameObject   _netSimulatorObject;
    
    private Dictionary<Type, IGameSystem> _gameSystemMap    = new Dictionary<Type, IGameSystem>();
    private List<IGameSystem>             _sortedSystemList = new List<IGameSystem>(20);

    public event Action<float>        onStep;
    public event Action<float>        onFixedStep;
    public event Action<NetworkRunner, NetSimulator> onFixedNetworkStep;
    public event Action<float>        onLateStep;
    
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

        Singleton.instance.networkManager.runner.Spawn(gameplayResources.netSimulator, onBeforeSpawned:OnBeforeSpawn);
       
        IGameSystem projectileSystem  = new ProjectileSystem(gameplayResources, 125);
        IGameSystem avatarSystem      = new AvatarSystem(gameplayResources);
        IGameSystem healthUiSystem    = new HealthUISystem(gameplayResources);
        IGameSystem networkSystem     = new NetworkSystem(gameplayResources.unitMap);
        IGameSystem spawnPointSystem  = new SpawnPointSystem();
        IGameSystem platformSystem    = new PlatformSystem();
        IGameSystem triggerSystem     = new TriggerSystem();
        IGameSystem netSnapshotSystem = new NetSnapshotSystem();
        
        /// Higher priority value goes first
        // _addSystem(600,     netSnapshotSystem);
        _addSystem(500,     triggerSystem);
        _addSystem(400,     avatarSystem);
        _addSystem(300,     spawnPointSystem);
        _addSystem(200,     platformSystem);
        // _addSystem(100,     projectileSystem);
        _addSystem(100,     healthUiSystem);
        // _addSystem( 50,     networkSystem);
        
        
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

    private void OnFixedNetworkStep(NetSimulator netSim)
    {
        onFixedNetworkStep?.Invoke(Singleton.instance.networkManager.runner, netSim);
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

    private void OnBeforeSpawn(NetworkRunner runner, NetworkObject obj)
    {
        _netSimulator = obj.GetComponent<NetSimulator>();
        _netSimulator.onFixedNetworkUpdate += OnFixedNetworkStep;   
    }
}

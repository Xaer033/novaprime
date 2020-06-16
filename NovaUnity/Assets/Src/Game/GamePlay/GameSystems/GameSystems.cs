using System;
using System.Collections.Generic;
using GhostGen;
using UnityEngine;

public class GameSystems : NotificationDispatcher
{
    private GameState _gameState;
    private GameplayResources _gameplayResources;
    
    private Dictionary<Type, IGameSystem> _gameSystemMap = new Dictionary<Type, IGameSystem>();
    
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
        IGameSystem healthUISystem       = new HealthUISystem();
        IGameSystem spawnPointSystem     = new SpawnPointSystem();
        IGameSystem platformSystems      = new PlatformSystem();

        _addSystem(projectileSystem);
        _addSystem(avatarSystem);
        _addSystem(healthUISystem);
        _addSystem(spawnPointSystem);
        _addSystem(platformSystems);
        
        // _gameSystemMap.Add(typeof(ProjectileSystem), projectileSystem);
        // _gameSystemMap.Add(typeof(AvatarSystem),     avatarSystem);
        // _gameSystemMap.Add(typeof(HealthUISystem),   healthUISystem);
        // _gameSystemMap.Add(typeof(SpawnPointSystem), spawnPointSystem);
        // _gameSystemMap.Add(typeof(PlatformSystem),   platformSystems);
    }

    public void Start()
    {
        foreach (var pair in _gameSystemMap)
        {
            pair.Value.Start(this, _gameState);
        }
    }
    
    public void FixedStep(float deltaTime)
    {
        foreach (var pair in _gameSystemMap)
        {
            pair.Value.FixedStep(deltaTime);
        }
    }

    public void Step(float deltaTime)
    {
        foreach (var pair in _gameSystemMap)
        {
            pair.Value.Step(deltaTime);
        }
    }

    public void LateStep(float deltaTime)
    {
        foreach (var pair in _gameSystemMap)
        {
            pair.Value.LateStep(deltaTime);
        }
    }
    
    public void CleanUp()
    {
        foreach (var pair in _gameSystemMap)
        {
            pair.Value.CleanUp();
        }
    }

    private void _addSystem(IGameSystem gameSystem)
    {
        _gameSystemMap.Add(gameSystem.GetType(), gameSystem);
    }
}

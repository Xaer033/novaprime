using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSystems : GhostGen.NotificationDispatcher
{
    private GameState _gameState;
    private Dictionary<Type, IGameSystem> _gameSystemMap = new Dictionary<Type, IGameSystem>();
    
    public ProjectileSystem projectileSystem { get; private set; }
    public AvatarSystem avatarSystem { get; private set; }
    public HealthUISystem healthUISystem { get; private set; }
    
    public T GetSystem<T>() where T : IGameSystem
    {
        IGameSystem system;
        if (!_gameSystemMap.TryGetValue(typeof(T), out system))
        {
            Debug.LogError("Game System: " + typeof(T) + " not found!");
            return default(T);
        }

        return (T)system;
    }
    
    public GameSystems(GameState gameState)
    {
        _gameState = gameState;
        
        projectileSystem = new ProjectileSystem(125);
        avatarSystem = new AvatarSystem();
        healthUISystem = new HealthUISystem();

        _gameSystemMap.Add(typeof(ProjectileSystem), projectileSystem);
        _gameSystemMap.Add(typeof(AvatarSystem), avatarSystem);
        _gameSystemMap.Add(typeof(HealthUISystem), healthUISystem);
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
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSystems : GhostGen.NotificationDispatcher
{
    private GameState _gameState;
    
    public ProjectileSystem projectileSystem { get; private set; }

    
    public GameSystems(GameState gameState)
    {
        _gameState = gameState;
        projectileSystem = new ProjectileSystem(500);
    }

    public void Start()
    {
        projectileSystem.Start();
    }
    
    public void FixedStep(float deltaTime)
    {
        projectileSystem.FixedStep(deltaTime);
    }

    public void Step(float deltaTime)
    {
        projectileSystem.Step(deltaTime);   
    }
}

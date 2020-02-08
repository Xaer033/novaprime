using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameSystem 
{
    // Start is called before the first frame update
    void Start(GameSystems gameSystems, GameState gameState);
    void FixedStep(float deltaTime);
    void Step(float deltaTime);
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState 
{
    public List<PlayerState> playerStateList = new List<PlayerState>(4);
    public List<EnemyState> enemyStateList = new List<EnemyState>(200);
    public List<ProjectileState> projectileStateList = new List<ProjectileState>(200);
}

using System.Collections.Generic;

public class GameState 
{
    public List<PlayerState> playerStateList = new List<PlayerState>(4);
    public List<EnemyState> enemyStateList = new List<EnemyState>(200);
    public List<ProjectileState> projectileStateList = new List<ProjectileState>(200);
    public List<SpawnPointState> spawnPointStateList = new List<SpawnPointState>(50);
    public List<PlatformState> platformStateList = new List<PlatformState>(100);
}

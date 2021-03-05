using System.Collections.Generic;

public class GameState
{
    // public List<NetPlayer> netPlayerList = new List<NetPlayer>(4);
    public List<PlayerState>     playerStateList     = new List<PlayerState>(4);
    public List<EnemyState>      enemyStateList      = new List<EnemyState>(200);
    public List<ProjectileState> projectileStateList = new List<ProjectileState>(200);
    public List<SpawnPointState> spawnPointStateList = new List<SpawnPointState>(50);
    public List<PlatformState>   platformStateList   = new List<PlatformState>(100);


    public struct Snapshot
    {
        public readonly List<PlayerStateSnapshot> playerStateList;
        public readonly List<EnemyState>          enemyStateList;
        public readonly List<ProjectileState>     projectileStateList;
        public readonly List<SpawnPointState>     spawnPointStateList;
        public readonly List<PlatformState>       platformStateList;

        public Snapshot(GameState gameState)
        {
            playerStateList     = new List<PlayerStateSnapshot>(4);
            enemyStateList      = new List<EnemyState>(200);
            projectileStateList = new List<ProjectileState>(200);
            spawnPointStateList = new List<SpawnPointState>(50);
            platformStateList   = new List<PlatformState>(100);

            foreach(var state in gameState.playerStateList)
            {
                playerStateList.Add(state.Snapshot());
            }
            
            projectileStateList.AddRange(gameState.projectileStateList); // Nice
            spawnPointStateList.AddRange(gameState.spawnPointStateList);
            platformStateList.AddRange(gameState.platformStateList);
        }
    }

}

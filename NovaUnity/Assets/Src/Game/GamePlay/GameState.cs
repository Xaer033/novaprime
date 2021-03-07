using System;
using System.Collections.Generic;

public class GameState
{
    // public List<NetPlayer> netPlayerList = new List<NetPlayer>(4);
    public List<PlayerState>     playerStateList     = new List<PlayerState>(4);
    public List<EnemyState>      enemyStateList      = new List<EnemyState>(200);
    public List<ProjectileState> projectileStateList = new List<ProjectileState>(200);
    public List<SpawnPointState> spawnPointStateList = new List<SpawnPointState>(50);
    public List<PlatformState>   platformStateList   = new List<PlatformState>(100);


    [Serializable]
    public struct Snapshot
    {
        public readonly List<PlayerState.NetSnapshot>     playerStateList;
        public readonly List<EnemyState.NetSnapshot>      enemyStateList;
        public readonly List<ProjectileState.NetSnapshot> projectileStateList;
        public readonly List<SpawnPointState>             spawnPointStateList;
        public readonly List<PlatformState.NetSnapshot>   platformStateList;


        public Snapshot(GameState gameState)
        {
            playerStateList     = new List<PlayerState.NetSnapshot>(4);
            enemyStateList      = new List<EnemyState.NetSnapshot>(200);
            projectileStateList = new List<ProjectileState.NetSnapshot>(200);
            spawnPointStateList = new List<SpawnPointState>(50);
            platformStateList   = new List<PlatformState.NetSnapshot>(100);

            spawnPointStateList.AddRange(gameState.spawnPointStateList);
            
            foreach(var state in gameState.playerStateList)
            {
                playerStateList.Add(NetUtility.Snapshot(state));
            }
            
            foreach(var state in gameState.enemyStateList)
            {
                enemyStateList.Add(NetUtility.Snapshot(state));
            }
            
            foreach(var state in gameState.projectileStateList)
            {
                if(state.isActive)
                {
                    projectileStateList.Add(NetUtility.Snapshot(state));                    
                }
            }
            
            foreach(var state in gameState.platformStateList)
            {
                platformStateList.Add(NetUtility.Snapshot(state));
            }
        }
    }
}

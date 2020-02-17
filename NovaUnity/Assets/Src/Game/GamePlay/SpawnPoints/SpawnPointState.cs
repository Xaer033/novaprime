using UnityEngine;

public class SpawnPointState
{
    public SpawnPointState(float nextTime)
    {
        nextSpawnTime = nextTime;
    }
    public float nextSpawnTime;
    public int spawnCount;
}

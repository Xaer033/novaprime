public struct SpawnPointState
{
    public SpawnPointState(float nextTime)
    {
        nextSpawnTime = nextTime;
        spawnCount    = 0;
    }
    
    public float nextSpawnTime;
    public int spawnCount;
}

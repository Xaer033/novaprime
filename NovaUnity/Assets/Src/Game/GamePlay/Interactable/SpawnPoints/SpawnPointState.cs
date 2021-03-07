using System;

[Serializable]
public struct SpawnPointState
{
    public SpawnPointState(float nextTime)
    {
        nextSpawnTime = nextTime;
        spawnCount    = 0;
        netId         = 0;
    }

    public uint  netId;
    public float nextSpawnTime;
    public int   spawnCount;
}

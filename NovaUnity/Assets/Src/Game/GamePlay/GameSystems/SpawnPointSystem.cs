using GhostGen;
using UnityEngine;

public class SpawnPointSystem : NotificationDispatcher, IGameSystem
{
    private GameSystems _gameSystems;
    private GameState _gameState;

    private SpawnPointView[] _spawnPointViewList;
    
    
    public int priority { get; set; }
    
    //Ensures that dependencies are initialized first
    public SpawnPointSystem()
    {
        
    }
    
    public void Start(GameSystems gameSystems, GameState gameState)
    {
        _gameSystems = gameSystems;
        _gameState = gameState;

        _gameState.spawnPointStateList.Clear();
        _spawnPointViewList = GameObject.FindObjectsOfType<SpawnPointView>();
        for (int i = 0; i < _spawnPointViewList.Length; ++i)
        {
            
            SpawnPointView view = _spawnPointViewList[i];
            SpawnPointState state = new SpawnPointState(Time.fixedTime + view.spawnInterval);
            _gameState.spawnPointStateList.Add(state);
        }
    }

    public void Step(float deltaTime)
    {
        
    }
    
    public void FixedStep(float deltaTime)
    {
        float now = Time.fixedTime;
        for (int i = 0; i < _spawnPointViewList.Length; ++i)
        {
            SpawnPointState state = _gameState.spawnPointStateList[i];
            SpawnPointView view = _spawnPointViewList[i];

            bool shouldSpawn = _shouldSpawn(now, view, state);
            if (shouldSpawn)
            {
                _spawn(now, view, state);
            }
        }
    }


    public void LateStep(float deltaTime)
    {
        
    }
    
    public void CleanUp()
    {
        _gameState.spawnPointStateList.Clear();
    }

    private bool _shouldSpawn(float now, SpawnPointView view, SpawnPointState state)
    {
        float timeDelta = state.nextSpawnTime - now;

        bool canRepeat = (state.spawnCount < view.maxSpawnCount) || (view.maxSpawnCount <= 0);
        
        switch (view.spawnMode)
        {
            case SpawnMode.ONCE:        return state.spawnCount == 0 && timeDelta <= 0.0f;
            case SpawnMode.REPEATING:   return canRepeat && timeDelta <= 0.0f;
        }

        return false;
    }
    
    private void _spawn(float now, SpawnPointView view, SpawnPointState state)
    {
        state.spawnCount++;
        state.nextSpawnTime = now + view.spawnInterval;

        SpawnPointData data = view.spawnPointData;
        data.position = view.transform.position;
        
        _gameSystems.DispatchEvent(GamePlayEventType.SPAWN_POINT_TRIGGERED, false, data);
    }
}

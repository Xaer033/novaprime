using System.Collections;
using System.Collections.Generic;
using GhostGen;
using UnityEngine;

public class AvatarSystem : NotificationDispatcher, IGameSystem
{
    public const int kMaxPlayerCount = 4;

    private GameState _gameState;
    private GameSystems _gameSystems;

    private UnitMap _unitMap;
    private List<IAvatarController>      _avatarControllerList;
    private Dictionary<string, IAvatarController> _avatarLookUpMap;

    private GameplayCamera _camera;
    private static int _spawnCount = 0;
    
    public AvatarSystem()
    {
        _unitMap = Singleton.instance.gameplayResources.unitMap;
        _avatarControllerList       = new List<IAvatarController>(200);
        _avatarLookUpMap            = new Dictionary<string, IAvatarController>();

//        _playerActionList         = new List<PlayerActions>(kMaxPlayerCount);
    }
    
    public void Start(GameSystems gameSystems, GameState gameState)
    {
        _gameSystems = gameSystems;
        _gameState = gameState;
    }

    public void FixedStep(float deltaTime)
    {
        for (int i = 0; i < _avatarControllerList.Count; ++i)
        {
            _avatarControllerList[i].FixedStep(deltaTime);
        }
    }

    public void Step(float deltaTime)
    {
        for (int i = 0; i < _avatarControllerList.Count; ++i)
        {
            _avatarControllerList[i].Step(deltaTime);
        }
    }

    public IAvatarController GetController(string uuid)
    {
        return _avatarLookUpMap[uuid];
    }
    
    public T Spawn<T>(string unitId, Vector3 position) where T : IAvatarController
    {
        _spawnCount++;

        UnitMap.Unit unit = _unitMap.GetUnit(unitId);
        string uuid = _generateUUID(unit);
        
        IAvatarController controller = _spawnAvatar(uuid, unit, position);

        if (controller != null)
        {
            _avatarControllerList.Add(controller);
            _avatarLookUpMap.Add(uuid, controller);
        }

        return (T)controller;
    }

    private IAvatarController _spawnAvatar(string uuid, UnitMap.Unit unit, Vector3 position)
    {
        switch (unit.type)
        {
            case UnitType.PLAYER:    return _spawnPlayer(uuid, unit, position);
            case UnitType.ENEMY:     return _spawnEnemy(uuid, unit, position);
        }
        Debug.LogError("Can't spawn type: " + unit.type);
        return null;
    }

    private PlayerController _spawnPlayer(string uuid, UnitMap.Unit unit, Vector3 position)
    {
        AvatarView view = GameObject.Instantiate<AvatarView>(unit.view);

        GameplayCamera cam = _getGameplayCamera();
        if (cam != null)
        {
            cam.AddTarget(view.transform);
        }
        else
        {
            Debug.LogError("Could not find or create gameplay camera! Aborting player creation");
        }
        
        PlayerState state = PlayerState.Create(uuid, 0, position);

        PlayerInput input = new PlayerInput(0, cam.gameCamera);
        PlayerController controller = new PlayerController(unit, state, view, input);
        controller.Start(_gameSystems);

        _gameState.playerStateList.Add(state); 
        return controller;
    }

    private IAvatarController _spawnEnemy(string uuid, UnitMap.Unit unit, Vector3 position)
    {
        AvatarView view = GameObject.Instantiate<AvatarView>(unit.view);
        EnemyState state = EnemyState.Create(uuid, position);
        GruntBrain input = new GruntBrain(_gameSystems, unit.data, state);
        GruntController controller = new GruntController(unit, state, view, input);
        controller.Start(_gameSystems);

        _gameState.enemyStateList.Add(state);
        return controller;
    }

    private string _generateUUID(UnitMap.Unit unit)
    {
        return StringUtil.CreateMD5(unit.id + "_" + _spawnCount);
    }

    private GameplayCamera _getGameplayCamera()
    {
        if (_camera != null)
        {
            return _camera;
        }

        _camera = GameObject.FindObjectOfType<GameplayCamera>();
        if (_camera == null)
        {
            _camera = GameObject.Instantiate<GameplayCamera>(Singleton.instance.gameplayResources.gameplayCamera);
        }
        return _camera;
    }
}

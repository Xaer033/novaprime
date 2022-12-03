using System;
using System.Collections.Generic;
using Fusion;
using GhostGen;
using UnityEngine;

public class AvatarSystem : NotificationDispatcher, IGameSystem
{
    private GameState         _gameState;
    private GameSystems       _gameSystems;
    private NetworkSystem     _networkSystem;
    private NetSnapshotSystem _netSnapshotSystem;

    private GameplayResources _gameplayResources;

    private UnitMap      _unitMap;
    private List<string> _removalList;

    private List<IAvatarController>               _avatarControllerList;
    private Dictionary<string, IAvatarController> _avatarLookUpMap;
    private Dictionary<string, FrameInput>        _lastInputMap;
    private Dictionary<NetPlayer, FrameInput>     _lastPlayerInputMap;
    private Dictionary<int, GameplayCamera>       _cameraMap;

    private List<PlayerSpawnPoint> _playerSpawnPointList;
    private List<FrameInput>       _frameInputList;

    private int _spawnCount;

    private GameplayCamera _camera;
    private GameObject     _playerParent;
    private GameObject     _enemyParent;

    // private IPunPrefabPool _oldPool;
    // private IPunPrefabPool _prefabPool;


    public int priority { get; set; }

    public AvatarSystem(GameplayResources gameplayResources)
    {
        _gameplayResources = gameplayResources;
        _unitMap           = _gameplayResources.unitMap;

        _avatarControllerList = new List<IAvatarController>(200);
        _avatarLookUpMap      = new Dictionary<string, IAvatarController>();
        _frameInputList       = new List<FrameInput>();
        _lastInputMap         = new Dictionary<string, FrameInput>();
        _lastPlayerInputMap   = new Dictionary<NetPlayer, FrameInput>();
        _cameraMap            = new Dictionary<int, GameplayCamera>();
        _playerSpawnPointList = new List<PlayerSpawnPoint>(16);
        _removalList          = new List<string>();

        _playerParent = new GameObject("PlayerParent");
        _enemyParent  = new GameObject("EnemyParent");


        var spawnList = GameObject.FindObjectsOfType<PlayerSpawnPoint>();
        _playerSpawnPointList.Clear();
        _playerSpawnPointList.AddRange(spawnList);
    }

    public void Start(bool hasAuthority, GameSystems gameSystems, GameState gameState)
    {
        _gameSystems = gameSystems;
        _gameState   = gameState;

        _networkSystem     = _gameSystems.Get<NetworkSystem>();
        _netSnapshotSystem = _gameSystems.Get<NetSnapshotSystem>();
        //
        // _netSnapshotSystem.onInterpolationUpdate += onNetInterplationUpdate;

        _gameSystems.onStep      += onStep;
        _gameSystems.onFixedNetworkStep += onFixedStep;
        _gameSystems.AddListener(GamePlayEventType.SPAWN_POINT_TRIGGERED, onSpawnPointTriggered);

        _gameState.playerStateList.Clear();
        _gameState.enemyStateList.Clear();
    }

    private void onFixedStep(NetworkRunner runner, NetSimulator netSim)
    {
        uint tick = NetworkManager.frameTick;

        for (int i = 0; i < _avatarControllerList.Count; ++i)
        {
            IAvatarController controller = _avatarControllerList[i];
            IInputGenerator   inputGen   = controller.input;
            FrameInput        frameInput = default(FrameInput);

            frameInput = _lastInputMap[controller.uuid];
            if (NetworkServer.active || controller.isSimulating)
            {
                _frameInputList.Add(frameInput);
                controller?.FixedStep(runner.DeltaTime, frameInput);
            }

            controller?.input?.Clear();
        }


        // Batch Remove anything that needs to go
        clearRemovedAvatars();

        // if (Keyboard.current.f10Key.wasPressedThisFrame)
        // {
        //     SaveToFile();
        // }
    }

    private void onStep(float deltaTime)
    {
        for (int i = 0; i < _avatarControllerList.Count; ++i)
        {
            IAvatarController controller     = _avatarControllerList[i];
            IInputGenerator   inputGenerator = controller.input;

            if (inputGenerator != null)
            {
                _lastInputMap[controller.uuid] = inputGenerator.GetInput();

                PlayerState pState = controller.state as PlayerState;
                if (pState != null)
                {
                    pState.latestInput = new PlayerInputTickPair
                    {
                        input = _lastInputMap[controller.uuid],
                        tick  = NetworkManager.frameTick,
                    };
                }
            }
            controller.Step(deltaTime);
        }
    }

    public void CleanUp()
    {
        foreach (var pair in _cameraMap)
        {
            pair.Value.ClearTargets();
        }

        for (int i = 0; i < _avatarControllerList.Count; ++i)
        {
            IAvatarView view = _avatarControllerList[i].view;
            _avatarControllerList[i].CleanUp();

            if (view != null && view.gameObject != null)
            {
                GameObject.Destroy(view.gameObject);
            }
        }

        GameObject.Destroy(_playerParent);
        GameObject.Destroy(_enemyParent);

        _avatarControllerList.Clear();
        _gameState.playerStateList.Clear();
        _gameState.enemyStateList.Clear();

        _gameSystems.onStep      -= onStep;
        _gameSystems.onFixedNetworkStep -= onFixedStep;
        _gameSystems.RemoveListener(GamePlayEventType.SPAWN_POINT_TRIGGERED, onSpawnPointTriggered);

        // PhotonNetwork.PrefabPool = _oldPool;
    }

    public PlayerSpawnPoint GetSpawnPointForSlot(PlayerSlot slot)
    {
        for (int i = 0; i < _playerSpawnPointList.Count; ++i)
        {
            PlayerSpawnPoint point = _playerSpawnPointList[i];
            if (slot == point.slot)
            {
                return point;
            }
        }

        return null;
    }

    public Dictionary<string, FrameInput> GetInputMap()
    {
        return _lastInputMap;
    }

    public IAvatarController GetController(string uuid)
    {
        IAvatarController controller = null;
        if (!_avatarLookUpMap.TryGetValue(uuid, out controller))
        {
            Debug.LogError("Could not retrieve Controller with uuid: " + uuid);
        }

        return controller;
    }

    public IAvatarController GetPlayerByNetId(uint netId)
    {
        for (int i = 0; i < _avatarControllerList.Count; ++i)
        {
            IAvatarController controller = _avatarControllerList[i];

            if (controller == null)
                continue;

            // if (controller.view?.netIdentity.netId == netId)
            //     return controller;
        }
        return null;
    }

    public T Spawn<T>(string unitId, Vector2 position, SpawnPointData spawnPointData = default) //where T : IAvatarController
    {
        UnitMap.Unit unit = _unitMap.GetUnit(unitId);
        string       uuid = _generateUUID(unit);

        IAvatarController controller = _spawnAvatar(uuid, unit, position, spawnPointData);

        if (controller != null)
        {
            controller.view.gameObject.name = uuid;
            _avatarControllerList.Add(controller);
            _avatarLookUpMap.Add(uuid, controller);

            _gameSystems.DispatchEvent(GamePlayEventType.AVATAR_SPAWNED, false, controller);
        }
        return (T)controller;
    }

    public void UnSpawn(GameObject obj)
    {
        string uuid = obj.name;
        _removalList.Add(uuid);
    }

    private IAvatarController _spawnAvatar(string uuid, UnitMap.Unit unit, Vector2 position, SpawnPointData spawnPointData)
    {
        switch (unit.type)
        {
            case UnitType.PLAYER: return _spawnPlayer(uuid, unit, position, spawnPointData);
            case UnitType.ENEMY:  return _spawnEnemy(uuid, unit, position, spawnPointData);
        }
        Debug.LogError("Can't spawn type: " + unit.type);
        return null;
    }

    private void _spawnPlayer(string uuid, UnitMap.Unit unit, Vector3 position, SpawnPointData spawnPointData, Action<PlayerController> onSpawnComplete)
    {
        PlayerSlot pSlot = (PlayerSlot)spawnPointData.customInt;


        // _networkManager.runner.Spawn(unit.view,)
        
        AvatarView       view       = GameObject.Instantiate<AvatarView>(unit.view as AvatarView, position, Quaternion.identity, _playerParent.transform);
        PlayerState      state      = PlayerState.Create(uuid, unit.stats, position);
        PlayerController controller = new PlayerController(unit, state, view, null);

        _gameState.playerStateList.Add(state);

        controller.Start(_gameSystems);
        // return controller;
    }

    private IAvatarController _spawnEnemy(string uuid, UnitMap.Unit unit, Vector2 position, SpawnPointData spawnPointData)
    {
        AvatarView view  = GameObject.Instantiate<AvatarView>(unit.view as AvatarView, position, Quaternion.identity, _enemyParent.transform);
        EnemyState state = EnemyState.Create(uuid, unit.stats, position);

        IInputGenerator   input      = _createEnemyGenerator(unit, state);
        IAvatarController controller = _createAvatarController(unit, state, view, input);

        _gameState.enemyStateList.Add(state);
        controller.Start(_gameSystems);

        return controller;
    }

    private string _generateUUID(UnitMap.Unit unit)
    {
        return StringUtil.CreateMD5(unit.id + "_" + (_spawnCount++));
    }

    private IInputGenerator _createEnemyGenerator(UnitMap.Unit unit, EnemyState state)
    {
        switch (unit.id)
        {
            case "grunt":   return new GruntBrain(_gameSystems, unit.stats, state);
            case "bruiser": return new GruntBrain(_gameSystems, unit.stats, state);
        }

        return null;
    }

    private IAvatarController _createAvatarController(UnitMap.Unit unit,
        AvatarState state,
        IAvatarView view,
        IInputGenerator inputGenerator)
    {
        switch (unit.id)
        {
            case "player":  return new PlayerController(unit, state, view, inputGenerator);
            case "grunt":   return new GruntController(unit, state, view, inputGenerator);
            case "bruiser": return new BruiserController(unit, state, view, inputGenerator);
        }

        return null;
    }

    private void onSpawnPointTriggered(GeneralEvent e)
    {
        SpawnPointData spawnData = (SpawnPointData)e.data;
        if (spawnData.spawnType == SpawnType.AVATAR)
        {
            string  unitId   = spawnData.subtypeId;
            Vector3 position = spawnData.position;
            Spawn<IAvatarController>(unitId, position, spawnData);
        }
    }


    private void clearRemovedAvatars()
    {
        while (_removalList.Count > 0)
        {
            int    removeIndex = _removalList.Count - 1;
            string uuid        = _removalList[removeIndex];

            _removalList.RemoveAt(removeIndex);

            IAvatarController controller = _avatarLookUpMap[uuid];
            controller.CleanUp();

            _avatarControllerList.Remove(controller);
            _avatarLookUpMap.Remove(uuid);
            _lastInputMap.Remove(uuid);

            if (controller.unit.type == UnitType.PLAYER)
            {
                _gameState.playerStateList.Remove(controller.state as PlayerState);
            }
            else if (controller.unit.type == UnitType.ENEMY)
            {
                _gameState.enemyStateList.Remove(controller.state as EnemyState);
            }

            GameObject.Destroy(controller.view.gameObject);
            
        }
    }

    private void onNetInterplationUpdate(float alpha, GameState.Snapshot from, GameState.Snapshot to)
    {
        var fromPlayerList = from.playerStateList;
        fromPlayerList.Sort((a, b) => { return a.netId.CompareTo(b.netId); });

        var toPlayerList = to.playerStateList;
        toPlayerList.Sort((a, b) => { return a.netId.CompareTo(b.netId); });

        int maxCount = Math.Max(toPlayerList.Count, fromPlayerList.Count);

        for (int i = 0; i < maxCount; ++i)
        {
            if (i >= toPlayerList.Count ||
                i >= fromPlayerList.Count ||
                i >= _avatarControllerList.Count)
            {
                break;
            }

            var controller = _avatarControllerList[i];
            if (controller.isSimulating)
            {
                continue;
            }

            var state = controller.state;

            int fromIndex = getStateIndexForList(state.netId, fromPlayerList);
            int toIndex   = getStateIndexForList(state.netId, toPlayerList);

            if (fromIndex < 0 || toIndex < 0)
            {
                continue;
            }

            var fromSnapshot = fromPlayerList[fromIndex];
            var toSnapshot   = toPlayerList[toIndex];

            state.prevPosition = state.position;
            state.position     = Vector2.Lerp(fromSnapshot.position, toSnapshot.position, alpha);
            state.aimPosition  = Vector2.Lerp(fromSnapshot.aimPosition, toSnapshot.aimPosition, alpha);

            controller.view.transform.position = state.position;
            controller.view.viewRoot.position  = state.position;
            controller.view.Aim(state.aimPosition);
        }
    }

    private int getStateIndexForList(uint netId, List<PlayerState.NetSnapshot> snapshotList)
    {
        int result = -1;

        for (int i = 0; i < snapshotList.Count; ++i)
        {
            if (snapshotList[i].netId == netId)
            {
                result = i;
                break;
            }
        }

        return result;
    }

}

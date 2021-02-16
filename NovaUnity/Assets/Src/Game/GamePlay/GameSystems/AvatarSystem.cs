using System.Collections.Generic;
using GhostGen;
using Mirror;
using Telepathy;
using UnityEngine;
using UnityEngine.InputSystem;

public class AvatarSystem : NotificationDispatcher, IGameSystem
{
    private GameState _gameState;
    private GameSystems _gameSystems;
    private NetworkSystem _networkSystem;
    
    private GameplayResources _gameplayResources;
    
    private UnitMap _unitMap;
    private List<IAvatarController> _avatarControllerList;
    private Dictionary<string, IAvatarController> _avatarLookUpMap;
    private Dictionary<string, FrameInput> _lastInputMap;
    private Dictionary<NetPlayer, FrameInput> _lastPlayerInputMap;
    private Dictionary<int, GameplayCamera> _cameraMap;

    private List<string> _removalList;

    private List<PlayerSpawnPoint> _playerSpawnPointList;
    private List<FrameInput> _frameInputList;

    private int _spawnCount;
    
    private GameplayCamera _camera;
    private GameObject _playerParent;
    private GameObject _enemyParent;

    // private IPunPrefabPool _oldPool;
    // private IPunPrefabPool _prefabPool;
    
    
    public int priority { get; set; }

    public AvatarSystem(GameplayResources gameplayResources)
    {
        _gameplayResources = gameplayResources;
        _unitMap = _gameplayResources.unitMap;
        
        _avatarControllerList           = new List<IAvatarController>(200);
        _avatarLookUpMap                = new Dictionary<string, IAvatarController>();
        _frameInputList                 = new List<FrameInput>();
        _lastInputMap                   = new Dictionary<string, FrameInput>();
        _lastPlayerInputMap             = new Dictionary<NetPlayer, FrameInput>();
        _cameraMap                      = new Dictionary<int, GameplayCamera>();
        _playerSpawnPointList           = new List<PlayerSpawnPoint>(16);
        _removalList                    = new List<string>();
        
        _playerParent                   = new GameObject("PlayerParent");
        _enemyParent                    = new GameObject("EnemyParent");


        var spawnList = GameObject.FindObjectsOfType<PlayerSpawnPoint>();
        _playerSpawnPointList.Clear();
        _playerSpawnPointList.AddRange(spawnList);
    }
    
    public void Start(GameSystems gameSystems, GameState gameState)
    {
        _gameSystems = gameSystems;
        _gameState = gameState;

        _networkSystem = _gameSystems.Get<NetworkSystem>();
        
        _gameSystems.onStep += Step;
        _gameSystems.onFixedStep += FixedStep;
        _gameSystems.AddListener(GamePlayEventType.SPAWN_POINT_TRIGGERED, onSpawnPointTriggered);
        
        _gameState.playerStateList.Clear();
        _gameState.enemyStateList.Clear();

        
        // PhotonNetwork.PrefabPool = _prefabPool;
    }

    public void FixedStep(float fixedDeltaTime)
    {
        for(int i = 0; i < _avatarControllerList.Count; ++i)
        {
            IAvatarController controller = _avatarControllerList[i];
            
            PlayerInputTickPair newInputPair;
            
            bool hasInput = _networkSystem.GetInputForTick(
                ((PlayerState)controller.state).playerSlot, 
                out newInputPair);
            
            if(!hasInput && _lastInputMap.ContainsKey(controller.uuid))
            {
                newInputPair.input =  _lastInputMap[controller.uuid];     
            }

            if(NetworkServer.active || controller.isSimulating)
            {
                _frameInputList.Add(newInputPair.input);

                controller.FixedStep(fixedDeltaTime, newInputPair.input);

                PlayerState pState = (PlayerState)controller.state;
                if(pState != default(PlayerState)) // Check again to make sure only the client does this part
                {
                    pState.nonAckInputBuffer.PushBack(newInputPair);
                    pState.nonAckStateBuffer.PushBack(PlayerState.Clone(pState));   
                }
            }

            controller?.input?.Clear();
        }
    

        // Batch Remove anything that needs to go
        clearRemovedAvatars();
        
        if (Keyboard.current.f10Key.wasPressedThisFrame)
        {
            SaveToFile();
        }
    }

    public void Step(float deltaTime)
    {
        FrameInput defaultInput = default(FrameInput);
        
        for (int i = 0; i < _avatarControllerList.Count; ++i)
        {
            IAvatarController controller = _avatarControllerList[i];
            IInputGenerator inputGenerator = controller.input;

            if(inputGenerator != null)
            {
                _lastInputMap[controller.uuid] = inputGenerator.GetInput();

                PlayerState pState = controller.state as PlayerState;
                if(pState != null)
                {
                    pState.latestInput = new PlayerInputTickPair
                    {
                        input = _lastInputMap[controller.uuid]
                    };
                }
            } 
            controller.Step(deltaTime);
        }
    }

    public void CleanUp()
    {
        foreach(var pair in _cameraMap)
        {
            pair.Value.ClearTargets();
        }
        
        for (int i = 0; i < _avatarControllerList.Count; ++i)
        {
            IAvatarView view = _avatarControllerList[i].view;
            _avatarControllerList[i].CleanUp();
            
            GameObject.Destroy(view.gameObject);
        }
        
        GameObject.Destroy(_playerParent);
        GameObject.Destroy(_enemyParent);
        
        _avatarControllerList.Clear();
        _gameState.playerStateList.Clear();
        _gameState.enemyStateList.Clear();
        
        _gameSystems.onStep -= Step;
        _gameSystems.onFixedStep -= FixedStep;
        _gameSystems.RemoveListener(GamePlayEventType.SPAWN_POINT_TRIGGERED, onSpawnPointTriggered);

        // PhotonNetwork.PrefabPool = _oldPool;
    }

    public PlayerSpawnPoint GetSpawnPointForSlot(PlayerSlot slot)
    {
        for(int i = 0; i < _playerSpawnPointList.Count; ++i)
        {
            PlayerSpawnPoint point = _playerSpawnPointList[i];
            if(slot == point.slot)
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
        if(!_avatarLookUpMap.TryGetValue(uuid, out controller))
        {
            Debug.LogError("Could not retrieve Controller with uuid: " + uuid);
        }

        return controller;
    }

    public IAvatarController GetPlayerByNetId(uint netId)
    {
        for(int i = 0; i < _avatarControllerList.Count; ++i)
        {
            IAvatarController controller = _avatarControllerList[i];
            
            if(controller == null) 
                continue;
            
            if(controller?.view?.netIdentity.netId == netId)
                return controller;        
        }
        return null;
    }
    
    public T Spawn<T>(string unitId, Vector3 position, SpawnPointData spawnPointData = default) //where T : IAvatarController
    {
        UnitMap.Unit unit = _unitMap.GetUnit(unitId);
        string uuid = _generateUUID(unit);
        
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
    
    private IAvatarController _spawnAvatar(string uuid, UnitMap.Unit unit, Vector3 position, SpawnPointData spawnPointData)
    {
        switch (unit.type)
        {
            case UnitType.PLAYER:    return _spawnPlayer(uuid, unit, position, spawnPointData);
            case UnitType.ENEMY:     return _spawnEnemy(uuid, unit, position, spawnPointData);
        }
        Debug.LogError("Can't spawn type: " + unit.type);
        return null;
    }

    private PlayerController _spawnPlayer(string uuid, UnitMap.Unit unit, Vector3 position, SpawnPointData spawnPointData)
    {
        // NetPlayer netPlayer = null;
        
        PlayerSlot pSlot = (PlayerSlot)spawnPointData.customInt;
        
        AvatarView view =  GameObject.Instantiate<AvatarView>(unit.view as AvatarView, position, Quaternion.identity, _playerParent.transform);
 

        PlayerState state = PlayerState.Create(uuid, unit.stats, position);
        PlayerController controller = new PlayerController(unit, state, view, null);
      
        _gameState.playerStateList.Add(state); 
        
        controller.Start(_gameSystems);
        return controller;
    }

    private IAvatarController _spawnEnemy(string uuid, UnitMap.Unit unit, Vector3 position, SpawnPointData spawnPointData)
    {
        AvatarView view = GameObject.Instantiate<AvatarView>(unit.view as AvatarView, position, Quaternion.identity,  _enemyParent.transform);
        
        EnemyState state = EnemyState.Create(uuid, unit.stats, position);

        IInputGenerator input = _createEnemyGenerator(unit, state);
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
        switch(unit.id)
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
        switch(unit.id)
        {
            case "player":    return new PlayerController(unit, state, view, inputGenerator);
            case "grunt":     return new GruntController(unit, state, view, inputGenerator);
            case "bruiser":   return new BruiserController(unit, state, view, inputGenerator);
        }

        return null;
    }

    private void onSpawnPointTriggered(GeneralEvent e)
    {
        SpawnPointData spawnData = (SpawnPointData)e.data;
        if (spawnData.spawnType == SpawnType.AVATAR)
        {
            string unitId = spawnData.subtypeId;
            Vector3 position = spawnData.position;
            Spawn<IAvatarController>(unitId, position, spawnData);
        }
    }


    private void clearRemovedAvatars()
    {
        while(_removalList.Count > 0)
        {
            int removeIndex = _removalList.Count - 1;
            string uuid = _removalList[removeIndex];
            _removalList.RemoveAt(removeIndex);
            
            IAvatarController controller = _avatarLookUpMap[uuid];
            controller.CleanUp();
            
            _avatarControllerList.Remove(controller);
            _avatarLookUpMap.Remove(uuid);
            _lastInputMap.Remove(uuid);
            
            if(controller.unit.type == UnitType.PLAYER)
            {
                _gameState.playerStateList.Remove(controller.state as PlayerState);
            }
            else if(controller.unit.type == UnitType.ENEMY)
            {
                _gameState.enemyStateList.Remove(controller.state as EnemyState);
            }
            
            GameObject.Destroy(controller.view.gameObject);
        }
    }
    private void SaveToFile()
    {
            
//        StreamWriter writer = new StreamWriter("Assets/Resources/inputList.txt", false, Encoding.UTF8);
        // BinaryWriter writer = new BinaryWriter(File.Open("Assets/Resources/inputList.dat", FileMode.Create));
        //
        // JsonWriter jsonWriter = new BsonWriter(writer);
        // jsonWriter.Formatting = Formatting.Indented;
        // jsonWriter.WriteStartArray();
        // foreach (var input in _frameInputList)
        // {
        //     string jsonInput = JsonUtility.ToJson(input);
        //     jsonWriter.WriteValue(jsonInput);
        // }
        // jsonWriter.WriteEndArray();
        // jsonWriter.Flush();
        // jsonWriter.Close();
    }
//    private static byte[] ToByteArray(PlayerState command)
//    {
//        string jsonCommand = JsonUtility.ToJson(command);
//        return System.Text.Encoding.UTF8.GetBytes(jsonCommand);
//    }


    // public class AvatarPrefabPool : IPunPrefabPool
    // {
    //     private GameplayResources _gameplayResources;
    //     private UnitMap _unitMap;
    //     
    //     public AvatarPrefabPool(GameplayResources gameplayResources)
    //     {
    //         _gameplayResources = gameplayResources;
    //         _unitMap = _gameplayResources.unitMap;
    //     }
    //     public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
    //     {
    //         // TODO: Later we can do real pooling, right now, just use the unit map to get the right prefab and boop it to the seen
    //         UnitMap.Unit unit = _unitMap.GetUnit(prefabId);
    //         GameObject gObject = GameObject.Instantiate(unit.view.gameObject, position, rotation);
    //         return gObject;
    //     }
    //
    //     public void Destroy(GameObject gameObject)
    //     {
    //         // TODO: Actually Recycle this bitch instead of destroying it
    //         GameObject.Destroy(gameObject);
    //     }
    // }
}



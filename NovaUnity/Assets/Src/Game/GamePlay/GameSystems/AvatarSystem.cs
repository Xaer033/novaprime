using System.Collections.Generic;
using System.IO.Compression;
using GhostGen;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.InputSystem;

public class AvatarSystem : NotificationDispatcher, IGameSystem
{
    public const int kMaxPlayerCount = 4;

    private GameState _gameState;
    private GameSystems _gameSystems;

    private GameplayResources _gameplayResources;
    
    private UnitMap _unitMap;
    private List<IAvatarController> _avatarControllerList;
    private Dictionary<string, IAvatarController> _avatarLookUpMap;
    private Dictionary<string, FrameInput> _lastInputMap;
    private Dictionary<int, GameplayCamera> _cameraMap;
    
    private List<FrameInput> _frameInputList;

    private int _spawnCount;
    
    private GameplayCamera _camera;
    private GameObject _playerParent;
    private GameObject _enemyParent;

    private IPunPrefabPool _oldPool;
    private IPunPrefabPool _prefabPool;
    
    
    public int priority { get; set; }

    public AvatarSystem(GameplayResources gameplayResources)
    {
        _gameplayResources = gameplayResources;
        _unitMap = _gameplayResources.unitMap;
        _avatarControllerList        = new List<IAvatarController>(200);
        _avatarLookUpMap             = new Dictionary<string, IAvatarController>();
        _frameInputList              = new List<FrameInput>();
        _lastInputMap                = new Dictionary<string, FrameInput>();
        _cameraMap = new Dictionary<int, GameplayCamera>();
        
        _playerParent    = new GameObject("PlayerParent");
        _enemyParent     = new GameObject("EnemyParent");

        _oldPool = PhotonNetwork.PrefabPool;
        _prefabPool = new AvatarPrefabPool(_gameplayResources);

    }
    
    public void Start(GameSystems gameSystems, GameState gameState)
    {
        _gameSystems = gameSystems;
        _gameState = gameState;
        _gameSystems.onStep += Step;
        _gameSystems.onFixedStep += FixedStep;
        _gameSystems.AddListener(GamePlayEventType.SPAWN_POINT_TRIGGERED, onSpawnPointTriggered);
        
        _gameState.playerStateList.Clear();
        _gameState.enemyStateList.Clear();

        PhotonNetwork.PrefabPool = _prefabPool;
    }

    public void FixedStep(float fixedDeltaTime)
    {
        for (int i = 0; i < _avatarControllerList.Count; ++i)
        {
            IAvatarController controller = _avatarControllerList[i];
            IInputGenerator inputGenerator = controller.input;
            
            FrameInput input = _lastInputMap.ContainsKey(controller.uuid) ? _lastInputMap[controller.uuid] : default(FrameInput);
           
            _frameInputList.Add(input);            
            // This is kinda cool, cuz now we can swap input generators or save/store them for replays
            _avatarControllerList[i].FixedStep(fixedDeltaTime, input);
        }
        
        if (Keyboard.current.f10Key.wasPressedThisFrame)
        {
            SaveToFile();
        }
    }

    public void Step(float deltaTime)
    {
        for (int i = 0; i < _avatarControllerList.Count; ++i)
        {
            IAvatarController controller = _avatarControllerList[i];
            IInputGenerator inputGenerator = controller.input;

            FrameInput frameInput = inputGenerator != null ? inputGenerator.GetInput() : default(FrameInput);
            _lastInputMap[controller.uuid] = frameInput;
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
            PhotonNetwork.Destroy(view.gameObject);
        }
        
        GameObject.Destroy(_playerParent);
        GameObject.Destroy(_enemyParent);
        
        _avatarControllerList.Clear();
        _gameState.playerStateList.Clear();
        _gameState.enemyStateList.Clear();
        
        _gameSystems.onStep -= Step;
        _gameSystems.onFixedStep -= FixedStep;
        _gameSystems.RemoveListener(GamePlayEventType.SPAWN_POINT_TRIGGERED, onSpawnPointTriggered);

        PhotonNetwork.PrefabPool = _oldPool;
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
        NetworkPlayer netPlayer = null;
        Player photonPlayer = null;
        
        int playerIndex = spawnPointData.customInt;
        bool isOwnerOfPlayer = true;
        if(playerIndex < _gameState.netPlayerList.Count)
        {
            netPlayer = _gameState.netPlayerList[playerIndex];
            photonPlayer = PhotonNetwork.CurrentRoom.GetPlayer(netPlayer.number);
            if(photonPlayer == null || !photonPlayer.IsLocal)
            {
                isOwnerOfPlayer = false;
            }
        }
        else
        {
            return null;
        }

        GameObject avatarGameObject = null;
        AvatarView view = null;
        
        if(isOwnerOfPlayer)
        {
            avatarGameObject = PhotonNetwork.Instantiate(unit.id, position, Quaternion.identity);
        }

        if(avatarGameObject != null)
        {
            avatarGameObject.transform.SetParent(_playerParent.transform);
            view = avatarGameObject.GetComponent<AvatarView>();
        }


        GameplayCamera cam = _getOrCreatePlayerCamera(playerIndex);
        if (cam != null)
        {
            if(photonPlayer.IsLocal)
            {
                cam.AddTarget(view.cameraTargetGroup.transform);
            }
        }
        else
        {
            Debug.LogError("Could not find or create gameplay camera!");
        }

        PlayerState state = PlayerState.Create(uuid, unit.stats, position);
        state.playerNumber = netPlayer.number;
        PlayerInput input = new PlayerInput(playerIndex, cam.gameCamera);
        PlayerController controller = new PlayerController(unit, state, view, input);
        controller.isSimulating = true;
        
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
        controller.Start(_gameSystems);

        _gameState.enemyStateList.Add(state);
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
    
    private GameplayCamera _getOrCreatePlayerCamera(int playerNumber)
    {
        if (_camera != null)
        {
            return _camera;
        }

        _camera = GameObject.FindObjectOfType<GameplayCamera>();
        if (_camera == null)
        {
            _camera = GameObject.Instantiate<GameplayCamera>(_gameplayResources.gameplayCamera);
        }
        return _camera;
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


    public class AvatarPrefabPool : IPunPrefabPool
    {
        private GameplayResources _gameplayResources;
        private UnitMap _unitMap;
        
        public AvatarPrefabPool(GameplayResources gameplayResources)
        {
            _gameplayResources = gameplayResources;
            _unitMap = _gameplayResources.unitMap;
        }
        public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
        {
            // TODO: Later we can do real pooling, right now, just use the unit map to get the right prefab and boop it to the seen
            UnitMap.Unit unit = _unitMap.GetUnit(prefabId);
            GameObject gObject = GameObject.Instantiate(unit.view.gameObject, position, rotation);
            return gObject;
        }

        public void Destroy(GameObject gameObject)
        {
            // TODO: Actually Recycle this bitch instead of destroying it
            GameObject.Destroy(gameObject);
        }
        
    }
}



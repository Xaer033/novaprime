using System.Collections.Generic;
using GhostGen;
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

    private List<FrameInput> _frameInputList;

    private int _spawnCount;
    
    private GameplayCamera _camera;
    private GameObject _playerParent;
    private GameObject _enemyParent;
    
    
    public int priority { get; set; }
    
    public AvatarSystem(GameplayResources gameplayResources)
    {
        _gameplayResources = gameplayResources;
        _unitMap = _gameplayResources.unitMap;
        _avatarControllerList        = new List<IAvatarController>(200);
        _avatarLookUpMap             = new Dictionary<string, IAvatarController>();
        _frameInputList              = new List<FrameInput>();
        _lastInputMap                = new Dictionary<string, FrameInput>();
        
        _playerParent    = new GameObject("PlayerParent");
        _enemyParent     = new GameObject("EnemyParent");
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
        GameplayCamera cam = _getGameplayCamera();
        cam.ClearTargets();
        
        for (int i = 0; i < _avatarControllerList.Count; ++i)
        {
            IAvatarView view = _avatarControllerList[i].view;
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
    
    public T Spawn<T>(string unitId, Vector3 position) where T : IAvatarController
    {
        UnitMap.Unit unit = _unitMap.GetUnit(unitId);
        string uuid = _generateUUID(unit);
        
        IAvatarController controller = _spawnAvatar(uuid, unit, position);

        if (controller != null)
        {
            controller.view.gameObject.name = uuid;
            _avatarControllerList.Add(controller);
            _avatarLookUpMap.Add(uuid, controller);
            
            _gameSystems.DispatchEvent(GamePlayEventType.AVATAR_SPAWNED, false, controller);
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
        AvatarView view = GameObject.Instantiate<AvatarView>(unit.view as AvatarView, position, Quaternion.identity, _playerParent.transform);

        GameplayCamera cam = _getGameplayCamera();
        if (cam != null)
        {
            cam.AddTarget(view.cameraTargetGroup.transform);
        }
        else
        {
            Debug.LogError("Could not find or create gameplay camera! Aborting player creation");
        }
        
        PlayerState state = PlayerState.Create(uuid, unit.stats, position);

        PlayerInput input = new PlayerInput(0, cam.gameCamera);
        PlayerController controller = new PlayerController(unit, state, view, input);
        controller.Start(_gameSystems);

        _gameState.playerStateList.Add(state); 
        return controller;
    }

    private IAvatarController _spawnEnemy(string uuid, UnitMap.Unit unit, Vector3 position)
    {
        AvatarView view = GameObject.Instantiate<AvatarView>(unit.view as AvatarView, position, Quaternion.identity,  _enemyParent.transform);
        
        EnemyState state = EnemyState.Create(uuid, unit.stats, position);

        IInputGenerator input = _createEnemyGenerator(unit, state);// new GruntBrain(_gameSystems, unit.stats, state);
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
    
    private GameplayCamera _getGameplayCamera()
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
            Spawn<IAvatarController>(unitId, position);
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
}

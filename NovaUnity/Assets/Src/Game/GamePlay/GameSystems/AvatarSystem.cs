using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using GhostGen;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using UnityEngine;
using UnityEngine.InputSystem;

public class AvatarSystem : NotificationDispatcher, IGameSystem
{
    public const int kMaxPlayerCount = 4;

    private GameState _gameState;
    private GameSystems _gameSystems;

    private UnitMap _unitMap;
    private List<IAvatarController>      _avatarControllerList;
    private Dictionary<string, IAvatarController> _avatarLookUpMap;

    private List<FrameInput> _frameInputList;
    
    private GameplayCamera _camera;
    private static int _spawnCount = 0;
    private int _fixedStepCount;
    
    public AvatarSystem()
    {
        _unitMap = Singleton.instance.gameplayResources.unitMap;
        _avatarControllerList       = new List<IAvatarController>(200);
        _avatarLookUpMap            = new Dictionary<string, IAvatarController>();
        _frameInputList             = new List<FrameInput>();

//        _playerActionList         = new List<PlayerActions>(kMaxPlayerCount);
    }
    
    public void Start(GameSystems gameSystems, GameState gameState)
    {
        _gameSystems = gameSystems;
        _gameState = gameState;
        _fixedStepCount = 0;
        
        _gameState.playerStateList.Clear();
        _gameState.enemyStateList.Clear();
        _gameSystems.AddListener(GamePlayEventType.SPAWN_POINT_TRIGGERED, onSpawnPointTriggered);
    }

    public void FixedStep(float deltaTime)
    {
        for (int i = 0; i < _avatarControllerList.Count; ++i)
        {
            IInputGenerator inputGenerator = _avatarControllerList[i].GetInput();
            FrameInput input = inputGenerator != null ? inputGenerator.GetInput() : default(FrameInput);
            _frameInputList.Add(input);            
            // This is kinda cool, cuz now we can swap input generators or save/store them for replays
            _avatarControllerList[i].FixedStep(deltaTime, input);
        }

        if (Keyboard.current.f10Key.wasPressedThisFrame)
        {
            SaveToFile();
        }

        _fixedStepCount++;
    }

    public void Step(float deltaTime)
    {
        for (int i = 0; i < _avatarControllerList.Count; ++i)
        {
            _avatarControllerList[i].Step(deltaTime);
        }
    }

    public void LateStep(float deltaTime)
    {
        
    }

    public void CleanUp()
    {
        _gameSystems.RemoveListener(GamePlayEventType.SPAWN_POINT_TRIGGERED, onSpawnPointTriggered);
        
        
        GameplayCamera cam = _getGameplayCamera();
        cam.ClearTargets();
        
        for (int i = 0; i < _avatarControllerList.Count; ++i)
        {
            AvatarView view = _avatarControllerList[i].GetView();
            GameObject.Destroy(view.gameObject);
        }
        _avatarControllerList.Clear();
        
        _gameState.playerStateList.Clear();
        _gameState.enemyStateList.Clear();
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
        
        PlayerState state = PlayerState.Create(uuid, unit.stats, position);

        PlayerInput input = new PlayerInput(0, cam.gameCamera);
        PlayerController controller = new PlayerController(unit, state, view, input);
        controller.Start(_gameSystems);

        _gameState.playerStateList.Add(state); 
        return controller;
    }

    private IAvatarController _spawnEnemy(string uuid, UnitMap.Unit unit, Vector3 position)
    {
        AvatarView view = GameObject.Instantiate<AvatarView>(unit.view, position, Quaternion.identity);
        EnemyState state = EnemyState.Create(uuid, unit.stats, position);
        GruntBrain input = new GruntBrain(_gameSystems, unit.stats, state);
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

    private void onSpawnPointTriggered(GeneralEvent e)
    {
        SpawnPointData spawnData = (SpawnPointData) e.data;
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
        BinaryWriter writer = new BinaryWriter(File.Open("Assets/Resources/inputList.dat", FileMode.Create));
       
        JsonWriter jsonWriter = new BsonWriter(writer);
        jsonWriter.Formatting = Formatting.Indented;
        jsonWriter.WriteStartArray();
        foreach (var input in _frameInputList)
        {
            string jsonInput = JsonUtility.ToJson(input);
            jsonWriter.WriteValue(jsonInput);
        }
        jsonWriter.WriteEndArray();
        jsonWriter.Flush();
        jsonWriter.Close();
    }
//    private static byte[] ToByteArray(PlayerState command)
//    {
//        string jsonCommand = JsonUtility.ToJson(command);
//        return System.Text.Encoding.UTF8.GetBytes(jsonCommand);
//    }
}

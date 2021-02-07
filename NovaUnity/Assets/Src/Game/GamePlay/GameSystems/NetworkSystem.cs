using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using GhostGen;
using Mirror;
using UnityEngine;

public class NetworkSystem : NotificationDispatcher, IGameSystem
{
    private GameSystems _gameSystems;
    private AvatarSystem _avatarSystem;
    
    private NetworkManager _networkManager;
    private UnitMap _unitMap;
    private Dictionary<Guid, UnitMap.Unit> _netPrefabMap;

    private Dictionary<int, IAvatarController> _connToPlayerMap;
    // private Dictionary<string, Guid> _unitIdToGuidMap;
    private GameplayCamera _camera;
    
    public int priority { get; set; }
    

    public NetworkSystem(UnitMap unitMap)
    {
        _networkManager = Singleton.instance.networkManager;
        
        _unitMap = unitMap;
        _netPrefabMap = new Dictionary<Guid, UnitMap.Unit>();
        _connToPlayerMap = new Dictionary<int, IAvatarController>();
        
        // _unitIdToGuidMap = new Dictionary<string, Guid>();

        for(int i = 0; i < _unitMap.unitList.Count; ++i)
        {
            UnitMap.Unit unit = _unitMap.unitList[i];
            Debug.Log("Unit: " + unit.id);
            Guid guid = unit.view.netIdentity.assetId;
            _netPrefabMap[guid] = unit;
        }
    }

    public void Start(GameSystems gameSystems, GameState gameState)
    {
        _gameSystems = gameSystems;
        _avatarSystem = gameSystems.Get<AvatarSystem>();
        
        _networkManager.onClientSpawnHandler += onClientUnitSpawnHandler;
        _networkManager.onClientUnspawnHandler += onClientUnitUnspawnHandler;
        _networkManager.onLocalClientDisconnect += onLocalClientDisconnect;
        
        _networkManager.onServerMatchBegin += onServerMatchBegin;
        _networkManager.onServerSendPlayerInput += onServerSendPlayerInput;
        _networkManager.onServerConnect += onServerConnect;
        _networkManager.onServerDisconnect += onServerDisconnect;
        _networkManager.onServerMatchLoadComplete += onServerMatchLoadComplete;

        _networkManager.frameTick = 0;
        _gameSystems.onFixedStep += onFixedStep;
    }

    public void CleanUp()
    {
        _networkManager.onClientSpawnHandler -= onClientUnitSpawnHandler;
        _networkManager.onClientUnspawnHandler -= onClientUnitUnspawnHandler;
        _networkManager.onLocalClientDisconnect -= onLocalClientDisconnect;
        
        _networkManager.onServerMatchBegin -= onServerMatchBegin;
        _networkManager.onServerSendPlayerInput -= onServerSendPlayerInput;
        _networkManager.onServerConnect -= onServerConnect;
        _networkManager.onServerDisconnect -= onServerDisconnect;
        _networkManager.onServerMatchLoadComplete -= onServerMatchLoadComplete;
        
        _gameSystems.onFixedStep -= onFixedStep;
    }

    private void onFixedStep(float fixedDeltaTime)
    {
        _networkManager.frameTick++;
    }

    private void onServerMatchLoadComplete(NetworkConnection conn)
    {
        if(_networkManager.sessionState == NetworkManager.SessionState.IN_GAME)
        {
            var netPlayerMap = _networkManager.GetServerPlayerMap();
            NetPlayer netPlayer = _networkManager.GetServerPlayerFromConnId(conn.connectionId);
            serverSpawnPlayer(netPlayer);
        }
    }
    
    private void onServerConnect(NetworkConnection conn)
    {
        if(_networkManager.sessionState == NetworkManager.SessionState.IN_GAME)
        {
            conn.Send(new StartMatchLoad(), Channels.DefaultReliable);
        }
    }

    private void onServerDisconnect(NetworkConnection conn)
    {
        NetworkServer.DestroyPlayerForConnection(conn);
    }
    
    private void onServerSendPlayerInput(NetworkConnection conn, SendPlayerInput msg)
    {
        var lastInputMap = _avatarSystem.GetInputMap();
        IAvatarController playerController = _connToPlayerMap[conn.connectionId];
        lastInputMap[playerController.uuid] = msg.input;
    }
    
    private void onServerMatchBegin()
    {
    
        NetworkServer.SendToAll(new MatchBegin(), Channels.DefaultReliable);
                
        NetworkServer.SpawnObjects();
        
        var netPlayerMap = _networkManager.GetServerPlayerMap();
        foreach(var pair in netPlayerMap)
        {
            NetPlayer netPlayer = pair.Value;
            serverSpawnPlayer(netPlayer);
        }
    }
    
    private GameObject onClientUnitSpawnHandler(SpawnMessage msg)
    {
        UnitMap.Unit unit = _netPrefabMap[msg.assetId];
        IAvatarController controller = _avatarSystem.Spawn<IAvatarController>(unit.id, msg.position);
        
        if(msg.isOwner)
        {
            setupLocalPlayer(controller);
        }

        return controller.view.gameObject;
    }

    private void setupLocalPlayer(IAvatarController controller)
    {
         GameplayCamera cam = _getOrCreatePlayerCamera();
         if(cam != null)
         {
             PlayerView pView = controller.view as PlayerView;
             cam.AddTarget(pView.cameraTargetGroup.transform);
             
             PlayerInput input = new PlayerInput(_networkManager.localPlayerSlot, cam.gameCamera);
             
             controller.input = input;
             controller.isSimulating = true;
         }
    }

    private void serverSpawnPlayer(NetPlayer netPlayer)
    {
        PlayerSpawnPoint point = _avatarSystem.GetSpawnPointForSlot(netPlayer.playerSlot);
        if(point == null)
        {
            Debug.LogErrorFormat("No spawn point for slot {0}, player {1} cant spawn!", netPlayer.playerSlot.ToString(), netPlayer.nickName );
            return;
        }

        UnitMap.Unit playerUnit = _unitMap.GetUnit("player");
        IAvatarController controller = _avatarSystem.Spawn<IAvatarController>(playerUnit.id, point.transform.position);
        GameObject spawnedGameObject = controller.view.gameObject;
        
        NetworkConnection conn = NetworkServer.connections[netPlayer.connectionId];
        
        NetworkServer.Spawn(spawnedGameObject, conn);
        NetworkServer.AddPlayerForConnection(conn, spawnedGameObject);
        _connToPlayerMap[conn.connectionId] = controller;
        
        if(_networkManager.localPlayer != null && netPlayer.connectionId == _networkManager.localPlayer.connectionId)
        {
            setupLocalPlayer(controller);
        }
    }
    
    private void onClientUnitUnspawnHandler(GameObject obj)
    {
        _avatarSystem.UnSpawn(obj);
    }

    private void onLocalClientDisconnect(NetworkConnection conn)
    {
        DispatchEvent(GamePlayEventType.NET_LOCAL_PLAYER_DISCONNECT, false, conn);
    }
    
     private GameplayCamera _getOrCreatePlayerCamera()
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

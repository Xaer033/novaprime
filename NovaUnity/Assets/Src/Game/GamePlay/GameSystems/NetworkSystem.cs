using System;
using System.Collections.Generic;
using GhostGen;
using Mirror;
using UnityEngine;

public class NetworkSystem : NotificationDispatcher, IGameSystem
{
    private const int kMaxPlayers = 16;
    
    private const int kTicksAhead = 2;
    
    private GameSystems _gameSystems;
    private GameState _gameState;
    
    private AvatarSystem _avatarSystem;
    
    private NetworkManager _networkManager;
    private UnitMap _unitMap;
    private Dictionary<Guid, UnitMap.Unit> _netPrefabMap;
    private List<NetFrameSnapshot> _snapshotList;
    
    /* Server */
    private Dictionary<int, IAvatarController> _serverConnToPlayerMap;
    private List<IAvatarController> _serverPlayerControllerList;
    private Dictionary<uint, Dictionary<string, FrameInput>> _serverTickInputs;
    private NetPlayerState[] _serverPlayerStateList;
    
    /* Client */
    private GameplayCamera _camera;
    private LocalPlayerState _localPlayer;
    
    
    public int priority { get; set; }
    

    public NetworkSystem(UnitMap unitMap)
    {
        _networkManager = Singleton.instance.networkManager;
        
        _unitMap = unitMap;
        _netPrefabMap = new Dictionary<Guid, UnitMap.Unit>();
        
        _serverConnToPlayerMap = new Dictionary<int, IAvatarController>();
        _serverTickInputs = new Dictionary<uint, Dictionary<string, FrameInput>>();
        _serverPlayerControllerList = new List<IAvatarController>();
        _serverPlayerStateList = new NetPlayerState[kMaxPlayers];
        _snapshotList = new List<NetFrameSnapshot>();
        
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
        Application.targetFrameRate = 60;
        
        _gameSystems = gameSystems;
        _avatarSystem = gameSystems.Get<AvatarSystem>();
        
        _networkManager.onClientSpawnHandler += onClientUnitSpawnHandler;
        _networkManager.onClientUnspawnHandler += onClientUnitUnspawnHandler;
        _networkManager.onLocalClientDisconnect += onLocalClientDisconnect;
        _networkManager.onClientFrameSnapshot += onClientFrameSnapshot;
        
        _networkManager.onServerMatchBegin += onServerMatchBegin;
        _networkManager.onServerSendPlayerInput += onServerSendPlayerInput;
        _networkManager.onServerConnect += onServerConnect;
        _networkManager.onServerDisconnect += onServerDisconnect;
        _networkManager.onServerMatchLoadComplete += onServerMatchLoadComplete;

        _gameSystems.onFixedStep += onFixedStep;
        
        NetworkManager.frameTick = 0;
    }

    public void CleanUp()
    {
        _networkManager.onClientSpawnHandler -= onClientUnitSpawnHandler;
        _networkManager.onClientUnspawnHandler -= onClientUnitUnspawnHandler;
        _networkManager.onLocalClientDisconnect -= onLocalClientDisconnect;
        _networkManager.onClientFrameSnapshot -= onClientFrameSnapshot;
        
        _networkManager.onServerMatchBegin -= onServerMatchBegin;
        _networkManager.onServerSendPlayerInput -= onServerSendPlayerInput;
        _networkManager.onServerConnect -= onServerConnect;
        _networkManager.onServerDisconnect -= onServerDisconnect;
        _networkManager.onServerMatchLoadComplete -= onServerMatchLoadComplete;
        
        _gameSystems.onFixedStep -= onFixedStep;
    }

    public FrameInput GetInputForTick(uint tick, string uuid)
    {
        FrameInput input = default(FrameInput);

        Dictionary<string, FrameInput> playerInputMap;
        if(_serverTickInputs.TryGetValue(tick, out playerInputMap))
        {
            playerInputMap.TryGetValue(uuid, out input);
        }
        
        return input;
    }
    
    private void onFixedStep(float fixedDeltaTime)
    {
        
        if(NetworkServer.active)
        {
            int currentPlayerCount = _serverPlayerControllerList.Count;
            for(int i = 0; i < currentPlayerCount; ++i)
            {
                IAvatarController pController = _serverPlayerControllerList[i];
                AvatarState state = pController.state;

                NetPlayerState netPlayerState = new NetPlayerState
                {
                    position = state.position,
                    velocity = state.velocity,
                    aimPosition = state.aimPosition,
                    netId = pController.view.netIdentity.netId
                };

                _serverPlayerStateList[i] = netPlayerState;
            }

            NetFrameSnapshot snapshot = new NetFrameSnapshot()
            {
                frameTick = NetworkManager.frameTick,
                timestamp = NetworkTime.time,
                playerCount = currentPlayerCount,
                playerStateList = _serverPlayerStateList
            };
            
            _snapshotList.Add(snapshot);
            NetworkServer.SendToAll(snapshot, Channels.DefaultUnreliable);
        }
        
        if(NetworkClient.active && _localPlayer != null)
        {
            var inputMap = _avatarSystem.GetInputMap();
            FrameInput lastInput = default(FrameInput);
            
            if(inputMap.TryGetValue(_localPlayer.controller.uuid, out lastInput))
            {
                SendPlayerInput sendInputMessage = new SendPlayerInput
                {
                    frameTick = NetworkManager.frameTick + kTicksAhead,
                    sentTime = NetworkTime.time,
                    input = lastInput
                };

                NetworkClient.Send(sendInputMessage, Channels.DefaultUnreliable);
            }
        }

        
        NetworkManager.frameTick++;
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
        IAvatarController playerController = _serverConnToPlayerMap[conn.connectionId];

        Dictionary<string, FrameInput> playerFrameInputMap;

        if(!_serverTickInputs.TryGetValue(msg.frameTick, out playerFrameInputMap))
        {
            playerFrameInputMap = new Dictionary<string, FrameInput>();
            _serverTickInputs[msg.frameTick] = playerFrameInputMap; //[conn.connectionId] = msg.input;
        }

        playerFrameInputMap[playerController.uuid] = msg.input;
        
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
            var playerMap = _networkManager.GetClientPlayerMap();
            NetPlayer netPlayer = playerMap[_networkManager.localPlayerSlot];
            setupLocalPlayer(netPlayer, NetworkClient.connection, controller);
        }

        return controller.view.gameObject;
    }

    private void setupLocalPlayer(NetPlayer nPlayer, NetworkConnection netConnection, IAvatarController playerController)
    {
        PlayerView pView = playerController.view as PlayerView;
        
        GameplayCamera cam = _getOrCreatePlayerCamera();
        if(cam != null)
        {
            cam.AddTarget(pView.cameraTargetGroup.transform);     
        }

        PlayerInput input = new PlayerInput(
                                    _networkManager.localPlayerSlot, 
                                    cam ? cam.gameCamera : null);
         
        playerController.input = input;
        playerController.isSimulating = true;
        
    
        _localPlayer = new LocalPlayerState
        {
            netPlayer = nPlayer,
            conn = netConnection,
            controller = playerController,
            pInput = input
        };
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
        _serverConnToPlayerMap[conn.connectionId] = controller;
        _serverPlayerControllerList.Add(controller);
        
        if(_networkManager.localPlayer != null && netPlayer.connectionId == _networkManager.localPlayer.connectionId)
        {
            setupLocalPlayer(netPlayer, conn, controller);
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

    private void onClientFrameSnapshot(NetworkConnection conn, NetFrameSnapshot msg)
    {
        if(_networkManager.isPureClient)
        {
            _snapshotList.Add(msg);
        }

        if(_networkManager.isHostClient)
        {
            return;
        }
        
        float lag = Mathf.Abs((float) (NetworkTime.time - msg.timestamp));
        
        // msg.
        for(int i = 0; i < msg.playerCount; ++i)
        {
            NetPlayerState newState = msg.playerStateList[i];
            IAvatarController c = _avatarSystem.GetPlayer(newState.netId);
            if(c != null && c.state != null)
            {
                c.state.previousPosition = c.state.position;
                c.state.position = newState.position + (newState.velocity * lag);
                c.state.aimPosition = newState.aimPosition;
                c.state.velocity = newState.velocity;
                
                c.view.Aim(c.state.aimPosition);
            }
        }
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

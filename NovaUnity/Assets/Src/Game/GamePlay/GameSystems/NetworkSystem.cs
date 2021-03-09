using System;
using System.Collections.Generic;
using GhostGen;
using Mirror;
using UnityEngine;

public class NetworkSystem : NotificationDispatcher, IGameSystem
{
    private const int MAX_PLAYERS = 16;
    

    private GameSystems _gameSystems;
    private GameState _gameState;
    
    private AvatarSystem _avatarSystem;
    
    private NetworkManager _networkManager;
    private UnitMap _unitMap;
    private Dictionary<Guid, UnitMap.Unit> _netPrefabMap;
    
    /* Server */
    private Dictionary<int, IAvatarController> _serverConnToPlayerMap;
    private List<IAvatarController> _serverPlayerControllerList;
    private List<NetPlayerState> _serverPlayerStateList;
    private uint _serverSendSequence = 0;
    private Dictionary<PlayerSlot, ServerPlayerInputBuffer> _serverPlayerInputBuffer;
    
    /* Client */
    private GameplayCamera               _camera;
    private LocalPlayerState             _localPlayer;
    private uint                         _clientSendSequence = 0;
    private uint                         _clientAckSequence  = 0;
    private List<PlayerInputTickPair>    _clientTempInputBuffer;
    
    
    public int priority { get; set; }
    

    public NetworkSystem(UnitMap unitMap)
    {
        _networkManager = Singleton.instance.networkManager;
        
        _unitMap = unitMap;
        _netPrefabMap = new Dictionary<Guid, UnitMap.Unit>();

        _serverConnToPlayerMap      = new Dictionary<int, IAvatarController>();
        _serverPlayerControllerList = new List<IAvatarController>();
        _serverPlayerStateList      = new List<NetPlayerState>(PlayerState.MAX_PLAYERS);
        _serverPlayerInputBuffer    = new Dictionary<PlayerSlot, ServerPlayerInputBuffer>(PlayerState.MAX_PLAYERS);
        
        _clientTempInputBuffer = new List<PlayerInputTickPair>(PlayerState.MAX_INPUTS);
        // _clientSnapshotBuffer  = new RingBuffer<NetFrameSnapshot>(5);

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
        // Application.targetFrameRate = 60; // TODO: Definitely temporary! 
        
        _gameSystems       = gameSystems;
        _gameState         = gameState;
        _avatarSystem      = gameSystems.Get<AvatarSystem>();
        _netSnapshotSystem = gameSystems.Get<NetSnapshotSystem<GameState.Snapshot>>();

        _netSnapshotSystem.onServerSendSnapshot = onServerSendSnapshot;
        
        _networkManager.onClientSpawnHandler   += onClientUnitSpawnHandler;
        _networkManager.onClientUnspawnHandler += onClientUnitUnspawnHandler;
        _networkManager.onClientDisconnect     += onClientLocalDisconnect;
        
        _networkManager.onServerMatchBegin        += onServerMatchBegin;
        _networkManager.onServerSendPlayerInput   += onServerSendPlayerInput;
        _networkManager.onServerConnect           += onServerConnect;
        _networkManager.onServerDisconnect        += onServerDisconnect;
        _networkManager.onServerMatchLoadComplete += onServerMatchLoadComplete;

        _gameSystems.onFixedStep += onFixedStep;
        
        
        NetworkManager.frameTick = 0;

        _clientAckSequence = 0;
        _clientSendSequence = 0;
        _serverSendSequence = 0;
    }

    public void CleanUp()
    {
        _networkManager.onClientSpawnHandler -= onClientUnitSpawnHandler;
        _networkManager.onClientUnspawnHandler -= onClientUnitUnspawnHandler;
        _networkManager.onClientDisconnect -= onClientLocalDisconnect;
        // _networkManager.onClientFrameSnapshot -= onClientFrameSnapshot;
        
        _networkManager.onServerMatchBegin -= onServerMatchBegin;
        _networkManager.onServerSendPlayerInput -= onServerSendPlayerInput;
        _networkManager.onServerConnect -= onServerConnect;
        _networkManager.onServerDisconnect -= onServerDisconnect;
        _networkManager.onServerMatchLoadComplete -= onServerMatchLoadComplete;
        
        _gameSystems.onFixedStep -= onFixedStep;
    }

    public bool GetInputForTick(PlayerSlot slot, out PlayerInputTickPair inputPair)
    {
        bool result = false;
        inputPair = default(PlayerInputTickPair);

        if(NetworkServer.active)
        {
            ServerPlayerInputBuffer inputBuffer;
            if(_serverPlayerInputBuffer.TryGetValue(slot, out inputBuffer) && !inputBuffer.isEmpty)
            {
                inputPair = inputBuffer.Pop();
                result = true;
            }
        }
        
        return result;
    }
    
    private void onFixedStep(float fixedDeltaTime)
    {
        if(NetworkServer.active)
        {
            serverFixedStep(fixedDeltaTime);
        }
        
        if(NetworkClient.active && _localPlayer != null)
        {
            clientFixedStep(fixedDeltaTime);
        }

        NetworkManager.frameTick++;
        _clientSendSequence++;
        _serverSendSequence++;
    }

    private void clientFixedStep(float fixedDeltaTime)
    {
        var pState = _localPlayer.state;
        // var inputBuffer = pState.nonAckInputBuffer;

        pState.sequence = _clientSendSequence;

        PlayerInputTickPair lastInput = pState.latestInput;

        _clientTempInputBuffer.Clear();
        int latestAckIndex = (int) (pState.ackSequence % PlayerState.MAX_INPUTS);
        _clientTempInputBuffer.Add(lastInput);
        // for(int i = 0; i < latestAckIndex; ++i)
        // {
        //     _clientTempInputBuffer.Add(pState.nonAckInputBuffer[i]);
        // }
        
        
        NetChannelHeader channelHeader = new NetChannelHeader
        {
            sequence    = _clientSendSequence,
            ackSequence = _clientAckSequence,
            frameTick   = NetworkManager.frameTick,
            sendTime    = TimeUtil.Now(),
        };
    
        SendPlayerInput sendInputMessage = new SendPlayerInput
        {
            header = channelHeader,
            inputList = _clientTempInputBuffer
        };

        NetworkClient.Send(sendInputMessage, Channels.DefaultUnreliable);
    }
    
    private void serverFixedStep(float fixedDeltaTime)
    {
        NetChannelHeader netHeader = new NetChannelHeader
        {
            sequence    = _serverSendSequence,
            ackSequence = _clientAckSequence,
            frameTick   =  NetworkManager.frameTick,
            sendTime    = TimeUtil.Now()
        };
        
        var playerMap = _networkManager.GetServerPlayerMap();
        foreach(var playerPair in playerMap)
        {
            int connId         = playerPair.Value.connectionId;
            var controller     = _serverConnToPlayerMap[playerPair.Value.connectionId];
            var state          = controller.state as PlayerState;
            var playerSnapshot = state.Snapshot();
            
            var msg = new PlayerStateUpdate
            {
                header   = netHeader,
                netId    = state.netId,
                snapshot = playerSnapshot
            };
            
            NetworkConnection conn = NetworkServer.connections[connId];
            conn.Send(msg, Channels.DefaultUnreliable);
        }
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
        Debug.Log("Session State: " + _networkManager.sessionState);
        
        if(_networkManager.sessionState == NetworkManager.SessionState.IN_GAME)
        {
            conn.Send(new StartMatchLoad(), Channels.DefaultReliable);
        }
    }

    private void onServerDisconnect(NetworkConnection conn)
    {
        IAvatarController controller = null;// _serverConnToPlayerMap[conn.connectionId];
        
        if(_serverConnToPlayerMap.TryGetValue(conn.connectionId, out controller))
        {
            NetPlayer netPlayer = _networkManager.GetServerPlayerFromConnId(conn.connectionId);
            
            _serverConnToPlayerMap?.Remove(conn.connectionId);
            _serverPlayerInputBuffer?.Remove(netPlayer.playerSlot);
            _serverPlayerControllerList?.Remove(controller);

            NetworkServer.UnSpawn(controller?.view?.gameObject);
            _avatarSystem.UnSpawn(controller?.view?.gameObject); // Unspawn locally
        }
    }
    
    private void onServerSendPlayerInput(NetworkConnection conn, SendPlayerInput msg)
    {
        IAvatarController playerController = _serverConnToPlayerMap[conn.connectionId];
        PlayerState pState = (PlayerState)playerController.state;

        ServerPlayerInputBuffer playerInputBuffer = _serverPlayerInputBuffer[pState.playerSlot];
        playerInputBuffer?.Push(msg);
    }
    
    private void onServerMatchBegin()
    {
        NetworkServer.SendToAll(new MatchBegin(), Channels.DefaultReliable);
        
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
        
        var playerMap = _networkManager.GetClientPlayerMap();
        NetPlayer netPlayer = playerMap[_networkManager.localPlayerSlot];
            
        setupPlayer(netPlayer, NetworkClient.connection, controller, msg.isOwner);
        controller.state.netId = msg.netId;
        
        return controller.view.gameObject;
    }

    private void setupPlayer(NetPlayer nPlayer, NetworkConnection netConnection, IAvatarController playerController, bool isOwner)
    {
        PlayerState pState = (PlayerState)playerController.state;
        pState.playerSlot = nPlayer.playerSlot;
        pState.netId      = playerController.view.netIdentity.netId;

        if(isOwner)
        {
            setupLocalPlayer(nPlayer, netConnection, playerController);
        }
    }
    
    private void setupLocalPlayer(NetPlayer nPlayer, NetworkConnection netConnection, IAvatarController playerController)
    {
        PlayerView     pView = playerController.view as PlayerView;
        GameplayCamera cam   = _getOrCreatePlayerCamera();
        cam?.AddTarget(pView.cameraTargetGroup.transform);     
        

        PlayerInput input = new PlayerInput(
                                    _networkManager.localPlayerSlot, 
                                    cam ? cam.gameCamera : null);
         
        playerController.input        = input;
        playerController.isSimulating = true;
        
    
        _localPlayer = new LocalPlayerState
        {
            netPlayer = nPlayer,
            conn = netConnection,
            controller = playerController,
            state = playerController.state as PlayerState,
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

        _serverPlayerInputBuffer[netPlayer.playerSlot] = new ServerPlayerInputBuffer(PlayerState.MAX_INPUTS);
        NetworkConnection conn = NetworkServer.connections[netPlayer.connectionId];
        
        NetworkServer.Spawn(spawnedGameObject, conn);
        NetworkServer.AddPlayerForConnection(conn, spawnedGameObject);
        
        _serverConnToPlayerMap[conn.connectionId] = controller;
        _serverPlayerControllerList.Add(controller);

        bool isOwner = _networkManager.localPlayer != null &&
                       netPlayer.connectionId == _networkManager.localPlayer.connectionId;
                       
        setupPlayer(netPlayer, conn, controller, isOwner);
    }
    
    private void onClientUnitUnspawnHandler(GameObject obj)
    {
        _avatarSystem.UnSpawn(obj);
    }

    private void onClientLocalDisconnect(NetworkConnection conn)
    {
        DispatchEvent(GamePlayEventType.NET_LOCAL_PLAYER_DISCONNECT, false, conn);
    }

    private void onClientPlayerStateUpdate(PlayerStateUpdate msg)
    {
        
    }
    
    private void clientProcessNetSnapshot(FrameSnapshot<GameState.Snapshot> snapshot)
    {
        //for(int i = 0; i < snapshot.playerStateList.Count; ++i)
        {
            // {
            //     int oldStateIndex = -1;
            //     for(int b = 0; b < state.nonAckInputBuffer.backIndex; ++b)
            //     {
            //         if(state.nonAckInputBuffer[b].tick == newState.ackTick)
            //         {
            //             oldStateIndex = b;
            //             Debug.Log("Old state found: " + b);
            //             break;
            //         }
            //     }
            //     
            //     // int oldStateIndex = (int)((newState.ackTick - 1) % PlayerState.MAX_INPUTS);
            //     if(oldStateIndex < 0)
            //     {
            //         state.previousPosition = state.position;//c.view.viewRoot.position;
            //         state.position = newState.position;
            //         state.aimPosition = newState.aimPosition;
            //     }
            //     else
            //     {
            //         PlayerStateSnapshot oldState = state.nonAckStateBuffer[oldStateIndex];
            //         Vector2 deltaPosition = oldState.position - newState.position;
            //         Vector2 deltaAimPosition = oldState.aimPosition - newState.aimPosition;
            //      
            //         if(deltaPosition.sqrMagnitude > 0.1f)// || deltaAimPosition.sqrMagnitude > 0.01f)
            //         {
            //             //Miss Predictited / catch up 
            //             state.SetFromSnapshot(oldState);
            //
            //             state.previousPosition = state.position;//c.view.viewRoot.position;
            //             state.position = newState.position;
            //             state.aimPosition = newState.aimPosition;
            //             
            //             c.view.transform.position = state.position;
            //             c.view.viewRoot.position     = state.previousPosition;
            //
            //             int currentStateIndex = (int)((state.nonAckInputBuffer.backIndex + 1) % PlayerState.MAX_INPUTS);
            //             int index = oldStateIndex;
            //             
            //             while(index != currentStateIndex)
            //             {
            //                 Debug.LogFormat("Index info: {0}, {1}, {2}", oldStateIndex, currentStateIndex, index);
            //         
            //                 PlayerInputTickPair simInput = state.nonAckInputBuffer[index];
            //                 c.FixedStep(Time.fixedDeltaTime, simInput.input);
            //                 state.nonAckStateBuffer[index] = state.Snapshot();
            //         
            //                 index = (int) ((index + 1) % PlayerState.MAX_INPUTS); 
            //             } 
            //         }
            //     }
            // }
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

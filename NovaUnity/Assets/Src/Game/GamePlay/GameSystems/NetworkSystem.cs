﻿using System;
using System.Collections.Generic;
using GhostGen;
using Mirror;
using UnityEngine;
public class NetworkSystem : NotificationDispatcher, IGameSystem
{
    private const int PLAYER_STATE_BUFFER = 10;
    
    private GameSystems    _gameSystems;
    private GameState      _gameState;
    
    private AvatarSystem   _avatarSystem;
    private NetworkManager _networkManager;
    private UnitMap        _unitMap;
    
    private Dictionary<uint, UnitMap.Unit> _netPrefabMap;
    
    /* Server */
    private uint                                            _serverSendSequence = 0;
    private Dictionary<int, IAvatarController>              _serverConnToPlayerMap;
    private List<IAvatarController>                         _serverPlayerControllerList;
    private List<NetPlayerState>                            _serverPlayerStateList;
    private Dictionary<PlayerSlot, ServerPlayerInputBuffer> _serverPlayerInputBuffer;
    
    /* Client */
    private GameplayCamera            _camera;
    private LocalPlayerState          _localPlayer;
    private uint                      _clientSendSequence = 0;
    private uint                      _clientAckSequence  = 0;
    private List<PlayerInputTickPair> _clientTempInputBuffer;
    private List<PlayerStateUpdate>   _clientPlayerJitterBuffer;
    
    private SortedList<uint, PlayerStateUpdate>             _clientJitterBuffer;
    
    
    public int priority { get; set; }
    

    public NetworkSystem(UnitMap unitMap)
    {
        _networkManager = Singleton.instance.networkManager;
        
        _unitMap = unitMap;

        _netPrefabMap               = new Dictionary<uint, UnitMap.Unit>();
        _serverConnToPlayerMap      = new Dictionary<int, IAvatarController>();
        _serverPlayerControllerList = new List<IAvatarController>();
        _serverPlayerStateList      = new List<NetPlayerState>(PlayerState.MAX_PLAYERS);
        _serverPlayerInputBuffer    = new Dictionary<PlayerSlot, ServerPlayerInputBuffer>(PlayerState.MAX_PLAYERS); 
        
        _clientTempInputBuffer    = new List<PlayerInputTickPair>(PlayerState.MAX_INPUTS);
        _clientJitterBuffer       = new SortedList<uint, PlayerStateUpdate>(32);
        _clientPlayerJitterBuffer = new List<PlayerStateUpdate>(32);
        
        for(int i = 0; i < _unitMap.unitList.Count; ++i)
        {
            UnitMap.Unit unit = _unitMap.unitList[i];
            Debug.Log("Unit: " + unit.id);
            uint guid = unit.view.netIdentity.assetId; //1406090550
            _netPrefabMap[guid] = unit;
        }
    }

    public void Start(bool hasAuthority, GameSystems gameSystems, GameState gameState)
    {
        // Application.targetFrameRate = 60; // TODO: Definitely temporary! 
        
        _gameSystems       = gameSystems;
        _gameState         = gameState;
        _avatarSystem      = gameSystems.Get<AvatarSystem>();
        
        _networkManager.onClientSpawnHandler   += onClientUnitSpawnHandler;
        _networkManager.onClientUnspawnHandler += onClientUnitUnspawnHandler;
        _networkManager.onClientDisconnect     += onClientLocalDisconnect;

        if (!_networkManager.isHostClient)
        {
            _networkManager.onClientPlayerStateUpdate += onClientPlayerStateUpdate;            
        }
        
        
        _networkManager.onServerMatchBegin        += onServerMatchBegin;
        _networkManager.onServerSendPlayerInput   += onServerSendPlayerInput;
        _networkManager.onServerConnect           += onServerConnect;
        _networkManager.onServerDisconnect        += onServerDisconnect;
        _networkManager.onServerMatchLoadComplete += onServerMatchLoadComplete;

        _gameSystems.onFixedStep += onFixedStep;
        
        _serverConnToPlayerMap.Clear();
        _serverPlayerControllerList.Clear();
        _serverPlayerStateList.Clear();
        _serverPlayerInputBuffer.Clear();
        
        _clientTempInputBuffer.Clear();
        _clientJitterBuffer.Clear();
        
        NetworkManager.frameTick = 0;

        _clientAckSequence  = 0;
        _clientSendSequence = 0;
        _serverSendSequence = 0;
    }

    public void CleanUp()
    {
        _networkManager.onClientSpawnHandler      -= onClientUnitSpawnHandler;
        _networkManager.onClientUnspawnHandler    -= onClientUnitUnspawnHandler;
        _networkManager.onClientDisconnect        -= onClientLocalDisconnect;
        _networkManager.onClientPlayerStateUpdate -= onClientPlayerStateUpdate;
        
        // _networkManager.onClientFrameSnapshot -= onClientFrameSnapshot;
        
        _networkManager.onServerMatchBegin        -= onServerMatchBegin;
        _networkManager.onServerSendPlayerInput   -= onServerSendPlayerInput;
        _networkManager.onServerConnect           -= onServerConnect;
        _networkManager.onServerDisconnect        -= onServerDisconnect;
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
        _serverSendSequence++;
        _clientSendSequence++;
    }

    private void clientFixedStep(float fixedDeltaTime)
    {
        // if (!_clientPlayerStateUpdateBuffer.IsEmpty)
        // {
        //     var msg = _clientPlayerStateUpdateBuffer.PopBack();
        //     clientPlayerReconsiliation(msg);            
        // }
        _clientPlayerJitterBuffer.Sort((a, b) =>
        {
            return a.header.ackSequence.CompareTo(b.header.ackSequence);
        });

        if (_clientPlayerJitterBuffer.Count > 0)
        {
            var msg = _clientPlayerJitterBuffer[0];
            clientPlayerReconsiliation(msg);
        }
        _clientPlayerJitterBuffer.Clear();
        
        clientSendInput();
    }
    
    private void clientSendInput()
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
            sequence    = lastInput.tick,//_clientSendSequence,
            ackSequence = _clientAckSequence,
            frameTick   = NetworkManager.frameTick,
            sendTime    = TimeUtil.Now(),
        };
    
        SendPlayerInput sendInputMessage = new SendPlayerInput
        {
            header    = channelHeader,
            inputList = _clientTempInputBuffer
        };

        NetworkClient.Send(sendInputMessage, Channels.Unreliable);
    }
    
    private void serverFixedStep(float fixedDeltaTime)
    {
        var playerMap = _networkManager.GetServerPlayerMap();
        foreach(var playerPair in playerMap)
        {
            int connId = playerPair.Value.connectionId;
            
            if (!_serverConnToPlayerMap.TryGetValue(connId, out var controller))
            {
                continue;
            }
            
            var state = controller.state as PlayerState;
            
            var playerSnapshot = new PlayerInputStateSnapshot
            {
                input       = state.latestInput.input,
                snapshot    = state.Snapshot()
            };
            
            NetChannelHeader netHeader = new NetChannelHeader
            {
                sequence    = _serverSendSequence,
                ackSequence = _serverSendSequence,//state.ackSequence,
                frameTick   = NetworkManager.frameTick,
                sendTime    = TimeUtil.Now()
            };
        
            var msg = new PlayerStateUpdate
            {
                header   = netHeader,
                netId    = state.netId,
                playerInputStateSnapshot = playerSnapshot
            };
            
            NetworkConnection conn = NetworkServer.connections[connId];
            conn.Send(msg, Channels.Unreliable);
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
            conn.Send(new StartMatchLoad(), Channels.Reliable);
        }
    }

    private void onServerDisconnect(NetworkConnection conn)
    {
        if(_serverConnToPlayerMap.TryGetValue(conn.connectionId, out IAvatarController controller))
        {
            NetPlayer netPlayer = _networkManager.GetServerPlayerFromConnId(conn.connectionId);
            
            _serverConnToPlayerMap?.Remove(conn.connectionId);
            _serverPlayerInputBuffer?.Remove(netPlayer.playerSlot);
            _serverPlayerControllerList?.Remove(controller);

            NetworkServer.UnSpawn(controller?.view?.gameObject);
            _avatarSystem?.UnSpawn(controller?.view?.gameObject); // Unspawn locally
        }
    }
    
    private void onServerSendPlayerInput(NetworkConnection conn, SendPlayerInput msg)
    {
        IAvatarController playerController = _serverConnToPlayerMap[conn.connectionId];
        PlayerState pState = (PlayerState)playerController.state;
        
        
        ServerPlayerInputBuffer playerInputBuffer = _serverPlayerInputBuffer[pState.playerSlot];
        playerInputBuffer?.Push(msg);

        var inputGen = playerController.input as ServerPlayerInputGenerator;
        inputGen?.AddInput(msg);
    }
    
    private void onServerMatchBegin()
    {
        NetworkServer.SendToAll(new MatchBegin(), Channels.Reliable);
        
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
        

        PlayerInputGenerator inputGenerator = new PlayerInputGenerator(
                                    _networkManager.localPlayerSlot, 
                                    cam ? cam.gameCamera : null);
         
        playerController.input        = inputGenerator;
        playerController.isSimulating = true;
        
    
        _localPlayer = new LocalPlayerState
        {
            netPlayer   = nPlayer,
            conn        = netConnection,
            controller  = playerController,
            state       = playerController.state as PlayerState,
            pInput      = inputGenerator
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

        UnitMap.Unit      playerUnit        = _unitMap.GetUnit("player");
        IAvatarController controller        = _avatarSystem.Spawn<IAvatarController>(playerUnit.id, point.transform.position);
        GameObject        spawnedGameObject = controller.view.gameObject;

        _serverPlayerInputBuffer[netPlayer.playerSlot] = new ServerPlayerInputBuffer(PlayerState.MAX_INPUTS);
        NetworkConnectionToClient conn = NetworkServer.connections[netPlayer.connectionId];
        
        NetworkServer.Spawn(spawnedGameObject, conn);
        NetworkServer.AddPlayerForConnection(conn, spawnedGameObject);
        
        _serverConnToPlayerMap[conn.connectionId] = controller;
        _serverPlayerControllerList.Add(controller);

        bool isOwner = _networkManager.localPlayer != null &&
                       netPlayer.connectionId == _networkManager.localPlayer.connectionId;
                       
        setupPlayer(netPlayer, conn, controller, isOwner);

        if (!isOwner)
        {
            controller.input = new ServerPlayerInputGenerator();
        }
    }
    
    private void onClientUnitUnspawnHandler(GameObject obj)
    {
        _avatarSystem.UnSpawn(obj);
    }

    private void onClientLocalDisconnect()
    {
        DispatchEvent(GamePlayEventType.NET_LOCAL_PLAYER_DISCONNECT);
    }

    private void onClientPlayerStateUpdate(PlayerStateUpdate msg)
    {
        // Not ready yet
        if (_localPlayer == null)
        {
            return;
        }
        
        NetChannelHeader header = msg.header;
        PlayerState state = _localPlayer.state;

        //Don't bother, already done
        if (_clientAckSequence >= header.ackSequence)
        {
            return;
        }
        
        _clientPlayerJitterBuffer?.Add(msg);
    }

    private void clientPlayerReconsiliation(PlayerStateUpdate msg)
    {
        NetChannelHeader  header     = msg.header;
        IAvatarController controller = _localPlayer.controller;
        PlayerState       state      = _localPlayer.state;
        

        Debug.LogFormat("state ack:{0}, header ack:{1}", state.ackSequence, header.ackSequence);

        var  snapshotTuple = msg.playerInputStateSnapshot;
        uint frameIndex    = msg.header.ackSequence % PlayerState.MAX_INPUTS;
        var  newState      = snapshotTuple.snapshot;
        var  oldState      = state.nonAckSnapshotBuffer[frameIndex];
        
        Vector2 positionDelta = oldState.snapshot.position - newState.position;
        if (positionDelta.magnitude > 0.01f)
        {
            state.SetFromSnapshot(newState);
        
            uint start = frameIndex;
            uint end   = (NetworkManager.frameTick) % PlayerState.MAX_INPUTS;   
            uint index = start;
            
            while (index != end)
            {
                // Debug.Log($"Start:{start} End:{end}, Index:{index}");
                
                var frame = state.nonAckSnapshotBuffer[index];
                
                controller.FixedStep(Time.fixedDeltaTime, frame.input);
                
                state.nonAckSnapshotBuffer[index].snapshot = state.Snapshot();
                
                index = (index + 1) % PlayerState.MAX_INPUTS;
            }
        }
        
        // state.SetFromSnapshot(snapshotTuple.snapshot);
        //
        // uint tickIter = header.ackSequence;
        // while (tickIter <= NetworkManager.frameTick)
        // {
        //     bool hasInput = false;
        //     PlayerInputTickPair tickPair = default(PlayerInputTickPair);
        //     
        //     while (tickPair.tick < tickIter && !state.nonAckInputBuffer.IsEmpty)
        //     {
        //         tickPair = state.nonAckInputBuffer.PopBack();
        //         hasInput = true;
        //     }
        //
        //     if (hasInput)
        //     {
        //         controller.FixedStep(Time.fixedDeltaTime, tickPair.input);
        //     }
        //     tickIter++;
        // }

        state.ackSequence  = header.ackSequence;
        _clientAckSequence = header.ackSequence;
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

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
    private NetPlayerState[] _serverPlayerStateList;
    private uint _serverSendSequence = 0;
    private Dictionary<PlayerSlot, ServerPlayerInputBuffer> _serverPlayerInputBuffer;
    
    /* Client */
    private GameplayCamera _camera;
    private LocalPlayerState _localPlayer;
    private uint _clientSendSequence = 0;
    private List<PlayerInputTickPair> _clientTempInputBuffer;
    // private Queue<NetFrameSnapshot> _clientSnapshotQueue;
    
    
    public int priority { get; set; }
    

    public NetworkSystem(UnitMap unitMap)
    {
        _networkManager = Singleton.instance.networkManager;
        
        _unitMap = unitMap;
        _netPrefabMap = new Dictionary<Guid, UnitMap.Unit>();
        
        _serverConnToPlayerMap = new Dictionary<int, IAvatarController>();
        _serverPlayerControllerList = new List<IAvatarController>();
        _serverPlayerStateList = new NetPlayerState[MAX_PLAYERS];
        _serverPlayerInputBuffer = new Dictionary<PlayerSlot, ServerPlayerInputBuffer>(MAX_PLAYERS);
        _clientTempInputBuffer = new List<PlayerInputTickPair>(PlayerState.MAX_INPUTS);
        // _clientSnapshotQueue = new Queue<NetFrameSnapshot>(3);

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
        Application.targetFrameRate = 60; // TODO: Definitely temporary! 
        
        _gameSystems = gameSystems;
        _avatarSystem = gameSystems.Get<AvatarSystem>();
        
        _networkManager.onClientSpawnHandler += onClientUnitSpawnHandler;
        _networkManager.onClientUnspawnHandler += onClientUnitUnspawnHandler;
        _networkManager.onClientLocalDisconnect += onClientLocalDisconnect;
        _networkManager.onClientFrameSnapshot += onClientFrameSnapshot;
        
        _networkManager.onServerMatchBegin += onServerMatchBegin;
        _networkManager.onServerSendPlayerInput += onServerSendPlayerInput;
        _networkManager.onServerConnect += onServerConnect;
        _networkManager.onServerDisconnect += onServerDisconnect;
        _networkManager.onServerMatchLoadComplete += onServerMatchLoadComplete;

        _gameSystems.onFixedStep += onFixedStep;
        
        
        NetworkManager.frameTick = 0;
        
        _clientSendSequence = 0;
        _serverSendSequence = 0;
    }

    public void CleanUp()
    {
        _networkManager.onClientSpawnHandler -= onClientUnitSpawnHandler;
        _networkManager.onClientUnspawnHandler -= onClientUnitUnspawnHandler;
        _networkManager.onClientLocalDisconnect -= onClientLocalDisconnect;
        _networkManager.onClientFrameSnapshot -= onClientFrameSnapshot;
        
        _networkManager.onServerMatchBegin -= onServerMatchBegin;
        _networkManager.onServerSendPlayerInput -= onServerSendPlayerInput;
        _networkManager.onServerConnect -= onServerConnect;
        _networkManager.onServerDisconnect -= onServerDisconnect;
        _networkManager.onServerMatchLoadComplete -= onServerMatchLoadComplete;
        
        _gameSystems.onFixedStep -= onFixedStep;
    }

    public bool GetInputForTick(PlayerSlot slot, out FrameInput input)
    {
        bool result = false;
        input = default(FrameInput);

        if(NetworkServer.active)
        {
            ServerPlayerInputBuffer inputBuffer = _serverPlayerInputBuffer[slot];
            if(!inputBuffer.isEmpty)
            {
                PlayerInputTickPair inputPair  = inputBuffer.Pop();
                input = inputPair.input;
                
                result = true;
            }
        }
        
        return result;
    }
    
    private void onFixedStep(float fixedDeltaTime)
    {
        if(NetworkServer.active)
        {
            int currentPlayerCount = _serverPlayerControllerList.Count;
            for(int i = 0; i < currentPlayerCount; ++i)
            {
                IAvatarController pController = _serverPlayerControllerList[i];
                PlayerState state = pController.state as PlayerState;

                if(state != null)
                {
                    _serverPlayerStateList[i] = state.nonAckStateBuffer.Front();
                }
            }

            NetChannelHeader channelHeader = new NetChannelHeader
            {
                sequence = _serverSendSequence
            };
            
            NetFrameSnapshot snapshot = new NetFrameSnapshot
            {
                frameTick = NetworkManager.frameTick,
                sendTime = NetworkTime.time,
                header = channelHeader,
                playerCount = currentPlayerCount,
                playerStateList = _serverPlayerStateList
            };
    
            
            NetworkServer.SendToAll(snapshot, Channels.DefaultUnreliable);
            _serverSendSequence++;
        }
        
        if(NetworkClient.active && _localPlayer != null)
        {
            var inputMap = _avatarSystem.GetInputMap();
            var inputBuffer = _localPlayer.state.nonAckInputBuffer;

            PlayerInputTickPair lastInput = !inputBuffer.IsEmpty ? inputBuffer.Back() : default(PlayerInputTickPair);

            _clientTempInputBuffer.Clear();
            int latestAckIndex = (int) (_localPlayer.state.lastAck % PlayerState.MAX_INPUTS);
            for(int i = 0; i < latestAckIndex; ++i)
            {
                _clientTempInputBuffer.Add(_localPlayer.state.nonAckInputBuffer[i]);
            }
            
            
            NetChannelHeader channelHeader = new NetChannelHeader
            {
                sequence = _clientSendSequence
            };
        
            SendPlayerInput sendInputMessage = new SendPlayerInput
            {
                header = channelHeader,
                frameTick = NetworkManager.frameTick,// + kTicksAhead,
                sentTime = TimeUtil.timestamp(),
                inputList = _clientTempInputBuffer
                
            };

            NetworkClient.Send(sendInputMessage, Channels.DefaultUnreliable);
            _clientSendSequence++;
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
        IAvatarController playerController = _serverConnToPlayerMap[conn.connectionId];
        PlayerState pState = playerController.state as PlayerState;

      
        ServerPlayerInputBuffer playerInputBuffer = _serverPlayerInputBuffer[pState.playerSlot];
        playerInputBuffer.Push(msg);
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
        
        var playerMap = _networkManager.GetClientPlayerMap();
        NetPlayer netPlayer = playerMap[_networkManager.localPlayerSlot];
            
        setupPlayer(netPlayer, NetworkClient.connection, controller, msg.isOwner);

        return controller.view.gameObject;
    }

    private void setupPlayer(NetPlayer nPlayer, NetworkConnection netConnection, IAvatarController playerController, bool isOwner)
    {
        PlayerState pState = playerController.state as PlayerState;
        pState.playerSlot = nPlayer.playerSlot;

        if(isOwner)
        {
            setupLocalPlayer(nPlayer, netConnection, playerController);
        }
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

    private void onClientFrameSnapshot(NetworkConnection conn, NetFrameSnapshot msg)
    {
        if(_networkManager.isHostClient)
        {
            return;
        }

        // _localPlayer.state.lastAck = msg.frameTick;
        
        float lag = Mathf.Abs((float) (NetworkTime.time - msg.sendTime));
        for(int i = 0; i < msg.playerCount; ++i)
        {
            NetPlayerState newState = msg.playerStateList[i];
            IAvatarController c = _avatarSystem.GetPlayerByNetId(newState.netId);
            
            if(c == null || c.state == null)
            {
                continue;    
            }
            
            if(!c.isSimulating)
            {
                c.state.previousPosition = c.state.position;
                c.state.position = newState.position + (newState.velocity * lag);
                c.state.aimPosition = newState.aimPosition;
                c.state.velocity = newState.velocity;
                
                c.view.Aim(c.state.aimPosition);
            }
            else
            {
                PlayerState state = c.state as PlayerState;
                state.lastAck = msg.frameTick;

                int oldStateIndex = (int)(msg.frameTick % state.nonAckStateBuffer.Capacity);
                
                NetPlayerState oldState = state.nonAckStateBuffer[oldStateIndex];
                Vector2 deltaPosition = oldState.position - newState.position;
                Vector2 deltaAimPosition = oldState.aimPosition - newState.aimPosition;

                if(deltaPosition.sqrMagnitude > 0.01f || deltaAimPosition.sqrMagnitude > 0.01f)
                {
                    //Miss Predictited / catch up
                    state.position = newState.position;
                    state.previousPosition = newState.position;
                    state.aimPosition = newState.aimPosition;

                    for(int j = oldStateIndex + 1; j < state.nonAckInputBuffer.Size; ++j)
                    {
                        PlayerInputTickPair simInput = state.nonAckInputBuffer[j];
                        c.FixedStep(Time.fixedDeltaTime, simInput.input);
                        
                        NetPlayerState newNetPlayerState = NetPlayerState.Create(
                            state, 
                            simInput.tick, 
                            c.view.netIdentity.netId);

                        state.nonAckStateBuffer[j] = newNetPlayerState;
                    }
                }
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

﻿using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

public class NetworkManager : Mirror.NetworkManager
{
    public const string kGameVersion = "6.6.6";

    // public const int kMaxPlayers = 4;
    public const string kSingleplayerRoom = "Singleplayer";

    public event Action<string> onError;
    public event Action onServerStarted;
    public event Action<NetworkConnection> onServerConnect;
    public event Action<NetworkConnection> onServerDisconnect;
    public event Action<NetworkConnection, int> onServerError;
    public event Action<NetworkConnection, RequestReadyUp> onServerRequestReadyUp;
    
    public event Action onClientStarted;
    public event Action<NetworkConnection> onClientConnect;
    public event Action<NetworkConnection> onClientDisconnect;
    public event Action<NetworkConnection> onLocalClientDisconnect;
    public event Action<NetworkConnection, int> onClientError;
    public event Action<NetworkConnection, SyncLobbyPlayers> onClientSyncLobbyPlayers;
    public event Action<NetworkConnection, ConfirmReadyUp> onClientConfirmReadyUp;
    public event Action<NetworkConnection, StartMatchLoad> onClientStartMatchLoad;
    public event Action<NetworkConnection, MatchBegin> onClientMatchBegin;

    public override void OnDestroy()
    {
        onError = null;
        onServerStarted = null;
        onServerConnect = null;
        onServerDisconnect = null;
        onServerError = null;
        onServerRequestReadyUp = null;
        
        onClientStarted = null;
        onClientConnect = null;
        onClientDisconnect = null;
        onLocalClientDisconnect = null;
        onClientError = null;
        onClientSyncLobbyPlayers = null;
        onClientConfirmReadyUp = null;
        onClientStartMatchLoad = null;
        onClientMatchBegin = null;

        base.OnDestroy();
    }

    private Dictionary<int, NetPlayer> _serverNetPlayerMap = new Dictionary<int, NetPlayer>();
    
    // Can't use ConnectionID's for the keys on the client because the connection id's won't match between server and client
    private Dictionary<PlayerSlot, NetPlayer> _clientNetPlayerMap = new Dictionary<PlayerSlot, NetPlayer>();

    private List<PlayerSlot> _serverAvailablePlayerSlots = new List<PlayerSlot>();


    public PlayerSlot localPlayerSlot { get; private set; }
    
    public Dictionary<PlayerSlot, NetPlayer> GetClientPlayerMap()
    {
        return _clientNetPlayerMap;
    }

    public override void Start()
    {
        base.Start();

        UnitMap unitMap = Singleton.instance.gameplayResources.unitMap;
        UnitMap.Unit unit = unitMap.GetUnit("player");
        spawnPrefabs.Add(unit.view.gameObject);
    }

    public override void OnStartServer()
    {
        Debug.Log("OnStartServer");
        
        _serverNetPlayerMap.Clear();
        setupPlayerSlotGenerator();
        
        NetworkServer.RegisterHandler<RequestMatchStart>(OnServerRequestMatchStart, false);
        NetworkServer.RegisterHandler<RequestReadyUp>(OnServerRequestReadyUp, false);
        NetworkServer.RegisterHandler<PlayerMatchLoadComplete>(OnServerPlayerMatchLoadComplete, false);
        
        onServerStarted?.Invoke();
    }

    public override void OnStartClient()
    {
        Debug.Log("OnStartClient");

        _clientNetPlayerMap.Clear();
        
        NetworkClient.RegisterHandler<SyncLobbyPlayers>(OnClientSyncLobbyPlayers, false);
        NetworkClient.RegisterHandler<ConfirmReadyUp>(OnClientConfirmReadyUp, false);
        NetworkClient.RegisterHandler<AssignPlayerSlot>(OnClientAssignPlayerSlot, false);
        NetworkClient.RegisterHandler<StartMatchLoad>(OnClientStartMatchLoad, false);
        NetworkClient.RegisterHandler<MatchBegin>(OnClientMatchBegin, false);
        
        onClientStarted?.Invoke();
    }

    public override void OnError(string reason)
    {
        base.OnError(reason);
        onError?.Invoke(reason);
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        Debug.Log("OnServerConnect");

        PlayerSlot pSlot = PlayerSlot.NONE;
        bool hasPlayerSlot = retrieveAvailablePlayerSlot(ref pSlot);

        if(hasPlayerSlot)
        {
            _serverNetPlayerMap[conn.connectionId] = new NetPlayer(
                conn.connectionId,
                pSlot,
                pSlot.ToString(),
                false);

            // Assign our connected player a number
            AssignPlayerSlot assignMessage = new AssignPlayerSlot(pSlot);
            conn.Send(assignMessage, Channels.DefaultReliable);
            
            // Sync player states with all clients
            SyncLobbyPlayers syncPlayersMessage = new SyncLobbyPlayers();
            syncPlayersMessage.playerList = _serverNetPlayerMap.Values.ToArray();
            NetworkServer.SendToAll(syncPlayersMessage, Channels.DefaultReliable);
            
            onServerConnect?.Invoke(conn);
        }
        else
        {
            NetworkServer.dontListen = true;
            // No more free player slots! Sorry bud :(
            conn.Disconnect();
        }
        

    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        Debug.Log("OnServerDisconnect");

        if(conn != null && _serverNetPlayerMap.ContainsKey(conn.connectionId))
        {
            NetPlayer player = _serverNetPlayerMap[conn.connectionId];
            PlayerSlot playerSlot = player.playerSlot;

            returnPlayerNumber(playerSlot);
            _serverNetPlayerMap.Remove(conn.connectionId);

            SyncLobbyPlayers syncPlayersMessage = new SyncLobbyPlayers();
            syncPlayersMessage.playerList = _serverNetPlayerMap.Values.ToArray();
            NetworkServer.SendToAll(syncPlayersMessage, Channels.DefaultReliable);
            
            NetworkServer.dontListen = false;
        }

        onServerDisconnect?.Invoke(conn);
    }

    public override void OnServerError(NetworkConnection conn, int errorCode)
    {
        Debug.LogError("OnServerErrorL " + errorCode);
        onServerError?.Invoke(conn, errorCode);
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        Debug.Log("OnClientConnect");
        // _netPlayerMap[conn.connectionId] = new NetPlayer(conn, conn.connectionId, "P" + conn.connectionId);
        onClientConnect?.Invoke(conn);
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        Debug.Log("OnClientDisconnect");

        onClientDisconnect?.Invoke(conn);

        NetworkConnection localConnection = NetworkClient.connection;
        if(localConnection != null && localConnection.connectionId == conn.connectionId)
        {
            onLocalClientDisconnect?.Invoke(conn);
        }

        // _netPlayerMap.Remove(conn.connectionId);
    }

    public override void OnClientError(NetworkConnection conn, int errorCode)
    {
        Debug.LogError("OnClientError: " + errorCode);
        onClientError?.Invoke(conn, errorCode);
    }

    private void OnClientSyncLobbyPlayers(NetworkConnection conn, SyncLobbyPlayers msg)
    {
        _clientNetPlayerMap = msg.GetPlayerMap();
        
        onClientSyncLobbyPlayers?.Invoke(conn, msg);
    }

    private void OnClientMatchBegin(NetworkConnection conn, MatchBegin msg)
    {
        Debug.Log("Client Begin Match");
        onClientMatchBegin?.Invoke(conn, msg);
    }

    private void OnClientStartMatchLoad(NetworkConnection conn, StartMatchLoad msg)
    {
        onClientStartMatchLoad?.Invoke(conn, msg);
    }
    
    private void OnClientAssignPlayerSlot(NetworkConnection conn, AssignPlayerSlot msg)
    {
        localPlayerSlot = msg.playerSlot;
    }
    
    private void OnClientConfirmReadyUp(NetworkConnection conn, ConfirmReadyUp msg)
    {
        Debug.LogFormat("OnClientConfirmReadyUp for Player:{0} : {1}", msg.playerSlot, msg.isReady);

        PlayerSlot playerSlot = msg.playerSlot;
        
        NetPlayer player = _clientNetPlayerMap[playerSlot];
        player.isReadyUp = msg.isReady;
        _clientNetPlayerMap[playerSlot] = player;
        
        onClientConfirmReadyUp?.Invoke(conn, msg);
    }

    private void OnServerRequestMatchStart(NetworkConnection conn, RequestMatchStart msg)
    {
        if(conn.connectionId == NetworkServer.localConnection.connectionId)
        {
            StartMatchLoad startMatchMessage = new StartMatchLoad();
            NetworkServer.SendToAll(startMatchMessage, Channels.DefaultReliable);
        }
    }

    private void OnServerPlayerMatchLoadComplete(NetworkConnection conn, PlayerMatchLoadComplete msg)
    {
        NetPlayer player = _serverNetPlayerMap[conn.connectionId];
        player.isMatchReady = true;
        _serverNetPlayerMap[conn.connectionId] = player;
        
        Debug.Log("OnServerPlayerMatchLoadComplete: " + player.playerSlot.ToString());

        bool allPlayersLoaded = hasAllPlayersLoaded();
        if(allPlayersLoaded)
        {
            Debug.Log("Server Match Start");
            NetworkServer.SendToAll(new MatchBegin(), Channels.DefaultReliable);
        }
    }
    
    private void OnServerRequestReadyUp(NetworkConnection conn, RequestReadyUp msg)
    {
        NetPlayer player = _serverNetPlayerMap[conn.connectionId];
        
        Debug.LogFormat("OnServerRequestReadyUp for Player:{0} : {1}", player.playerSlot, msg.isReady);

        if(player.isReadyUp != msg.isReady)
        {
            player.isReadyUp = msg.isReady;
            _serverNetPlayerMap[conn.connectionId] = player;

            bool allPlayersReady = canStartGame();
            ConfirmReadyUp readyMessage = new ConfirmReadyUp(
                                                player.playerSlot, 
                                                player.isReadyUp, 
                                                allPlayersReady);
                                                
            NetworkServer.SendToAll(readyMessage, Channels.DefaultReliable);

            onServerRequestReadyUp?.Invoke(conn, msg);
        }
    }

    public void Disconnect()
    {
        if(NetworkServer.active)
        {
            StopServer();
        }

        if(NetworkClient.active)
        {
            StopClient();
        }
    }

    public bool isConnected
    {
        get { return NetworkClient.active || NetworkServer.active; }
    }

    private bool retrieveAvailablePlayerSlot(ref PlayerSlot num)
    {
        int availablePlayerCount = _serverAvailablePlayerSlots.Count;
        if(availablePlayerCount <= 0)
        {
            return false;
        }

        int index = availablePlayerCount - 1;
        num = _serverAvailablePlayerSlots[index];
        _serverAvailablePlayerSlots.RemoveAt(index);

        return true;
    }

    private bool canStartGame()
    {
        foreach(var player in _serverNetPlayerMap)
        {
            if(!player.Value.isReadyUp)
            {
                return false;
            }
        }

        return true;
    }
    
    private bool hasAllPlayersLoaded()
    {
        foreach(var player in _serverNetPlayerMap)
        {
            if(!player.Value.isMatchReady)
            {
                return false;
            }
        }

        return true;
    }
    
    private void returnPlayerNumber(PlayerSlot num)
    {
        _serverAvailablePlayerSlots.Add(num);
        _serverAvailablePlayerSlots.Sort((a, b) =>
        {
            return b.CompareTo(a);
        });
    }

    private void setupPlayerSlotGenerator()
    {
        foreach(PlayerSlot pNum in Enum.GetValues(typeof(PlayerSlot)))
        {
            if(pNum == PlayerSlot.NONE) { continue; }
            if(pNum == PlayerSlot.MAX_PLAYERS) { continue; }

            _serverAvailablePlayerSlots.Add(pNum);
        }

        _serverAvailablePlayerSlots.Sort((a, b) =>
        {
            return b.CompareTo(a);
        });
    }
    
    public NetworkConnection localPlayer
    {
        get { return NetworkClient.connection != null ? NetworkClient.connection : null; }
    }

    private void safeCall(Action callback)
    {
        if(callback != null)
        {
            callback();
        }
    }
}

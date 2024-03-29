﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using kcp2k;
using Mirror;
using UnityEngine;
using UnityEngine.Networking;


public class NetworkManager : Mirror.NetworkManager
{
    public const string kGameVersion = "6.6.6";

    // public const int kMaxPlayers = 4;
    public const string kSingleplayerRoom = "Singleplayer";

    private const string kMasterListSecretId = "NodeListServerDefaultKey";//"23431909-e58c-40e3-94d2-8d90b4f9e11d";
    private const string kAppIdKey = "serverKey";
    private const string kServerUuIdKey = "serverUuid";
    private const string kServerName = "serverName";  
    private const string kServerIp = "serverIp";
    private const string kServerPort = "serverPort";    
    private const string kServerPlayerCount = "serverPlayers";
    private const string kServerPlayerCapacity = "serverCapacity";

    private Action _onSingleplayerCallback;


    public string masterServerLocation = "http://xaer0-vpn.duckdns.org"; //"http://novamaster.net";
    public int    masterServerPort     = 11667;

    public GameObject syncStatePrefab;
    
    
    // public event Action<string> onError;
    public event Action onServerStarted;
    public event Action onServerStopped;
    public event Action onServerMatchBegin;
    public event Action<NetworkConnection> onServerConnect;
    public event Action<NetworkConnection> onServerDisconnect;
    // public event Action<NetworkConnection, int> onServerError;
    public event Action<NetworkConnection, ConfirmReadyUp> onServerConfirmReadyUp;
    public event Action<NetworkConnection, SendPlayerInput> onServerSendPlayerInput;
    public event Action<NetworkConnection> onServerMatchLoadComplete;
    
    public event Action onClientStarted;
    public event Action onClientStopped;
    
    public event Action onClientConnect;
    public event Action onClientDisconnect;
    public event Action onClientLocalDisconnect;
    // public event Action<NetworkConnection, int> onClientError;
    public event Action<SyncLobbyPlayers> onClientSyncLobbyPlayers;
    public event Action<ConfirmReadyUp> onClientConfirmReadyUp;
    public event Action<StartMatchLoad> onClientStartMatchLoad;
    public event Action<MatchBegin> onClientMatchBegin;
    public event Action<NetFrameSnapshot> onClientFrameSnapshot;
    public event Action<CurrentSessionUpdate> onClientCurrentSession;
    public event Action<PlayerStateUpdate> onClientPlayerStateUpdate;
    public event SpawnHandlerDelegate onClientSpawnHandler;
    public event UnSpawnDelegate onClientUnspawnHandler;

    public enum SessionState
    {
        NONE = 0,
        IN_LOBBY,
        IN_GAME
    }
    
    public ServerListEntry serverEntry { get; set; }
    
    public SessionState sessionState { get; private set; }

    public SyncStore syncStore { get; private set; }
    
    public static uint frameTick { get; set; }

    public override void Start()
    {
        base.Start();

        // NetworkDiagnostics.OutMessageEvent += OnOutMessageEvent;
        // NetworkDiagnostics.InMessageEvent  += OnInMessageEvent;
    }
    
    public override void OnDestroy()
    {
        onServerStarted           = null;
        onServerStopped           = null;
        onServerConnect           = null;
        onServerDisconnect        = null;
        onServerConfirmReadyUp    = null;
        onServerMatchBegin        = null;
        onServerSendPlayerInput   = null;
        onServerMatchLoadComplete = null;
        
        
        onClientStarted           = null;
        onClientStopped           = null;
        onClientConnect           = null;
        onClientDisconnect        = null;
        onClientLocalDisconnect   = null;
        onClientSyncLobbyPlayers  = null;
        onClientConfirmReadyUp    = null;
        onClientStartMatchLoad    = null;
        onClientMatchBegin        = null;
        onClientFrameSnapshot     = null;
        onClientCurrentSession    = null;
        onClientPlayerStateUpdate = null;

        base.OnDestroy();
    }

    private Dictionary<int, NetPlayer> _serverNetPlayerMap = new Dictionary<int, NetPlayer>();
    
    // Can't use ConnectionID's for the keys on the client because the connection id's won't match between server and client
    private Dictionary<PlayerSlot, NetPlayer> _clientNetPlayerMap = new Dictionary<PlayerSlot, NetPlayer>();

    private List<PlayerSlot> _serverAvailablePlayerSlots = new List<PlayerSlot>();


    public PlayerSlot localPlayerSlot { get; private set; }

    public bool isPureServer
    {
        get
        {
            return NetworkServer.active && !NetworkClient.active;
        }
    }

    public bool isPureClient
    {
        get
        {
            return !NetworkServer.active && NetworkClient.active;
        }
    }

    public bool isHostClient // Server & client 
    {
        get
        {
            return NetworkServer.active && NetworkClient.active;
        }
    }

    public Dictionary<PlayerSlot, NetPlayer> GetClientPlayerMap()
    {
        return _clientNetPlayerMap;
    }
    
    public Dictionary<PlayerSlot, NetPlayer> GetServerPlayerMap()
    {
        var playerMap = new Dictionary<PlayerSlot, NetPlayer>();
        foreach(var pair in _serverNetPlayerMap)
        {
            NetPlayer player = pair.Value;
            playerMap[player.playerSlot] = player;
        }

        return playerMap;
    }

    public NetPlayer GetServerPlayerFromConnId(int connId)
    {
        return _serverNetPlayerMap[connId];
    }

    public void StartSingleplayer(Action onSingleplayerCreated)
    {
        _onSingleplayerCallback = onSingleplayerCreated;
        
        onClientCurrentSession += onSingleplayerCurrentSession;
        // onServerMatchBegin += onSingleplayerMatchBegin;
        
	    NetworkServer.dontListen = true;
	    StartHost();
    }

    private void onSingleplayerCurrentSession(CurrentSessionUpdate msg)
    {   
        onClientCurrentSession -= onSingleplayerCurrentSession;
        onServerMatchBegin += onSingleplayerMatchBegin;
        
	    // ClientScene.Ready(NetworkClient.connection);
        NetworkClient.Send(new PlayerMatchLoadComplete(), Channels.Reliable);
    }
    
    private void onSingleplayerMatchBegin()
    {
        onServerMatchBegin -= onSingleplayerMatchBegin;
        
        _onSingleplayerCallback?.Invoke();
        _onSingleplayerCallback = null;
    }
    
    private void registerNetworkPrefabs()
    {
        UnitMap unitMap = Singleton.instance.gameplayResources.unitMap;
        for(int i = 0; i < unitMap.unitList.Count; ++i)
        {
            UnitMap.Unit unit = unitMap.unitList[i];
            Debug.Log("Unit: " + unit.id);
            
            NetworkClient.RegisterPrefab(
                unit.view.gameObject, 
                OnClientSpawnHandler,
                OnClientUnspawnHandler);
        }
        
        NetworkClient.RegisterPrefab(
                syncStatePrefab, 
                onClientSyncStoreCreated, 
                onClientSyncStoreDestory);
    }

    private void unregisterClientPrefabs()
    {
        UnitMap unitMap = Singleton.instance.gameplayResources.unitMap;
        for(int i = 0; i < unitMap.unitList.Count; ++i)
        {
            UnitMap.Unit unit = unitMap.unitList[i];
            Debug.Log("Unit: " + unit.id);
            
            NetworkClient.UnregisterPrefab(
                unit.view.gameObject);
        }
        
        NetworkClient.UnregisterPrefab(
                syncStatePrefab);
    }

    private GameObject onClientSyncStoreCreated(SpawnMessage msg)
    {
        GameObject syncStoreInstance = GameObject.Instantiate(syncStatePrefab, transform);
        syncStore = syncStoreInstance.GetComponent<SyncStore>();
        return syncStoreInstance;
    }
    
    private void onClientSyncStoreDestory(GameObject obj)
    {
        GameObject.Destroy(obj);
    }
    
    private string getMasterServerCommand(string command)
    {
        return string.Format("{0}:{1}/{2}", masterServerLocation, masterServerPort, command); 
    }


    public void FetchExternalIpAddress(Action<bool, string> onComplete)
    {
        StartCoroutine(enumGetExternalIPAddress(onComplete));
    }

    private IEnumerator enumGetExternalIPAddress(Action<bool, string> onComplete)
    {
        UnityWebRequest www = UnityWebRequest.Get("http://ipinfo.io/ip");
        yield return www.SendWebRequest();

        string result = "";
        bool didSucceed = false;
        
        if (www.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.LogError(www.error);
        }
        else
        {
            string rawResult = www.downloadHandler.text;
            string emptyStringRemoved = rawResult.Replace(" ", "");
            
            result  = emptyStringRemoved.Replace("\n", "");
            Debug.Log("External IP Address = " + result);

            didSucceed = true;
        }
        
        onComplete?.Invoke(didSucceed, result);
    }
    
    public void FetchMasterServerList(Action<bool, List<ServerListEntry>> onComplete)
    {
        StartCoroutine(enumFetchMasterServerList(onComplete));
    }

    private IEnumerator enumFetchMasterServerList(Action<bool, List<ServerListEntry>> onComplete)
    {
        List<ServerListEntry> serverEntries = new List<ServerListEntry>();
        
        WWWForm form = new WWWForm(); 
        form.AddField(kAppIdKey, kMasterListSecretId);

        string masterUri = getMasterServerCommand("list"); 
        Debug.Log("Rest Command: " + masterUri);
        using (UnityWebRequest www = UnityWebRequest.Post(masterUri, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
                onComplete?.Invoke(false, serverEntries);
            }
            else
            {
                string rawText = www.downloadHandler.text;
                Debug.Log(rawText);
                
                string strippedText = rawText.Replace("::ffff:", "");
                ServerListResponse response = JsonUtility.FromJson<ServerListResponse>(strippedText);
                
                if(response != null && response.servers != null)
                {
                    serverEntries = response.servers;
                }
                
                onComplete?.Invoke(true, serverEntries);
            }
        } 
    }
    
    public void AddServerToMasterList(ServerListEntry entry, Action<long> onComplete)
    {
        if(entry != null)
        {
            StartCoroutine(enumAddServerToMasterList(entry, onComplete));            
        }
        else
        {
            onComplete?.Invoke(0);
        }
    }
    
    private IEnumerator enumAddServerToMasterList(ServerListEntry entry, Action<long> onComplete)
    {
        WWWForm form = new WWWForm(); 
        form.AddField(kAppIdKey, kMasterListSecretId);
        form.AddField(kServerName, entry.name);
        form.AddField(kServerIp, entry.ip);
        form.AddField(kServerPort, entry.port);
        form.AddField(kServerPlayerCapacity, entry.capacity);
        form.AddField(kServerPlayerCount, entry.players);
        
        if (!string.IsNullOrEmpty(entry.serverUuid))
        {
            form.AddField(kServerUuIdKey, entry.serverUuid);
        }

        string masterUri = getMasterServerCommand("add"); 
        Debug.Log("Rest Command: " + masterUri);
        
        using (UnityWebRequest www = UnityWebRequest.Post(masterUri, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
                onComplete?.Invoke(www.responseCode);
            }
            else
            {
                Debug.Log(www.responseCode);
                entry.serverUuid = www.downloadHandler.text.Trim();
                onComplete?.Invoke(www.responseCode);
            }
        } 
    }
    
    public void removeServerToMasterList(ServerListEntry entry, Action<long> onComplete)
    {
        if(entry != null)
        {
            StartCoroutine(enumRemoveServerToMasterList(entry, onComplete));
        }
        else
        {
            onComplete?.Invoke(0);
        }
    }
    
    private IEnumerator enumRemoveServerToMasterList(ServerListEntry entry, Action<long> onComplete)
    {
        WWWForm form = new WWWForm();
        form.AddField(kAppIdKey, kMasterListSecretId);
        form.AddField(kServerUuIdKey, entry.serverUuid);

        string masterUri = getMasterServerCommand("remove");
        Debug.Log("Rest Command: " + masterUri);

        using(UnityWebRequest www = UnityWebRequest.Post(masterUri, form))
        {
            yield return www.SendWebRequest();

            if(www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
                onComplete?.Invoke(www.responseCode);
            }
            else
            {
                Debug.Log(www.responseCode);
                onComplete?.Invoke(www.responseCode);
            }
        }
    }

    private GameObject OnClientSpawnHandler(SpawnMessage msg)
    {
        return onClientSpawnHandler?.Invoke(msg);
        
    }
    
    private void OnClientUnspawnHandler(GameObject obj)
    {
        onClientUnspawnHandler?.Invoke(obj);
    }

    // public void ServerBeginMatch()
    // {
    //     // StartMatchLoad startMatchMessage = new StartMatchLoad();
    //     // NetworkServer.SendToAll(startMatchMessage, Channels.DefaultReliable);
    //
    //     
    // }
    public override void OnStartServer()
    {
        Debug.Log("OnStartServer");
        
        _serverNetPlayerMap.Clear();
    
        serverSpawnSyncStore();
        setupPlayerSlotGenerator();
        
        NetworkServer.RegisterHandler<RequestMatchStart>(OnServerRequestMatchStart, false);
        NetworkServer.RegisterHandler<RequestReadyUp>(OnServerRequestReadyUp, false);
        NetworkServer.RegisterHandler<PlayerMatchLoadComplete>(OnServerPlayerMatchLoadComplete, false);
        NetworkServer.RegisterHandler<SendPlayerInput>(OnServerSendPlayerInput, false);
        
        NetworkServer.OnConnectedEvent    += OnServerConnect;
        NetworkServer.OnDisconnectedEvent += OnServerDisconnect;
        
        
        sessionState =  SessionState.IN_LOBBY;
        
        onServerStarted?.Invoke();
    }

    private void serverSpawnSyncStore()
    {
        GameObject syncObjectPrefab = syncStatePrefab;
        GameObject syncInstance = GameObject.Instantiate(syncObjectPrefab, transform);
        
        NetworkServer.Spawn(syncInstance);
        
        syncStore = syncInstance.GetComponent<SyncStore>();
    }
    public override void OnStopServer()
    {
        if(syncStore != null)
        {
            NetworkServer.Destroy(syncStore.gameObject);
        }
        
        removeServerToMasterList(serverEntry, null);
        onServerStopped?.Invoke();    
    }
    
    public override void OnStartClient()
    {
        Debug.Log("OnStartClient");
        
        registerNetworkPrefabs();
        
        _clientNetPlayerMap.Clear();
        
        NetworkClient.RegisterHandler<SyncLobbyPlayers>(OnClientSyncLobbyPlayers, false);
        NetworkClient.RegisterHandler<ConfirmReadyUp>(OnClientConfirmReadyUp, false);
        NetworkClient.RegisterHandler<AssignPlayerSlot>(OnClientAssignPlayerSlot, false);
        NetworkClient.RegisterHandler<StartMatchLoad>(OnClientStartMatchLoad, false);
        NetworkClient.RegisterHandler<MatchBegin>(OnClientMatchBegin, false);
        NetworkClient.RegisterHandler<NetFrameSnapshot>(OnClientFrameSnapshot, false);
        NetworkClient.RegisterHandler<CurrentSessionUpdate>(OnCurrentSessionUpdate, false);
        NetworkClient.RegisterHandler<PlayerStateUpdate>(OnClientPlayerStateUpdate, false);
        
        NetworkClient.OnConnectedEvent    += OnClientConnected;
        NetworkClient.OnDisconnectedEvent += OnClientDisconnected;
        
        onClientStarted?.Invoke();
    }

    public override void OnStopClient()
    {
        onClientStopped?.Invoke();
        
        unregisterClientPrefabs();
        
        NetworkClient.UnregisterHandler<SyncLobbyPlayers>();
        NetworkClient.UnregisterHandler<ConfirmReadyUp>();
        NetworkClient.UnregisterHandler<AssignPlayerSlot>();
        NetworkClient.UnregisterHandler<StartMatchLoad>();
        NetworkClient.UnregisterHandler<MatchBegin>();
        NetworkClient.UnregisterHandler<NetFrameSnapshot>();
        NetworkClient.UnregisterHandler<CurrentSessionUpdate>();
        NetworkClient.UnregisterHandler<PlayerStateUpdate>();

        // if(syncStore != null)
        // {
        //     GameObject.Destroy(syncStore);
        // }
    }

    // public override void OnError(string reason)
    // {
    //     base.OnError(reason);
    //     onError?.Invoke(reason);
    // }

    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        Debug.Log("OnServerConnect");

        PlayerSlot pSlot = PlayerSlot.NONE;
        bool hasPlayerSlot = retrieveAvailablePlayerSlot(ref pSlot);

        if(hasPlayerSlot)
        {
            NetworkServer.SetClientReady(conn);
            
            NetPlayer netPlayer = new NetPlayer(
                conn.connectionId,
                pSlot,
                pSlot.ToString(),
                false);

            _serverNetPlayerMap[conn.connectionId] = netPlayer;
            syncStore.playerMap[pSlot] = netPlayer;
            
            // Assign our connected player a number
            AssignPlayerSlot assignMessage = new AssignPlayerSlot(pSlot);
            conn.Send(assignMessage, Channels.Reliable);

            CurrentSessionUpdate sessionMessage = new CurrentSessionUpdate(sessionState);
            conn.Send(sessionMessage, Channels.Reliable);
            
            // Sync player states with all clients
            SyncLobbyPlayers syncPlayersMessage = new SyncLobbyPlayers();
            syncPlayersMessage.playerList = _serverNetPlayerMap.Values.ToArray();
            NetworkServer.SendToAll(syncPlayersMessage, Channels.Reliable);
            
            onServerConnect?.Invoke(conn);
        }
        else
        {
            NetworkServer.dontListen = true;
            // No more free player slots! Sorry bud :(
            conn.Disconnect();
        }
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        Debug.Log("OnServerDisconnect");
        PlayerSlot playerSlot = PlayerSlot.NONE;
        
        if(conn != null && _serverNetPlayerMap.ContainsKey(conn.connectionId))
        {
            NetPlayer player = _serverNetPlayerMap[conn.connectionId];
            playerSlot = player.playerSlot;

            returnPlayerNumber(playerSlot);

            SyncLobbyPlayers syncPlayersMessage = new SyncLobbyPlayers();
            syncPlayersMessage.playerList = _serverNetPlayerMap.Values.ToArray();
            NetworkServer.SendToAll(syncPlayersMessage, Channels.Reliable);
            
            NetworkServer.dontListen = false;
        }

        onServerDisconnect?.Invoke(conn);
        
        _serverNetPlayerMap.Remove(conn.connectionId);
        syncStore.playerMap.Remove(playerSlot);
    }

    // public override void OnServerError(NetworkConnection conn, int errorCode)
    // {
    //     Debug.LogError("OnServerErrorL " + errorCode);
    //     onServerError?.Invoke(conn, errorCode);
    // }

    public void OnClientConnected()
    {
        Debug.Log("OnClientConnect");
        // _netPlayerMap[conn.connectionId] = new NetPlayer(conn, conn.connectionId, "P" + conn.connectionId);
        onClientConnect?.Invoke();
    }

    public void OnClientDisconnected()
    {
        Debug.Log("OnClientDisconnect");

        onClientDisconnect?.Invoke();

        if(!NetworkClient.active)
        {
            onClientLocalDisconnect?.Invoke();
        }

        // _netPlayerMap.Remove(conn.connectionId);
    }

    // public override void OnClientError(NetworkConnection conn, int errorCode)
    // {
    //     Debug.LogError("OnClientError: " + errorCode);
    //     onClientError?.Invoke(conn, errorCode);
    // }

    private void OnClientSyncLobbyPlayers(SyncLobbyPlayers msg)
    {
        _clientNetPlayerMap = msg.GetPlayerMap();
        
        onClientSyncLobbyPlayers?.Invoke(msg);
    }

    private void OnClientMatchBegin(MatchBegin msg)
    {
        Debug.Log("Client Begin Match");
        onClientMatchBegin?.Invoke(msg);
    }

    private void OnClientStartMatchLoad(StartMatchLoad msg)
    {
        onClientStartMatchLoad?.Invoke(msg);
    }
    
    private void OnClientAssignPlayerSlot(AssignPlayerSlot msg)
    {
        localPlayerSlot = msg.playerSlot;
    }

    private void OnClientFrameSnapshot(NetFrameSnapshot msg)
    {
        onClientFrameSnapshot?.Invoke( msg);
    }

    private void OnClientPlayerStateUpdate(PlayerStateUpdate msg)
    {
        onClientPlayerStateUpdate?.Invoke(msg);
    }
    
    private void OnCurrentSessionUpdate(CurrentSessionUpdate msg)
    {
        sessionState = msg.sessionState;
        onClientCurrentSession?.Invoke(msg);
    }
    
    private void OnClientConfirmReadyUp(ConfirmReadyUp msg)
    {
        Debug.LogFormat("OnClientConfirmReadyUp for Player:{0} : {1}", msg.playerSlot, msg.isReady);

        PlayerSlot playerSlot = msg.playerSlot;
        
        NetPlayer player = _clientNetPlayerMap[playerSlot];
        player.isReadyUp = msg.isReady;
        _clientNetPlayerMap[playerSlot] = player;
        
        onClientConfirmReadyUp?.Invoke(msg);
    }

    private void OnServerRequestMatchStart(NetworkConnectionToClient conn, RequestMatchStart msg)
    {
        if(conn.connectionId == NetworkServer.localConnection.connectionId)
        {
            NetworkServer.SetAllClientsNotReady();
            
            StartMatchLoad startMatchMessage = new StartMatchLoad();
            NetworkServer.SendToAll(startMatchMessage, Channels.Reliable);
        }
    }

    private void OnServerSendPlayerInput(NetworkConnectionToClient conn, SendPlayerInput msg)
    {
        onServerSendPlayerInput?.Invoke(conn, msg);
    }
    
    private void OnServerPlayerMatchLoadComplete(NetworkConnectionToClient conn, PlayerMatchLoadComplete msg)
    {
        NetPlayer player = _serverNetPlayerMap[conn.connectionId];
        player.isMatchReady = true;
        _serverNetPlayerMap[conn.connectionId] = player;
        
        Debug.Log("OnServerPlayerMatchLoadComplete: " + player.playerSlot.ToString());

        onServerMatchLoadComplete?.Invoke(conn);
        
        if(sessionState == SessionState.IN_GAME)
        {
            // sessionState == SessionState.IN_GAME;
            // conn.Send
        }
        else
        {
            bool allPlayersLoaded = hasAllPlayersLoaded();
            if(allPlayersLoaded)
            {
                Debug.Log("Server Match Start");
                
                sessionState = SessionState.IN_GAME;
                onServerMatchBegin?.Invoke();
            }
        }
    }
    
    private void OnServerRequestReadyUp(NetworkConnectionToClient conn, RequestReadyUp msg)
    {
        NetPlayer player = _serverNetPlayerMap[conn.connectionId];
        
        Debug.LogFormat("OnServerRequestReadyUp for Player:{0} : {1}", player.playerSlot, msg.isReady);

        if(player.isReadyUp != msg.isReady)
        {
            player.isReadyUp = msg.isReady;
            _serverNetPlayerMap[conn.connectionId] = player;
            syncStore.playerMap[player.playerSlot] = player;

            NetworkServer.SetClientReady(conn);
            
            bool allPlayersReady = canStartGame();
            ConfirmReadyUp readyMessage = new ConfirmReadyUp(
                                                player.playerSlot, 
                                                player.isReadyUp, 
                                                allPlayersReady);
                                                
            NetworkServer.SendToAll(readyMessage, Channels.Reliable);

            onServerConfirmReadyUp?.Invoke(conn, readyMessage);
        }
    }

    public void Disconnect()
    {
        NetworkServer.dontListen = false;
        
        if (NetworkClient.isHostClient)
        {
            StopHost();
        }
        else if (NetworkClient.active)
        {
            StopClient();
        }
        else if (NetworkServer.active)
        {
            StopServer();
        }
        
        _serverNetPlayerMap.Clear();
        _clientNetPlayerMap.Clear();
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
        _serverAvailablePlayerSlots.Clear();
        
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

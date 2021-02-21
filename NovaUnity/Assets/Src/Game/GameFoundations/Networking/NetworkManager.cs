using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using Mirage;

public class NetworkManager : Mirage.NetworkManager
{
    public const string kGameVersion = "6.6.6";

    // public const int kMaxPlayers = 4;
    public const string kSingleplayerRoom = "Singleplayer";

    private const string kMasterServerAppId = "23431909-e58c-40e3-94d2-8d90b4f9e11d";
    private const string kAppIdKey = "serverKey";
    private const string kServerUuIdKey = "serverUuid";
    private const string kServerName = "serverName";  
    private const string kServerIp = "serverIp";
    private const string kServerPort = "serverPort";    
    private const string kServerPlayerCount = "serverPlayers";   
    private const string kServerPlayerCapacity = "serverCapacity";  

    public string masterServerLocation = "http://novamaster.net";
    public int masterServerPort = 11667;
    
    
    public event Action<string> onError;
    public event Action onServerStarted;
    public event Action onServerStopped;
    public event Action onServerMatchBegin;
    public event Action<INetworkConnection> onServerConnect;
    public event Action<INetworkConnection> onServerDisconnect;
    public event Action<INetworkConnection, int> onServerError;
    public event Action<INetworkConnection, ConfirmReadyUp> onServerConfirmReadyUp;
    public event Action<INetworkConnection, SendPlayerInput> onServerSendPlayerInput;
    public event Action<INetworkConnection> onServerMatchLoadComplete;
    
    public event Action<INetworkConnection> onClientConnect;
    public event Action onClientDisconnect;
    public event Action<INetworkConnection, int> onClientError;
    public event Action<INetworkConnection, SyncLobbyPlayers> onClientSyncLobbyPlayers;
    public event Action<INetworkConnection, ConfirmReadyUp> onClientConfirmReadyUp;
    public event Action<INetworkConnection, StartMatchLoad> onClientStartMatchLoad;
    public event Action<INetworkConnection, MatchBegin> onClientMatchBegin;
    public event Action<INetworkConnection, NetFrameSnapshot> onClientFrameSnapshot;
    public event Action<INetworkConnection, CurrentSessionUpdate> onClientCurrentSession;
    public event SpawnHandlerDelegate onClientSpawnHandler;
    public event UnSpawnDelegate onClientUnspawnHandler;

    public ServerListEntry serverEntry { get; set; }
    
    public SessionState sessionState { get; private set; }

    public NetworkTime time
    {
        get
        {
            if(Client.Active)   { return Client.Time; }
            if(Server.Active)   { return Server.Time; }

            return null;
        }
    }
    public static uint frameTick { get; set; }

  

    private Dictionary<INetworkConnection, NetPlayer> _serverNetPlayerMap = new Dictionary<INetworkConnection, NetPlayer>();
    
    // Can't use ConnectionID's for the keys on the client because the connection id's won't match between server and client
    private Dictionary<PlayerSlot, NetPlayer> _clientNetPlayerMap = new Dictionary<PlayerSlot, NetPlayer>();

    private List<PlayerSlot> _serverAvailablePlayerSlots = new List<PlayerSlot>();


    public PlayerSlot localPlayerSlot { get; private set; }

    public bool isPureServer
    {
        get
        {
            return Server.Active && !Client.Active;
        }
    }

    public bool isPureClient
    {
        get
        {
            return !Server.Active && Client.Active;
        }
    }

    public bool isHostClient // Server & client 
    {
        get
        {
            return Server.Active && Client.Active;
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

    public NetPlayer GetServerPlayerFromConn(INetworkConnection conn)
    {
        return _serverNetPlayerMap[conn];
    }

    public virtual void Start()
    {
        registerNetworkPrefabs();
        
        Server.Started.AddListener(OnStartServer);
        Server.Stopped.AddListener(OnServerStopped);
        Server.Connected.AddListener(OnServerConnect);
        Server.Disconnected.AddListener(OnServerDisconnect);
        
        Client.Connected.AddListener(OnClientConnect);
        Client.Disconnected.AddListener(OnClientDisconnect);
    }
    
    
    public virtual void OnDestroy()
    {
        onError = null;
        onServerStarted = null;
        onServerStopped = null;
        onServerConnect = null;
        onServerDisconnect = null;
        onServerError = null;
        onServerConfirmReadyUp = null;
        onServerMatchBegin = null;
        onServerSendPlayerInput = null;
        onServerMatchLoadComplete = null;
        
        onClientConnect = null;
        onClientDisconnect = null;
        onClientError = null;
        onClientSyncLobbyPlayers = null;
        onClientConfirmReadyUp = null;
        onClientStartMatchLoad = null;
        onClientMatchBegin = null;
        onClientFrameSnapshot = null;
        onClientCurrentSession = null;
    }


    private void registerNetworkPrefabs()
    {
        UnitMap unitMap = Singleton.instance.gameplayResources.unitMap;
        for(int i = 0; i < unitMap.unitList.Count; ++i)
        {
            UnitMap.Unit unit = unitMap.unitList[i];
            Debug.Log("Unit: " + unit.id);
            
            ClientObjectManager.RegisterPrefab(
                unit.view.netIdentity, 
                OnClientSpawnHandler, 
                OnClientUnspawnHandler);
                
        }
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
        form.AddField(kAppIdKey, kMasterServerAppId);

        string masterUri = getMasterServerCommand("list"); 
        Debug.Log("Rest Command: " + masterUri);
        using (UnityWebRequest www = UnityWebRequest.Post(masterUri, form))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
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
        form.AddField(kAppIdKey, kMasterServerAppId);
        form.AddField(kServerUuIdKey, entry.serverUuid);
        form.AddField(kServerName, entry.name);
        form.AddField(kServerIp, entry.ip);
        form.AddField(kServerPort, entry.port);
        form.AddField(kServerPlayerCapacity, entry.capacity);
        form.AddField(kServerPlayerCount, entry.players);

        string masterUri = getMasterServerCommand("add"); 
        Debug.Log("Rest Command: " + masterUri);
        
        using (UnityWebRequest www = UnityWebRequest.Post(masterUri, form))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
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
    
    public void RemoveServerToMasterList(ServerListEntry entry, Action<long> onComplete)
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
        form.AddField(kAppIdKey, kMasterServerAppId);
        form.AddField(kServerUuIdKey, entry.serverUuid);
        form.AddField(kServerName, entry.name);
        form.AddField(kServerIp, entry.ip);
        form.AddField(kServerPort, entry.port);
        form.AddField(kServerPlayerCapacity, entry.capacity);
        form.AddField(kServerPlayerCount, entry.players);

        string masterUri = getMasterServerCommand("remove");
        Debug.Log("Rest Command: " + masterUri);

        using(UnityWebRequest www = UnityWebRequest.Post(masterUri, form))
        {
            yield return www.SendWebRequest();

            if(www.isNetworkError || www.isHttpError)
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
    
    private NetworkIdentity OnClientSpawnHandler(SpawnMessage msg)
    {
        return onClientSpawnHandler?.Invoke(msg);
        
    }
    
    private void OnClientUnspawnHandler(NetworkIdentity identity)
    {
        onClientUnspawnHandler?.Invoke(identity);
    }

    // public void ServerBeginMatch()
    // {
    //     // StartMatchLoad startMatchMessage = new StartMatchLoad();
    //     // NetworkServer.SendToAll(startMatchMessage, Channels.DefaultReliable);
    //
    //     
    // }
    private void OnStartServer()
    {
        Debug.Log("OnStartServer");
        _serverNetPlayerMap.Clear();
        
        setupPlayerSlotGenerator();
        
        sessionState = SessionState.IN_LOBBY;
        
        onServerStarted?.Invoke();
    }

    private void registerServerEvents(INetworkConnection connection)
    {
        connection.RegisterHandler<RequestMatchStart>(OnServerRequestMatchStart);
        connection.RegisterHandler<RequestReadyUp>(OnServerRequestReadyUp);
        connection.RegisterHandler<PlayerMatchLoadComplete>(OnServerPlayerMatchLoadComplete);
        connection.RegisterHandler<SendPlayerInput>(OnServerSendPlayerInput);
    }

    private void unregisterServerEvents(INetworkConnection connection)
    {
        connection.UnregisterHandler<RequestMatchStart>();
        connection.UnregisterHandler<RequestReadyUp>();
        connection.UnregisterHandler<PlayerMatchLoadComplete>();
        connection.UnregisterHandler<SendPlayerInput>();
    }

    private void registerClientEvents(INetworkConnection connection)
    {
        connection.RegisterHandler<SyncLobbyPlayers>(OnClientSyncLobbyPlayers);
        connection.RegisterHandler<ConfirmReadyUp>(OnClientConfirmReadyUp);
        connection.RegisterHandler<AssignPlayerSlot>(OnClientAssignPlayerSlot);
        connection.RegisterHandler<StartMatchLoad>(OnClientStartMatchLoad);
        connection.RegisterHandler<MatchBegin>(OnClientMatchBegin);
        connection.RegisterHandler<NetFrameSnapshot>(OnClientFrameSnapshot);
        connection.RegisterHandler<CurrentSessionUpdate>(OnCurrentSessionUpdate);
    }
    
    private void unregisterClientEvents(INetworkConnection connection)
    {
        connection.UnregisterHandler<SyncLobbyPlayers>();
        connection.UnregisterHandler<ConfirmReadyUp>();
        connection.UnregisterHandler<AssignPlayerSlot>();
        connection.UnregisterHandler<StartMatchLoad>();
        connection.UnregisterHandler<MatchBegin>();
        connection.UnregisterHandler<NetFrameSnapshot>();
        connection.UnregisterHandler<CurrentSessionUpdate>();
    }
    
    private void OnServerStopped()
    {
        unregisterServerEvents(Server.LocalConnection);
        
        RemoveServerToMasterList(serverEntry, null);
        onServerStopped?.Invoke();    
    }

    private void OnError(string reason)
    {
        // base.OnError(reason);
        onError?.Invoke(reason);
    }

    private void OnServerConnect(INetworkConnection conn)
    {
        Debug.Log("OnServerConnect");
        
        registerServerEvents(conn);
        
        PlayerSlot pSlot = PlayerSlot.NONE;
        bool hasPlayerSlot = retrieveAvailablePlayerSlot(ref pSlot);

        if(hasPlayerSlot)
        {
            _serverNetPlayerMap[conn] = new NetPlayer(
                conn,//.Identity.NetId,
                pSlot,
                pSlot.ToString(),
                false);

            // Assign our connected player a number
            AssignPlayerSlot assignMessage = new AssignPlayerSlot(pSlot);
            conn.Send(assignMessage, Channel.Reliable);

            CurrentSessionUpdate sessionMessage = new CurrentSessionUpdate(sessionState);
            conn.Send(sessionMessage, Channel.Reliable);
            
            // Sync player states with all clients
            SyncLobbyPlayers syncPlayersMessage = new SyncLobbyPlayers();
            syncPlayersMessage.playerList = _serverNetPlayerMap.Values.ToArray();
            Server.SendToAll(syncPlayersMessage, Channel.Reliable);
            
            onServerConnect?.Invoke(conn);
        }
        else
        {
            Server.Listening = false;
            // No more free player slots! Sorry bud :(
            conn.Disconnect();
        }
    }

    private void OnServerDisconnect(INetworkConnection conn)
    {
        Debug.Log("OnServerDisconnect");

        if(conn != null && _serverNetPlayerMap.ContainsKey(conn))
        {
            NetPlayer player = _serverNetPlayerMap[conn];
            PlayerSlot playerSlot = player.playerSlot;

            returnPlayerNumber(playerSlot);

            SyncLobbyPlayers syncPlayersMessage = new SyncLobbyPlayers();
            syncPlayersMessage.playerList = _serverNetPlayerMap.Values.ToArray();
            Server.SendToAll(syncPlayersMessage, Channel.Reliable);
            
            Server.Listening = true;
        }

        onServerDisconnect?.Invoke(conn);
        
        _serverNetPlayerMap.Remove(conn);
    }

    private void OnServerError(INetworkConnection conn, int errorCode)
    {
        Debug.LogError("OnServerErrorL " + errorCode);
        onServerError?.Invoke(conn, errorCode);
    }

    private void OnClientConnect(INetworkConnection conn)
    {
        Debug.Log("OnClientConnect");
        
        registerClientEvents(conn);
        
        _clientNetPlayerMap.Clear();
        
        onClientConnect?.Invoke(conn);
    }

    private void OnClientDisconnect()
    {
        Debug.Log("OnClientDisconnect");
        unregisterClientEvents(Client.Connection);
        
        onClientDisconnect?.Invoke();
    }

    private void OnClientError(INetworkConnection conn, int errorCode)
    {
        Debug.LogError("OnClientError: " + errorCode);
        onClientError?.Invoke(conn, errorCode);
    }

    private void OnClientSyncLobbyPlayers(INetworkConnection conn, SyncLobbyPlayers msg)
    {
        _clientNetPlayerMap = msg.GetPlayerMap();
        onClientSyncLobbyPlayers?.Invoke(conn, msg);
    }

    private void OnClientMatchBegin(INetworkConnection conn, MatchBegin msg)
    {
        Debug.Log("Client Begin Match");
        onClientMatchBegin?.Invoke(conn, msg);
    }

    private void OnClientStartMatchLoad(INetworkConnection conn, StartMatchLoad msg)
    {
        onClientStartMatchLoad?.Invoke(conn, msg);
    }
    
    private void OnClientAssignPlayerSlot(INetworkConnection conn, AssignPlayerSlot msg)
    {
        localPlayerSlot = msg.playerSlot;
    }

    private void OnClientFrameSnapshot(INetworkConnection conn, NetFrameSnapshot msg)
    {
        onClientFrameSnapshot?.Invoke(conn, msg);
    }

    private void OnCurrentSessionUpdate(INetworkConnection conn, CurrentSessionUpdate msg)
    {
        sessionState = msg.sessionState;
        onClientCurrentSession?.Invoke(conn, msg);
    }
    
    private void OnClientConfirmReadyUp(INetworkConnection conn, ConfirmReadyUp msg)
    {
        Debug.LogFormat("OnClientConfirmReadyUp for Player:{0} : {1}", msg.playerSlot, msg.isReady);

        PlayerSlot playerSlot = msg.playerSlot;
        
        NetPlayer player = _clientNetPlayerMap[playerSlot];
        player.isReadyUp = msg.isReady;
        _clientNetPlayerMap[playerSlot] = player;
        
        onClientConfirmReadyUp?.Invoke(conn, msg);
    }

    private void OnServerRequestMatchStart(INetworkConnection conn, RequestMatchStart msg)
    {
        if(conn == Server.LocalConnection)
        {
            StartMatchLoad startMatchMessage = new StartMatchLoad();
            Server.SendToAll(startMatchMessage, Channel.Reliable);
        }
    }

    private void OnServerSendPlayerInput(INetworkConnection conn, SendPlayerInput msg)
    {
        onServerSendPlayerInput?.Invoke(conn, msg);
    }
    
    private void OnServerPlayerMatchLoadComplete(INetworkConnection conn, PlayerMatchLoadComplete msg)
    {
        NetPlayer player = _serverNetPlayerMap[conn];
        player.isMatchReady = true;
        _serverNetPlayerMap[conn] = player;
        
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
    
    private void OnServerRequestReadyUp(INetworkConnection conn, RequestReadyUp msg)
    {
        NetPlayer player = _serverNetPlayerMap[conn];
        
        Debug.LogFormat("OnServerRequestReadyUp for Player:{0} : {1}", player.playerSlot, msg.isReady);

        if(player.isReadyUp != msg.isReady)
        {
            player.isReadyUp = msg.isReady;
            _serverNetPlayerMap[conn] = player;

            ServerObjectManager.SetClientReady(conn);
            
            bool allPlayersReady = canStartGame();
            ConfirmReadyUp readyMessage = new ConfirmReadyUp(
                                                player.playerSlot, 
                                                player.isReadyUp, 
                                                allPlayersReady);
                                                
            Server.SendToAll(readyMessage, Channel.Reliable);

            onServerConfirmReadyUp?.Invoke(conn, readyMessage);
        }
    }

    public void Disconnect()
    {
        if(Server.Active)
        {
            Server.Disconnect();
        }

        if(Client.Active)
        {
            Client.Disconnect();
        }
        
        _serverNetPlayerMap.Clear();
        _clientNetPlayerMap.Clear();
    }

    public bool isConnected
    {
        get { return Client.Active || Server.Active; }
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
    
    public INetworkConnection localPlayer
    {
        get { return Client.Connection != null ? Client.Connection : null; }
    }
}

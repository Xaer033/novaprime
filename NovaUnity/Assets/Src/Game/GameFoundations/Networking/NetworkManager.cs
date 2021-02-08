using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkManager : Mirror.NetworkManager
{
    public const string kGameVersion = "6.6.6";

    // public const int kMaxPlayers = 4;
    public const string kSingleplayerRoom = "Singleplayer";

    private const string kAppId = "23431909-e58c-40e3-94d2-8d90b4f9e11d";
    private const string kAppIdKey = "serverKey";
    private const string kServerUuIdKey = "serverUuid";
    private const string kServerName = "serverName";  
    private const string kServerPort = "serverPort";    
    private const string kServerPlayerCount = "serverPlayers";   
    private const string kServerPlayerCapacity = "serverCapacity";  
    private const string kServerIp = "ip";

    public string masterServerLocation = "http://novamaster.net";
    public int masterServerPort = 11667;
    
    
    public event Action<string> onError;
    public event Action onServerStarted;
    public event Action onServerStopped;
    public event Action onServerMatchBegin;
    public event Action<NetworkConnection> onServerConnect;
    public event Action<NetworkConnection> onServerDisconnect;
    public event Action<NetworkConnection, int> onServerError;
    public event Action<NetworkConnection, ConfirmReadyUp> onServerConfirmReadyUp;
    public event Action<NetworkConnection, SendPlayerInput> onServerSendPlayerInput;
    public event Action<NetworkConnection> onServerMatchLoadComplete;
    
    public event Action onClientStarted;
    public event Action<NetworkConnection> onClientConnect;
    public event Action<NetworkConnection> onClientDisconnect;
    public event Action<NetworkConnection> onLocalClientDisconnect;
    public event Action<NetworkConnection, int> onClientError;
    public event Action<NetworkConnection, SyncLobbyPlayers> onClientSyncLobbyPlayers;
    public event Action<NetworkConnection, ConfirmReadyUp> onClientConfirmReadyUp;
    public event Action<NetworkConnection, StartMatchLoad> onClientStartMatchLoad;
    public event Action<NetworkConnection, MatchBegin> onClientMatchBegin;
    public event Action<NetworkConnection, NetFrameSnapshot> onClientFrameSnapshot;
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
    public static uint frameTick { get; set; }
    
    public override void OnDestroy()
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
        
        
        onClientStarted = null;
        onClientConnect = null;
        onClientDisconnect = null;
        onLocalClientDisconnect = null;
        onClientError = null;
        onClientSyncLobbyPlayers = null;
        onClientConfirmReadyUp = null;
        onClientStartMatchLoad = null;
        onClientMatchBegin = null;
        onClientFrameSnapshot = null;

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

    public override void Start()
    {
        base.Start();

        UnitMap unitMap = Singleton.instance.gameplayResources.unitMap;
        for(int i = 0; i < unitMap.unitList.Count; ++i)
        {
            UnitMap.Unit unit = unitMap.unitList[i];
            Debug.Log("Unit: " + unit.id);
            
            ClientScene.RegisterPrefab(
                unit.view.gameObject, 
                unit.view.netIdentity.assetId, 
                OnClientSpawnHandler, 
                OnClientUnspawnHandler);
        }
    }

    private string getMasterServerCommand(string command)
    {
        return string.Format("{0}:{1}/{2}", masterServerLocation, masterServerPort, command); 
    }
    
    public void fetchMasterServerList(Action<bool, List<ServerListEntry>> onComplete)
    {
        StartCoroutine(enumFetchMasterServerList(onComplete));
    }

    private IEnumerator enumFetchMasterServerList(Action<bool, List<ServerListEntry>> onComplete)
    {
        WWWForm form = new WWWForm(); 
        form.AddField(kAppIdKey, kAppId);

        string masterUri = getMasterServerCommand("list"); 
        Debug.Log("Rest Command: " + masterUri);
        using (UnityWebRequest www = UnityWebRequest.Post(masterUri, form))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError(www.error);
                onComplete?.Invoke(false, null);
            }
            else
            {
                string rawText = www.downloadHandler.text;
                Debug.Log(rawText);
                
                string strippedText = rawText.Replace("::ffff:", "");
                ServerListResponse response = JsonUtility.FromJson<ServerListResponse>(strippedText);
                onComplete?.Invoke(true, response.servers);
            }
        } 
    }
    
    public void addServerToMasterList(ServerListEntry entry, Action<long> onComplete)
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
        form.AddField(kAppIdKey, kAppId);
        form.AddField(kServerUuIdKey, entry.serverUuid);
        form.AddField(kServerName, entry.name);
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
        form.AddField(kAppIdKey, kAppId);
        form.AddField(kServerUuIdKey, entry.serverUuid);
        form.AddField(kServerName, entry.name);
        form.AddField(kServerPort, entry.port);
        form.AddField(kServerPlayerCapacity, entry.capacity);
        form.AddField(kServerPlayerCount, entry.players);

        string masterUri = getMasterServerCommand("add");
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
        setupPlayerSlotGenerator();
        
        NetworkServer.RegisterHandler<RequestMatchStart>(OnServerRequestMatchStart, false);
        NetworkServer.RegisterHandler<RequestReadyUp>(OnServerRequestReadyUp, false);
        NetworkServer.RegisterHandler<PlayerMatchLoadComplete>(OnServerPlayerMatchLoadComplete, false);
        NetworkServer.RegisterHandler<SendPlayerInput>(OnServerSendPlayerInput, false);

        sessionState = SessionState.IN_LOBBY;
        
        onServerStarted?.Invoke();
    }
    
    public override void OnStopServer()
    {
        removeServerToMasterList(serverEntry, null);
        onServerStopped?.Invoke();    
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
        NetworkClient.RegisterHandler<NetFrameSnapshot>(OnClientFrameSnapshot, false);
        
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

    private void OnClientFrameSnapshot(NetworkConnection conn, NetFrameSnapshot msg)
    {
        onClientFrameSnapshot?.Invoke(conn, msg);
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

    private void OnServerSendPlayerInput(NetworkConnection conn, SendPlayerInput msg)
    {
        onServerSendPlayerInput?.Invoke(conn, msg);
    }
    
    private void OnServerPlayerMatchLoadComplete(NetworkConnection conn, PlayerMatchLoadComplete msg)
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
    
    private void OnServerRequestReadyUp(NetworkConnection conn, RequestReadyUp msg)
    {
        NetPlayer player = _serverNetPlayerMap[conn.connectionId];
        
        Debug.LogFormat("OnServerRequestReadyUp for Player:{0} : {1}", player.playerSlot, msg.isReady);

        if(player.isReadyUp != msg.isReady)
        {
            player.isReadyUp = msg.isReady;
            _serverNetPlayerMap[conn.connectionId] = player;

            NetworkServer.SetClientReady(conn);
            
            bool allPlayersReady = canStartGame();
            ConfirmReadyUp readyMessage = new ConfirmReadyUp(
                                                player.playerSlot, 
                                                player.isReadyUp, 
                                                allPlayersReady);
                                                
            NetworkServer.SendToAll(readyMessage, Channels.DefaultReliable);

            onServerConfirmReadyUp?.Invoke(conn, readyMessage);
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

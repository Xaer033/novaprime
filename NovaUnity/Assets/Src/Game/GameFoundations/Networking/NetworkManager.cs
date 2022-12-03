using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{
    public enum SessionState
    {
        NONE = 0,
        IN_LOBBY,
        IN_GAME
    }

#region Consts

    public const string kGameVersion = "6.6.6";

    // public const int kMaxPlayers = 4;
    public const string kSingleplayerRoom = "Singleplayer";

    private const string kMasterListSecretId   = "23431909-e58c-40e3-94d2-8d90b4f9e11d";
    private const string kAppIdKey             = "serverKey";
    private const string kServerUuIdKey        = "serverUuid";
    private const string kServerName           = "serverName";
    private const string kServerIp             = "serverIp";
    private const string kServerPort           = "serverPort";
    private const string kServerPlayerCount    = "serverPlayers";
    private const string kServerPlayerCapacity = "serverCapacity";

#endregion

#region Editor Properties

    public string                  masterServerLocation = "http://novamaster.net";
    public int                     masterServerPort     = 11667;
    public GameObject              syncStatePrefab;
    public NetworkSceneManagerBase sceneObjectProvider;

#endregion

#region Private Vars

    private Dictionary<PlayerRef, NetPlayer> _serverNetPlayerMap = new Dictionary<PlayerRef, NetPlayer>();
    // Can't use ConnectionID's for the keys on the client because the connection id's won't match between server and client
    private Dictionary<PlayerSlot, NetPlayer> _clientNetPlayerMap         = new Dictionary<PlayerSlot, NetPlayer>();
    private List<PlayerSlot>                  _serverAvailablePlayerSlots = new List<PlayerSlot>();
    private NetObjectPoolRoot                 _pool;

#endregion

#region Public properties

    public event Action                                     onServerStarted;
    public event Action                                     onServerStopped;
    public event Action                                     onServerMatchBegin;
    public event Action<NetworkConnection>                  onServerConnect;
    public event Action<NetworkConnection>                  onServerDisconnect;
    public event Action<NetworkConnection, ConfirmReadyUp>  onServerConfirmReadyUp;
    public event Action<NetworkConnection, SendPlayerInput> onServerSendPlayerInput;
    public event Action<NetworkRunner, PlayerRef>           onServerMatchLoadComplete;

    public event Action                                          onClientStarted;
    public event Action                                          onClientStopped;
    public event Action<NetworkConnection>                       onClientConnect;
    public event Action<NetworkConnection>                       onClientDisconnect;
    public event Action<NetworkConnection>                       onClientLocalDisconnect;
    public event Action<NetworkConnection, SyncLobbyPlayers>     onClientSyncLobbyPlayers;
    public event Action<NetworkConnection, ConfirmReadyUp>       onClientConfirmReadyUp;
    public event Action<NetworkConnection, StartMatchLoad>       onClientStartMatchLoad;
    public event Action<NetworkConnection, MatchBegin>           onClientMatchBegin;
    public event Action<NetworkConnection, NetFrameSnapshot>     onClientFrameSnapshot;
    public event Action<NetworkConnection, CurrentSessionUpdate> onClientCurrentSession;
    public event Action<NetworkConnection, PlayerStateUpdate>    onClientPlayerStateUpdate;
    public event Action<NetworkRunner>                           onSceneLoadDone;
    public event Action<NetworkRunner>                           onSceneLoadStart;

    // public event SpawnHandlerDelegate onClientSpawnHandler;
    // public event UnSpawnDelegate onClientUnspawnHandler;

    public        NetworkRunner   runner          { get; private set; }
    public        SessionState    sessionState    { get; private set; }
    public        SyncStore       syncStore       { get; private set; }
    public        PlayerSlot      localPlayerSlot { get; private set; }
    public        ServerListEntry serverEntry     { get; set; }
    public static uint            frameTick       { get; set; }

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

    public bool isConnected
    {
        get { return NetworkClient.active || NetworkServer.active; }
    }

    // public NetworkConnection localPlayer
    // {
    //     get { return NetworkClient.connection != null ? NetworkClient.connection : null; }
    // }

#endregion

#region Public API

    public static string GetLocalIPAddress()
    {
        var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }

        return "";
    }

    public Dictionary<PlayerSlot, NetPlayer> GetClientPlayerMap()
    {
        return _clientNetPlayerMap;
    }

    public Dictionary<PlayerSlot, NetPlayer> GetServerPlayerMap()
    {
        var playerMap = new Dictionary<PlayerSlot, NetPlayer>();
        foreach (var pair in _serverNetPlayerMap)
        {
            NetPlayer player = pair.Value;
            playerMap[player.playerSlot] = player;
        }

        return playerMap;
    }

    public NetPlayer GetServerPlayerFromRef(PlayerRef playerRef)
    {
        return _serverNetPlayerMap[playerRef];
    }

    public async void StartSingleplayer()
    {
        StartGameArgs args = new StartGameArgs
        {
            Address             = NetAddress.LocalhostIPv4(0),
            SceneObjectProvider = sceneObjectProvider,
            ObjectPool          = _pool,
            Config              = NetworkProjectConfig.Global,
            GameMode            = GameMode.Server,
        };
        await runner.StartGame(args);

        onServerStarted?.Invoke();
    }

    public async void StartServer(NetAddress address)
    {
        StartGameArgs args = new StartGameArgs
        {
            Address             = address,
            SceneObjectProvider = sceneObjectProvider,
            ObjectPool          = _pool,
            Config              = NetworkProjectConfig.Global,
            GameMode            = GameMode.Server,
        };
        
        await runner.StartGame(args);

        onServerStarted?.Invoke();
    }

    public void StartHost(int port)
    {

    }

    public void StartClient(Uri uri)
    {

    }

    // public void ServerBeginMatch()
    // {
    //     // StartMatchLoad startMatchMessage = new StartMatchLoad();
    //     // NetworkServer.SendToAll(startMatchMessage, Channels.DefaultReliable); 
    // }

    public void FetchMasterServerList(Action<bool, List<ServerListEntry>> onComplete)
    {
        StartCoroutine(enumFetchMasterServerList(onComplete));
    }

    public void Disconnect()
    {
        _serverNetPlayerMap.Clear();
        _clientNetPlayerMap.Clear();
    }

    public void AddServerToMasterList(ServerListEntry entry, Action<long> onComplete)
    {
        if (entry != null)
        {
            StartCoroutine(enumAddServerToMasterList(entry, onComplete));
        }
        else
        {
            onComplete?.Invoke(0);
        }
    }


    public void RemoveServerToMasterList(ServerListEntry entry, Action<long> onComplete)
    {
        if (entry != null)
        {
            StartCoroutine(enumRemoveServerToMasterList(entry, onComplete));
        }
        else
        {
            onComplete?.Invoke(0);
        }
    }

#endregion

#region Unity Callbacks

    protected void Awake()
    {
        _pool = new NetObjectPoolRoot(transform);

        runner = gameObject.GetComponent<NetworkRunner>();
        runner.AddCallbacks(this);

    }

    public virtual void OnDestroy()
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
    }

#endregion

#region Callbacks


    public void OnStopServer()
    {
        if (syncStore != null)
        {
            // NetworkServer.Destroy(syncStore.gameObject);
        }

        RemoveServerToMasterList(serverEntry, null);
        // onServerStopped?.Invoke();    
    }

    public void OnStartClient()
    {
      
    }


    public void OnStartServer()
    {
        Debug.Log("OnStartServer");

        _serverNetPlayerMap.Clear();
        setupPlayerSlotGenerator();

        sessionState = SessionState.IN_LOBBY;
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef playerRef)
    {
        throw new NotImplementedException();
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        throw new NotImplementedException();
    }
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        throw new NotImplementedException();
    }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        throw new NotImplementedException();
    }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        throw new NotImplementedException();
    }
    public void OnConnectedToServer(NetworkRunner runner)
    {
        throw new NotImplementedException();
    }
    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
        throw new NotImplementedException();
    }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        throw new NotImplementedException();
    }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        throw new NotImplementedException();
    }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        throw new NotImplementedException();
    }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        throw new NotImplementedException();
    }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        throw new NotImplementedException();
    }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        throw new NotImplementedException();
    }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {
        throw new NotImplementedException();
    }
    public void OnSceneLoadDone(NetworkRunner runner)
    {
        onSceneLoadDone?.Invoke(runner);

        if (runner.IsServer)
        {
            onServerMatchLoadComplete?.Invoke(runner, runner.LocalPlayer);
        }
    }
    public void OnSceneLoadStart(NetworkRunner runner)
    {
        onSceneLoadStart?.Invoke(runner);
    }

#endregion

#region Private methods



    private void onClientSyncStoreDestroy(GameObject obj)
    {
        Destroy(obj);
    }

    private void serverSpawnSyncStore()
    {
        GameObject syncObjectPrefab = syncStatePrefab;
        GameObject syncInstance     = Instantiate(syncObjectPrefab, transform);

        // NetworkServer.Spawn(syncInstance);

        syncStore = syncInstance.GetComponent<SyncStore>();
    }


    private bool retrieveAvailablePlayerSlot(ref PlayerSlot num)
    {
        int availablePlayerCount = _serverAvailablePlayerSlots.Count;
        if (availablePlayerCount <= 0)
        {
            return false;
        }

        int index = availablePlayerCount - 1;
        num = _serverAvailablePlayerSlots[index];
        _serverAvailablePlayerSlots.RemoveAt(index);

        return true;
    }
    //
    // private bool canStartGame()
    // {
    //     foreach(var player in _serverNetPlayerMap)
    //     {
    //         if(!player.Value.isReadyUp)
    //         {
    //             return false;
    //         }
    //     }
    //
    //     return true;
    // }
    //
    private bool hasAllPlayersLoaded()
    {
        foreach (var player in _serverNetPlayerMap)
        {
            if (!player.Value.isMatchReady)
            {
                return false;
            }
        }

        return true;
    }
    //
    // private void returnPlayerNumber(PlayerSlot num)
    // {
    //     _serverAvailablePlayerSlots.Add(num);
    //     _serverAvailablePlayerSlots.Sort((a, b) =>
    //     {
    //         return b.CompareTo(a);
    //     });
    // }
    //
    private void setupPlayerSlotGenerator()
    {
        _serverAvailablePlayerSlots.Clear();

        foreach (PlayerSlot pNum in Enum.GetValues(typeof(PlayerSlot)))
        {
            if (pNum == PlayerSlot.NONE) { continue; }
            if (pNum == PlayerSlot.MAX_PLAYERS) { continue; }

            _serverAvailablePlayerSlots.Add(pNum);
        }

        _serverAvailablePlayerSlots.Sort((a, b) =>
        {
            return b.CompareTo(a);
        });
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

        string result     = "";
        bool   didSucceed = false;

        if (www.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.LogError(www.error);
        }
        else
        {
            string rawResult          = www.downloadHandler.text;
            string emptyStringRemoved = rawResult.Replace(" ", "");

            result = emptyStringRemoved.Replace("\n", "");
            Debug.Log("External IP Address = " + result);

            didSucceed = true;
        }

        onComplete?.Invoke(didSucceed, result);
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

                string             strippedText = rawText.Replace("::ffff:", "");
                ServerListResponse response     = JsonUtility.FromJson<ServerListResponse>(strippedText);

                if (response != null && response.servers != null)
                {
                    serverEntries = response.servers;
                }

                onComplete?.Invoke(true, serverEntries);
            }
        }
    }

    private IEnumerator enumAddServerToMasterList(ServerListEntry entry, Action<long> onComplete)
    {
        WWWForm form = new WWWForm();
        form.AddField(kAppIdKey, kMasterListSecretId);
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

            if (www.result != UnityWebRequest.Result.Success)
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

    private IEnumerator enumRemoveServerToMasterList(ServerListEntry entry, Action<long> onComplete)
    {
        WWWForm form = new WWWForm();
        form.AddField(kAppIdKey, kMasterListSecretId);
        form.AddField(kServerUuIdKey, entry.serverUuid);
        form.AddField(kServerName, entry.name);
        form.AddField(kServerIp, entry.ip);
        form.AddField(kServerPort, entry.port);
        form.AddField(kServerPlayerCapacity, entry.capacity);
        form.AddField(kServerPlayerCount, entry.players);

        string masterUri = getMasterServerCommand("remove");
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
                onComplete?.Invoke(www.responseCode);
            }
        }
    }

#endregion
}

public class NetworkClient
{
    public static bool active { get; }
}

public class NetworkServer
{
    public static bool active { get; }
}

public class NetworkConnection
{
    public int connectionId;
}

using System;
using Mirror;
using UnityEngine;

public class NetworkManager : Mirror.NetworkManager
{

    public const string kGameVersion = "6.6.6";
    public const int kMaxPlayers = 4;
    public const string kSingleplayerRoom = "Singleplayer";
    
    public event Action onServerStarted;
    public event Action<string> onError;
    public event Action<NetworkConnection> onServerConnect;
    public event Action<NetworkConnection> onServerDisconnect;
    public event Action<NetworkConnection, int> onServerError;
    public event Action<NetworkConnection> onClientConnect;
    public event Action<NetworkConnection> onClientDisconnect;
    public event Action<NetworkConnection, int> onClientError;
   
    
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
        onServerStarted?.Invoke();   
    }

    public override void OnError(string reason)
    {
        base.OnError(reason);
        onError?.Invoke(reason);
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        Debug.Log("OnServerConnect");
        onServerConnect?.Invoke(conn);   
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        Debug.Log("OnServerDisconnect");
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
        onClientConnect?.Invoke(conn);
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        Debug.Log("OnClientDisconnect");
        onClientDisconnect?.Invoke(conn);
    }

    public override void OnClientError(NetworkConnection conn, int errorCode)
    {
        Debug.LogError("OnClientError: " + errorCode);
        onClientError?.Invoke(conn, errorCode);
    }
    
    public override void OnDestroy()
    {
        onServerStarted = null;
        onServerConnect = null; 
        onServerDisconnect = null;
        onServerError = null;
        onClientConnect = null;
        onClientDisconnect = null;
        onClientError = null;
        onError = null;
   
        base.OnDestroy();
    }

    // public List<RoomInfo> GetRoomList()
    // {
    //     return _roomList;
    // }
    
    
    // public void StartRoom(string roomId, RoomOptions options)
    // {
    //     if (PhotonNetwork.InRoom)
    //     {
    //         Debug.LogError("Network: Already in Session: " + PhotonNetwork.CurrentRoom.Name);
    //         return;
    //     }
    //
    //     PhotonNetwork.CreateRoom(roomId, options);
    // }

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
    

    private void safeCall(Action callback)
    {
        if(callback != null)
        {
            callback();
        }
    }
}

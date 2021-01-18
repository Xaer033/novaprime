using System;
using System.Linq;
using Bolt;
using Bolt.Matchmaking;
using ExitGames.Client.Photon;
using UdpKit;
using UnityEngine;

public class NetworkManager  : GlobalEventListener//, IInitializable, ILateDisposable, IOnEventCallback
{

    public const string kGameVersion = "0.1.0";
    public const int kMaxPlayers = 4;

    public event Action<byte , object , int > onCustomEvent;

    public event Action onCreatedRoom;
    public event Action onJoinedLobby;
    public event Action onJoinedRoom;
    public event Action onLeftLobby;
    public event Action onLeftRoom;
    public event Action onNetworkStart;
    public event Action<UdpSession, IProtocolToken> onSessionJoined;
    public event Action<BoltConnection> onConnection;
    public event Action<Map<Guid, UdpSession>> onSessionListUpdated;
    
    // public event Action<List<RoomInfo>> onReceivedRoomListUpdate;
    // public event Action<Player> onPlayerConnected;
    // public event Action<Player> onPlayerDisconnected;

    public void Initialize()
    {
        
//        PhotonNetwork.OnEventCall += onCustomEventCallback;
    }

    public void LateDispose()
    {
//        PhotonNetwork.OnEventCall -= onCustomEventCallback;

        onCreatedRoom = null;
        onJoinedLobby = null;
        onJoinedRoom = null;
        onLeftLobby = null;
        onLeftRoom = null;
        onNetworkStart = null;
        onConnection = null;
        onSessionJoined = null;
        onSessionListUpdated = null;

        if (BoltNetwork.IsConnected)
        {
            Disconnect();
        }
    }

    public void CreateSession(string sessionId, IProtocolToken token)
    {
        BoltMatchmaking.CreateSession(sessionId, token);
    }
    
    public override void SessionListUpdated(Map<Guid, UdpSession> sessionList)
    {
        if(onSessionListUpdated != null)
        {
            onSessionListUpdated(sessionList);
        }
    }

    public void StartServer(UdpEndPoint endPoint)
    {
        if (BoltNetwork.IsConnected)
        {
            Debug.Log("Network: Already Connected!");
            return;
        }

        //PhotonNetwork.autoJoinLobby = true;
        BoltLauncher.StartServer(endPoint);
    }

    public void StartSinglePlayer()
    {
        BoltLauncher.StartSinglePlayer();
    }
    
    public void JoinServer(UdpEndPoint endPoint)
    {
        BoltLauncher.StartClient(endPoint);
    }
    
    public override void BoltStartFailed(UdpConnectionDisconnectReason disconnectReason)
    {
        BoltLog.Error("Failed to Start");
    }

    public override void BoltStartDone()
    {
        if(onNetworkStart != null)
        {
            onNetworkStart();
        }
    }

    public override void Connected(BoltConnection connection)
    {
        if(onConnection != null)
        {
            onConnection(connection);
        }
    }

    public override void SessionConnected(UdpSession session, IProtocolToken token)
    {
        if (onSessionJoined != null)
        {
            onSessionJoined(session, token);
        }
    }
    
    public void OnConnectedToServer()
    {
        // if (onConnection != null)
        // {
        //     onConnection()
        // }
    }

    public void Disconnect()
    {
        BoltLauncher.Shutdown();
    }

    public bool isConnected
    {
        get { return BoltNetwork.IsConnected; }
    }

    public BoltConnection GetPlayerById(uint playerId)
    {
        BoltConnection connection = default;
        
        BoltConnection[] playerList = BoltNetwork.Connections.ToList().ToArray();
    
        int count = playerList.Length;
        for(int i = 0; i < count; ++i)
        {
            if(playerId == playerList[i].ConnectionId)
            {
                connection = playerList[i];
                break;
            }
        }
        return connection;
    }

    public void OnNetworkStartDone()
    {
        
    }
    /// PUN Callbacks
    public void OnCreatedRoom()
    {
        if(onCreatedRoom != null)
        {
            onCreatedRoom();
        }
    }

    public void OnEvent(EventData photonEvent)
    {
        if (onCustomEvent != null)
        {
            onCustomEvent(photonEvent.Code, photonEvent.CustomData, photonEvent.Sender );
        }
    }


    public virtual void OnError(object[] codeAndMsg)
    {
        Debug.Log(string.Format("Error:{0}, {1}", codeAndMsg[0], codeAndMsg[1]));
    }

    // public void OnRoomListUpdate(List<RoomInfo> roomList)
    // {
    //     Debug.Log("-On Received Room list update: "  + roomList.Count);
    //     if (onReceivedRoomListUpdate != null)
    //     {
    //         onReceivedRoomListUpdate(roomList);
    //     }
    // }

    public void OnConnectedToMaster()
    {
        //Debug.Log("-Joining Lobby-");
        //PhotonNetwork.JoinLobby();
    }

    public void OnJoinedLobby(UdpEvent updEvent)
    {
        Debug.Log("Joined Lobby: " + updEvent.ToString());
        safeCall(onJoinedLobby);
    }

    public void OnJoinedRoom()
    {
        safeCall(onJoinedRoom);
    }

    // public override void OnPlayerEnteredRoom(Player newPlayer)
    // {
    //     if(onPlayerConnected != null)
    //     {
    //         onPlayerConnected(newPlayer);
    //     }
    // }
    //
    // public override void OnPlayerLeftRoom(Player otherPlayer)
    // {
    //     if (onPlayerDisconnected != null)
    //     {
    //         onPlayerDisconnected(otherPlayer);
    //     }
    // }

    public void OnLeftRoom()
    {
        safeCall(onLeftRoom);
    }

    public void OnLeftLobby()
    {
        safeCall(onLeftLobby);
    }

    
    private void onCustomEventCallback(byte eventCode, object content, int senderId)
    {
        if(onCustomEvent != null)
        {
//            onCustomEvent(eventCode, content, senderId);
        }
    }

    private void safeCall(Action callback)
    {
        if(callback != null)
        {
            callback();
        }
    }
}

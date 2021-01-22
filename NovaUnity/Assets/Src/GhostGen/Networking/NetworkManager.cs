using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class NetworkManager  : MonoBehaviourPunCallbacks, IOnEventCallback
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
    public event Action<List<RoomInfo>> onReceivedRoomListUpdate;
    public event Action<Player> onPlayerConnected;
    public event Action<Player> onPlayerDisconnected;

    private List<RoomInfo> _roomList = new List<RoomInfo>();
    
    
    public bool Initialize()
    {
        return PhotonNetwork.ConnectUsingSettings();
    }
    
    public void LateDispose()
    {
    
        onCreatedRoom = null;
        onJoinedLobby = null;
        onJoinedRoom = null;
        onLeftLobby = null;
        onLeftRoom = null;
        onNetworkStart = null;
    
        if (PhotonNetwork.IsConnected)
        {
            Disconnect();
        }
    }

    public List<RoomInfo> GetRoomList()
    {
        return _roomList;
    }
    
    
    public void StartRoom(string roomId, RoomOptions options)
    {
        if (PhotonNetwork.InRoom)
        {
            Debug.Log("Network: Already in Session: " + PhotonNetwork.CurrentRoom.Name);
            return;
        }
    
        PhotonNetwork.CreateRoom(roomId, options);
    }
    //
    
    // public void OnConnectedToServer()
    // {
    //     if (onConnection != null)
    //     {
    //         onConnection()
    //     }
    // }
    
    public void Disconnect()
    {
        PhotonNetwork.Disconnect();
    }
    
    public bool isConnected
    {
        get { return PhotonNetwork.IsConnected; }
    }
    
    // public BoltConnection GetPlayerById(uint playerId)
    // {
    //     BoltConnection connection = default;
    //     
    //     BoltConnection[] playerList = BoltNetwork.Connections.ToList().ToArray();
    //
    //     int count = playerList.Length;
    //     for(int i = 0; i < count; ++i)
    //     {
    //         if(playerId == playerList[i].ConnectionId)
    //         {
    //             connection = playerList[i];
    //             break;
    //         }
    //     }
    //     return connection;
    // }
    
    public override void OnFriendListUpdate(List<FriendInfo> friendList)
    {
        throw new NotImplementedException();
    }
    
    /// PUN Callbacks
    public override void OnCreatedRoom()
    {
        if(onCreatedRoom != null)
        {
            onCreatedRoom();
        }
    }
    
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        throw new NotImplementedException();
    }
    
    public void OnEvent(EventData photonEvent)
    {
        if (onCustomEvent != null)
        {
            onCustomEvent(photonEvent.Code, photonEvent.CustomData, photonEvent.Sender );
        }
    }
    
    
    public override void OnErrorInfo(ErrorInfo errorInfo)
    {
        Debug.LogError(string.Format("Net Error:{0}", errorInfo.Info));
    }
    
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("-OnRoomListUpdate, room count: "  + roomList.Count);
        _roomList = roomList;
        
        if (onReceivedRoomListUpdate != null)
        {
            onReceivedRoomListUpdate(roomList);
        }
    }
    
    public override void OnConnected()
    {
        // throw new NotImplementedException();
    }
    
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }
    
    public override void OnDisconnected(DisconnectCause cause)
    {
        // throw new NotImplementedException();
    }
    
    public override void OnRegionListReceived(RegionHandler regionHandler)
    {
        // throw new NotImplementedException();
    }
    
    public override void OnCustomAuthenticationResponse(Dictionary<string, object> data)
    {
        // throw new NotImplementedException();
    }
    
    public override void OnCustomAuthenticationFailed(string debugMessage)
    {
        // throw new NotImplementedException();
    }
    
    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
        safeCall(onJoinedLobby);
    }
    
    public override void OnJoinedRoom()
    {
        safeCall(onJoinedRoom);
    }
    
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        // throw new NotImplementedException();
    }
    
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        // throw new NotImplementedException();
    }
    
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if(onPlayerConnected != null)
        {
            onPlayerConnected(newPlayer);
        }
    }
    
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (onPlayerDisconnected != null)
        {
            onPlayerDisconnected(otherPlayer);
        }
    }

    public override void OnLeftRoom()
    {
        safeCall(onLeftRoom);
    }
    
    public override void OnLeftLobby()
    {
        safeCall(onLeftLobby);
    }
    
    private void safeCall(Action callback)
    {
        if(callback != null)
        {
            callback();
        }
    }
}

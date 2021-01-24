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
    public const string kSingleplayerRoom = "Singleplayer";
    
    public event Action<byte , object , int > onCustomEvent;
    
    public event Action onCreatedRoom;
    public event Action onJoinedLobby;
    public event Action onJoinedRoom;
    public event Action onLeftLobby;
    public event Action onLeftRoom;
    public event Action onConnectedToMaster;
    public event Action<DisconnectCause> onNetworkDisconnected;
    public event Action<List<RoomInfo>> onReceivedRoomListUpdate;
    public event Action<Player> onPlayerConnected;
    public event Action<Player> onPlayerDisconnected;

    private List<RoomInfo> _roomList = new List<RoomInfo>();
    private Dictionary<string, RoomInfo> _cachedRoomList = new Dictionary<string, RoomInfo>();
    
    
    public bool Initialize()
    {
        _roomList.Clear();
        return PhotonNetwork.ConnectUsingSettings();
    }
    
    public void LateDispose()
    {
        onCreatedRoom = null;
        onJoinedLobby = null;
        onJoinedRoom = null;
        onLeftLobby = null;
        onLeftRoom = null;
        onConnectedToMaster = null;
        onNetworkDisconnected = null;
        onReceivedRoomListUpdate = null;
        onPlayerConnected = null;
        onPlayerDisconnected = null;
    
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
            Debug.LogError("Network: Already in Session: " + PhotonNetwork.CurrentRoom.Name);
            return;
        }
    
        PhotonNetwork.CreateRoom(roomId, options);
    }

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
        Debug.LogFormat("OnFriendListUpdate: {0}", friendList.Count);
    }
    
    /// PUN Callbacks
    public override void OnCreatedRoom()
    {
        Debug.Log("OnCreatedRoom");
        if(onCreatedRoom != null)
        {
            onCreatedRoom();
        }
    }
    
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogErrorFormat("OnCreateRoomFailed: {0}, {1}", returnCode, message);
    }
    
    public void OnEvent(EventData photonEvent)
    {
        Debug.LogFormat("OnEvent: {0}, {1} ", photonEvent.Code, photonEvent.Sender);
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
            
        
        int roomCount = roomList.Count;
        for(int i = 0; i < roomCount; ++i)
        {
            RoomInfo info = roomList[i];
            if(!info.IsVisible) { continue; }
            if (info.RemovedFromList)
            {
                _cachedRoomList.Remove(info.Name);
            }
            else
            {
                _cachedRoomList[info.Name] = info;
            }
        }
        
        
        _roomList.Clear();
        foreach(var pair in _cachedRoomList)
        {
            _roomList.Add(pair.Value);
        }
        
        if (onReceivedRoomListUpdate != null)
        {
            onReceivedRoomListUpdate(_roomList);
        }
    }
    
    public override void OnConnected()
    {
        Debug.Log("OnConnected");
        // throw new NotImplementedException();
    }
    
    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster");

        if(onConnectedToMaster != null)
        {
            onConnectedToMaster();
        }
    }
    
    public override void OnDisconnected(DisconnectCause cause)
    {
        // throw new NotImplementedException();
        Debug.Log("Disconnection: " + cause.ToString());
        if(onNetworkDisconnected != null)
        {
            onNetworkDisconnected(cause);
        }
    }
    
    public override void OnRegionListReceived(RegionHandler regionHandler)
    {
        Debug.LogFormat("OnRegionListReceived:{0}\n{1}\n", regionHandler.BestRegion.ToString(), regionHandler.GetResults());
    }
    
    public override void OnCustomAuthenticationResponse(Dictionary<string, object> data)
    {
        // throw new NotImplementedException();
    }
    
    public override void OnCustomAuthenticationFailed(string debugMessage)
    {
        // throw new NotImplementedException();
        Debug.LogErrorFormat("Network Error:{0}", debugMessage);
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
        Debug.LogErrorFormat("OnJoinRoomFailed:{0} : {1}", returnCode, message);
    }
    
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        // throw new NotImplementedException();
        Debug.LogErrorFormat("OnJoinRandomFailed:{0} : {1}", returnCode, message);
    }
    
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.LogFormat("OnPlayerEnteredRoom:{0}", newPlayer.ToStringFull());
        if(onPlayerConnected != null)
        {
            onPlayerConnected(newPlayer);
        }
    }
    
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.LogFormat("OnPlayerLeftRoom:{0}", otherPlayer.ToStringFull());
        if (onPlayerDisconnected != null)
        {
            onPlayerDisconnected(otherPlayer);
        }
    }

    public override void OnLeftRoom()
    {
        Debug.Log("OnLeftRoom");
        safeCall(onLeftRoom);
    }
    
    public override void OnLeftLobby()
    {
        Debug.Log("OnLeftLobby");
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

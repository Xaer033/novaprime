using System;
using System.Collections;
using System.Collections.Generic;
using GhostGen;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Random = UnityEngine.Random;

public class MultiplayerLobbyController : BaseController
{
    
    // private MultiplayerLobbyView _lobbyView;
    private List<Hashtable> _roomLobbyData = new List<Hashtable>();

    // private ListScrollRect _roomListView;
    private int _selectedRoomIndex;

    private NetworkManager _networkManager;

    public MultiplayerLobbyController()
    {
        _networkManager = Singleton.instance.networkManager;
    }
    
    public void Start()
    {
        _selectedRoomIndex = -1;

        viewFactory.CreateAsync<MultiplayerLobbyView>("GUI/MainMenu/MultiplayerLobbyView", (popup)=>
        {
            view = popup;
            
            view.AddListener(MenuUIEventType.JOIN_SERVER, onJoinButton);
            view.AddListener(MenuUIEventType.CREATE_SERVER, onCreateButton);
            view.AddListener(MenuUIEventType.BACK, onBackButton);

            popup.SetSelectedItemCallback(onRoomClicked);
            popup.SetLobbyDataProvider(_getRoomDataProvider(_networkManager.GetRoomList()));

            _networkManager.onJoinedRoom += onJoinedRoom;
            _networkManager.onCreatedRoom += onCreatedRoom;
            _networkManager.onNetworkStart += onNetworkStart;
            _networkManager.onReceivedRoomListUpdate += onRoomListUpdate;

        });
    }

    public override void RemoveView()
    {
        if(view != null)
        {
            view.RemoveListener(MenuUIEventType.JOIN_SERVER, onJoinButton);
            view.RemoveListener(MenuUIEventType.CREATE_SERVER, onCreateButton);
            view.RemoveListener(MenuUIEventType.BACK, onBackButton);

            _networkManager.onJoinedRoom -= onJoinedRoom;
            _networkManager.onCreatedRoom -= onCreatedRoom;
            _networkManager.onNetworkStart -= onNetworkStart;
            _networkManager.onReceivedRoomListUpdate -= onRoomListUpdate;
        }

        base.RemoveView();
    }

    private void onJoinedRoom()
    {
        Debug.Log("Room Joined: " + PhotonNetwork.CurrentRoom.Name);
        //Switch to room view
    }
    private void onCreatedRoom()
    {
        Debug.Log("Room Created");
    }
    
    private void onNetworkStart()
    {
        
    }
  
    private void onRoomListUpdate(List<RoomInfo> roomList)
    {
        if(lobbyView != null)
        {
            List<Hashtable> roomHash = _getRoomDataProvider(roomList);
            lobbyView.SetLobbyDataProvider(roomHash);
        }
    }

    private MultiplayerLobbyView lobbyView
    {
        get { return (view as MultiplayerLobbyView); }
    }
    
    private void onRoomClicked(int index, bool isSelected)
    {
        (view as MultiplayerLobbyView)._joinButton._button.interactable =  (view as MultiplayerLobbyView)._listScrollRect.selectedIndex >= 0;
        if(isSelected)
        {
            if(index >= 0 && index < _roomLobbyData.Count)
            {
                Debug.Log("Item: " + _roomLobbyData[index]["roomName"]);
                _selectedRoomIndex = index;
            }
        }
        else
        {
            _selectedRoomIndex = -1;
        }
    }
    
    private void onJoinButton(GeneralEvent e)
    {
        if(_selectedRoomIndex < 0)
        {
            return;
        }

        // view.RemoveListener(MenuUIEventType.JOIN_SERVER, onJoinButton);

        string roomName = _roomLobbyData[_selectedRoomIndex]["roomName"] as string;
        bool result = PhotonNetwork.JoinRoom(roomName);
        // PhotonNetwork.JoinRoom(roomName);

        Debug.Log(string.Format("Joining room: {0} with result: {1}", roomName, result));  
    }

    private void onCreateButton(GeneralEvent e)
    {
        // view.RemoveListener(MenuUIEventType.BACK, onBackButton);
        Debug.Log("Create a room");

        // UdpEndPoint endpoint = new UdpEndPoint(UdpIPv4Address.Localhost, 11666);
        // _networkManager.StartServer(endpoint);
        
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 4;

        string roomName = "Room_" + UnityEngine.Random.Range(0, 10000);
        PhotonNetwork.CreateRoom(roomName, options);
    }

    private void onBackButton(GeneralEvent e)
    {
        PhotonNetwork.Disconnect();
        view.RemoveListener(MenuUIEventType.BACK, onBackButton);
        DispatchEvent(MenuUIEventType.BACK);
    }



    // private void onReceivedRoomListUpdate()
    // {
    //     BoltLauncher.LocalPlayer.NickName = string.Format("PL-{0}", Random.Range(0, 1000));//SystemInfo.deviceName + "_" + UnityEngine.Random.Range(0, 2000);
    // }

    private List<Hashtable> _getRoomDataProvider(List<RoomInfo> roomInfoList)
    {
        _roomLobbyData.Clear();

        if(PhotonNetwork.InLobby || PhotonNetwork.InRoom)
        {
            Debug.Log("Is inside Lobby: " + PhotonNetwork.CurrentLobby.Name) ;
            List<RoomInfo> roomList = roomInfoList; 
            
            int roomCount = roomList.Count;
            for(int i = 0; i < roomCount; ++i)
            {
                Hashtable roomData = new Hashtable();
            
                RoomInfo info = roomList[i];
                if(!info.IsVisible) { continue; }
            
                roomData.Add("roomName", info.Name);
                roomData.Add("playerCount", string.Format("{0}/{1}", info.PlayerCount, info.MaxPlayers));
                _roomLobbyData.Add(roomData);
            }
        }

        return _roomLobbyData;
    }
}


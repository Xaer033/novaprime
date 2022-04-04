using System.Collections;
using System.Collections.Generic;
using GhostGen;
using UnityEngine;

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
    
    public override void Start()
    {
        _selectedRoomIndex = -1;

        viewFactory.CreateAsync<MultiplayerLobbyView>("GUI/MainMenu/MultiplayerLobbyView", (popup)=>
        {
            view = popup;
            
            view.AddListener(MenuUIEventType.JOIN_SERVER, onJoinButton);
            view.AddListener(MenuUIEventType.CREATE_SERVER, onCreateButton);
            view.AddListener(MenuUIEventType.BACK, onBackButton);

            popup.SetSelectedItemCallback(onRoomClicked);
            // popup.SetLobbyDataProvider(_generateRoomDataProvider(_networkManager.GetRoomList()));

            // _networkManager.onNetworkDisconnected += onNetworkDisconnected;
            // _networkManager.onReceivedRoomListUpdate += onRoomListUpdate;

        });
    }

    public override void RemoveView()
    {
        if(view != null)
        {
            view.RemoveListener(MenuUIEventType.JOIN_SERVER, onJoinButton);
            view.RemoveListener(MenuUIEventType.CREATE_SERVER, onCreateButton);
            view.RemoveListener(MenuUIEventType.BACK, onBackButton);

            // _networkManager.onNetworkDisconnected -= onNetworkDisconnected;
            // _networkManager.onReceivedRoomListUpdate -= onRoomListUpdate;
        }

        base.RemoveView();
    }

    private MultiplayerLobbyView lobbyView
    {
        get { return view as MultiplayerLobbyView;}
    }
    
    private void onJoinedRoom()
    {
        // Debug.Log("Room Joined: " + PhotonNetwork.CurrentRoom.Name);
        //
        // //Switch to room view
        // if(!PhotonNetwork.OfflineMode)
        // {
        //     PhotonNetwork.LocalPlayer.NickName = string.Format("P{0}", PhotonNetwork.LocalPlayer.ActorNumber);
        //     DispatchEvent(MenuUIEventType.GOTO_NETWORK_ROOM);
        // }
    }
    
    private void onCreatedRoom()
    {
        
    }
    
    private void onConnectedToMaster()
    {
        // if(!PhotonNetwork.OfflineMode)
        // {
        //     PhotonNetwork.JoinLobby();
        // }
    }

    // private void onNetworkDisconnected(DisconnectCause cause)
    // {
    //     PhotonNetwork.OfflineMode = true;
    //     PhotonNetwork.JoinRoom(NetworkManager.kSingleplayerRoom);
    //
    //     DispatchEvent(MenuUIEventType.BACK);
    // }
  
    // private void onRoomListUpdate(List<RoomInfo> roomList)
    // {
    //     if(lobbyView != null)
    //     {
    //         List<Hashtable> roomHash = _generateRoomDataProvider(roomList);
    //         lobbyView.SetLobbyDataProvider(roomHash);
    //     }
    // }

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
        // bool result = PhotonNetwork.JoinRoom(roomName);
    }

    private void onCreateButton(GeneralEvent e)
    {
        // view.RemoveListener(MenuUIEventType.BACK, onBackButton);
        Debug.Log("Create a room");

        // RoomOptions options = new RoomOptions();
        // options.MaxPlayers = 4;
        //
        // string roomName = "Room_" + Random.Range(0, 10000);
        // PhotonNetwork.CreateRoom(roomName, options);
    }

    private void onBackButton(GeneralEvent e)
    {
        view.RemoveListener(MenuUIEventType.BACK, onBackButton);
        if(_networkManager.isConnected)
        {
            _networkManager.Disconnect();
        }
        else
        {
            DispatchEvent(MenuUIEventType.BACK);
        }
    }



    // private void onReceivedRoomListUpdate()
    // {
    //     BoltLauncher.LocalPlayer.NickName = string.Format("PL-{0}", Random.Range(0, 1000));//SystemInfo.deviceName + "_" + UnityEngine.Random.Range(0, 2000);
    // }

    // private List<Hashtable> _generateRoomDataProvider(List<RoomInfo> roomInfoList)
    // {
    //     
    //     _roomLobbyData.Clear();
    //
    //     for (int i = 0; i < roomInfoList.Count; ++i)
    //     {
    //         RoomInfo info = roomInfoList[i];
    //         Hashtable roomData = new Hashtable();
    //         
    //         roomData.Add("roomName", info.Name);
    //         roomData.Add("playerCount", string.Format("{0}/{1}", info.PlayerCount, info.MaxPlayers));
    //         _roomLobbyData.Add(roomData);
    //     }
    //     return _roomLobbyData;
    // }
}


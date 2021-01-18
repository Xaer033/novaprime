using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UdpKit;
using UnityEngine;
using Random = UnityEngine.Random;

public class MultiplayerLobbyController : BaseController
{
    public static string kServerIp = "127.0.0.1";
    
    // private MultiplayerLobbyView _lobbyView;
    private List<Hashtable> _roomLobbyData = new List<Hashtable>();

    // private ListScrollRect _roomListView;
    private int _selectedRoomIndex;

    private NetworkManager _networkManager;

    public void Start()
    {
        _selectedRoomIndex = -1;
        _networkManager = Singleton.instance.networkManager;

        UdpEndPoint endpoint = UdpEndPoint.Parse(kServerIp);
        _networkManager.StartServer(endpoint);
        
        viewFactory.CreateAsync<MultiplayerLobbyView>("GUI/MultiplayerSetup/MultiplayerLobbyView", (popup)=>
        {
            view = popup;
            
            view.AddListener(MenuUIEventType.JOIN_SERVER, onJoinButton);
            view.AddListener(MenuUIEventType.CREATE_SERVER, onCreateButton);
            view.AddListener(MenuUIEventType.BACK, onBackButton);

            popup.SetSelectedItemCallback(onRoomClicked);
            popup.SetLobbyDataProvider(_getRoomDataProvider());

            _networkManager.onSessionListUpdated += onSessionListUpdate;
            _networkManager.onJoinedRoom += onJoinedRoom;

        });
    }

    public override void RemoveView()
    {
        if(view != null)
        {
            view.RemoveListener(MenuUIEventType.JOIN_SERVER, onJoinButton);
            view.RemoveListener(MenuUIEventType.CREATE_SERVER, onCreateButton);
            view.RemoveListener(MenuUIEventType.BACK, onBackButton);
        }

        _networkManager.onSessionListUpdated -= onSessionListUpdate;
        _networkManager.onJoinedRoom -= onJoinedRoom;

        base.RemoveView();
    }

    private void onSessionListUpdate(Map<Guid, UdpSession> sessionMap)
    {
        List<Hashtable> roomList = new List<Hashtable>();
        
        Debug.Log("Session List Count: " + sessionMap.Count);
        foreach(var pair in sessionMap)
        {
            Hashtable roomData = new Hashtable();
            
            Debug.Log("Session: " + pair.Key);
            UdpSession session = pair.Value;
            
            string hostName = session.HostName;
            string playerCount =  string.Format("{0}/{1}", session.ConnectionsCurrent, session.ConnectionsMax);
            roomData.Add("roomName", hostName);
            roomData.Add("playerCount", playerCount);
            
            roomList.Add(roomData);
        }

        if(lobbyView != null)
        {
            lobbyView.SetLobbyDataProvider(roomList);
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
            Debug.Log("Item: " + _roomLobbyData[index]["roomName"]);
            _selectedRoomIndex = index;
        }
        else
        {
            _selectedRoomIndex = -1;
        }
    }
    
    private void onJoinButton(GhostGen.GeneralEvent e)
    {
        if(_selectedRoomIndex < 0)
        {
            return;
        }

        view.RemoveListener(MenuUIEventType.JOIN_SERVER, onJoinButton);

        string roomName = _roomLobbyData[_selectedRoomIndex]["roomName"] as string;
        bool result = PhotonNetwork.JoinRoom(roomName);

        Debug.Log(string.Format("Joining room: {0} with result: {1}", roomName, result));  
    }

    private void onCreateButton(GhostGen.GeneralEvent e)
    {
        view.RemoveListener(MenuUIEventType.BACK, onBackButton);
        Debug.Log("Create a room");

        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 4;//(byte)PlayerGroup.kMaxPlayerCount;

        string roomName = "Room_" + Random.Range(0, 10000);
        bool result = PhotonNetwork.CreateRoom(roomName, options, null);
        Debug.Log("Create Room Result: " + result);
    }

    private void onBackButton(GhostGen.GeneralEvent e)
    {
        view.RemoveListener(MenuUIEventType.BACK, onBackButton);
        DispatchEvent(MenuUIEventType.BACK);
    }



    // private void onReceivedRoomListUpdate()
    // {
    //     BoltLauncher.LocalPlayer.NickName = string.Format("PL-{0}", Random.Range(0, 1000));//SystemInfo.deviceName + "_" + UnityEngine.Random.Range(0, 2000);
    // }

    private void onJoinedRoom()
    {
        DispatchEvent(MenuUIEventType.JOIN_SERVER);
    }

    private List<Hashtable> _getRoomDataProvider()
    {
        _roomLobbyData.Clear();

        // Debug.Log("Is inside Lobby: " + PhotonNetwork.CurrentLobby.Name) ;
        // RoomInfo[] roomList = null;// PhotonNetwork.GetRoomList();
        //
        // int roomCount = roomList.Length;
        // for(int i = 0; i < roomCount; ++i)
        // {
        //     Hashtable roomData = new Hashtable();
        //
        //     RoomInfo info = roomList[i];
        //     if(!info.IsVisible) { continue; }
        //
        //     roomData.Add("roomName", info.Name);
        //     roomData.Add("playerCount", string.Format("{0}/{1}", info.PlayerCount, info.MaxPlayers));
        //     _roomLobbyData.Add(roomData);
        // }

        return _roomLobbyData;
    }
}

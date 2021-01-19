using System;
using System.Collections;
using System.Collections.Generic;
using Bolt;
using Bolt.Matchmaking;
using Bolt.Photon;
using GhostGen;
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
            popup.SetLobbyDataProvider(_getRoomDataProvider());

            _networkManager.onSessionListUpdated += onSessionListUpdate;
            _networkManager.onSessionJoined += onSessionJoined;
            _networkManager.onConnection += onConnected;
            _networkManager.onNetworkStart += onNetworkStart;

        });
    }

    public override void RemoveView()
    {
        if(view != null)
        {
            view.RemoveListener(MenuUIEventType.JOIN_SERVER, onJoinButton);
            view.RemoveListener(MenuUIEventType.CREATE_SERVER, onCreateButton);
            view.RemoveListener(MenuUIEventType.BACK, onBackButton);

            _networkManager.onSessionListUpdated -= onSessionListUpdate;
            _networkManager.onSessionJoined -= onSessionJoined;
            _networkManager.onConnection -= onConnected;
            _networkManager.onNetworkStart -= onNetworkStart;
        }

        base.RemoveView();
    }

    private void onSessionJoined(UdpSession session, IProtocolToken token)
    {
        Debug.Log("Connected to server: " + session.Id);
    }

    private void onNetworkStart()
    {
        if(BoltNetwork.IsServer)
        { 
            _networkManager.StartSession(string.Format("test-{0}", Random.Range(0, 1000)));
        }
    }
    
    private void onConnected(BoltConnection connection)
    {
        Debug.Log("Client connected: " + connection.ConnectionId);
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
    
    private void onJoinButton(GeneralEvent e)
    {
        if(_selectedRoomIndex < 0)
        {
            return;
        }

        view.RemoveListener(MenuUIEventType.JOIN_SERVER, onJoinButton);

        string roomName = _roomLobbyData[_selectedRoomIndex]["roomName"] as string;
        // bool result = PhotonNetwork.JoinRoom(roomName);
        _networkManager.JoinSession(roomName);

        // Debug.Log(string.Format("Joining room: {0} with result: {1}", roomName, result));  
    }

    private void onCreateButton(GeneralEvent e)
    {
        view.RemoveListener(MenuUIEventType.BACK, onBackButton);
        Debug.Log("Create a room");

        var customToken = new PhotonRoomProperties();
        customToken.AddRoomProperty("maxPlayers", NetworkManager.kMaxPlayers);
        customToken.AddRoomProperty("currentPlayers", 1);

        UdpEndPoint endpoint = new UdpEndPoint(UdpIPv4Address.Localhost, 11666);
        _networkManager.StartServer(endpoint);
    }

    private void onBackButton(GeneralEvent e)
    {
        view.RemoveListener(MenuUIEventType.BACK, onBackButton);
        DispatchEvent(MenuUIEventType.BACK);
    }



    // private void onReceivedRoomListUpdate()
    // {
    //     BoltLauncher.LocalPlayer.NickName = string.Format("PL-{0}", Random.Range(0, 1000));//SystemInfo.deviceName + "_" + UnityEngine.Random.Range(0, 2000);
    // }

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

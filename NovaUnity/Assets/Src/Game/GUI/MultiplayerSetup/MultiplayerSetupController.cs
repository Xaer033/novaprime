using System;
using System.Collections.Generic;
using GhostGen;
using Mirror;
using UnityEngine;

public class MultiplayerSetupController : BaseController 
{
    private NetworkManager _networkManager;
    
    private List<object> _uiServerData = new List<object>();
    private int _selectedRoomIndex;
    private string _serverName;

    public MultiplayerSetupController()
    {
        _networkManager = Singleton.instance.networkManager;
    }
    
    public override void Start()
    {
        viewFactory.CreateAsync<MultiplayerSetupView>("GUI/MainMenu/MultiplayerSetupView", (v) =>
        {
            view = v;
    
            view.AddListener(MenuUIEventType.CREAT_SERVER_AS_HOST, onCreateServerAsHost);
            view.AddListener(MenuUIEventType.CREATE_SERVER, onCreateServer);
            view.AddListener(MenuUIEventType.JOIN_LISTED_SERVER, onJoinListedServer);
            view.AddListener(MenuUIEventType.REFRESH_SERVER_LIST, onRefreshServerList);
            view.AddListener(MenuUIEventType.JOIN_SERVER, onJoinServer);
            view.AddListener(MenuUIEventType.BACK, onBackButton);

            setupView.SetSelectedItemCallback(onRoomClicked);

            _networkManager.fetchMasterServerList(onFetchComplete);

        });
    }
    
    public override void RemoveView()
    {
        // _networkManager.onPlayerPropertiesUpdate -= onPlayerPropertiesUpdate;
        // _networkManager.onPlayerConnected -= onPlayerConnectionStatusChanged;
        // _networkManager.onPlayerDisconnected -= onPlayerConnectionStatusChanged;
        // _networkManager.onNetworkDisconnected -= onNetworkDisconnected;
        
        base.RemoveView();
    }
    
    
    private MultiplayerSetupView setupView
    {
        get { return view as MultiplayerSetupView; }
    }
    
    
    //
    // private void onNetworkDisconnected(DisconnectCause cause)
    // {
    //     DispatchEvent(MenuUIEventType.GOTO_MAIN_MENU);
    // }
   

    private void onCreateServer(GeneralEvent e)
    {
        _serverName = e.data as string;
        
        _networkManager.onServerStarted += onServerStarted;
        _networkManager.StartServer();
    }

    private void onCreateServerAsHost(GeneralEvent e)
    {
        _serverName = e.data as string;
        
        _networkManager.onServerStarted += onServerStarted;
        _networkManager.StartHost();
    }
    
    private void onServerStarted()
    {
        _networkManager.onServerStarted -= onServerStarted; 
        
        // Inform the master server
        ServerListEntry entry = new ServerListEntry
        {
            serverUuid = Guid.NewGuid().ToString(),
            name = _serverName,
            port = 11666, // for now
            players = 0,
            capacity = 10 // FOr now
        };

        _networkManager.serverEntry = entry;
        
        _networkManager.addServerToMasterList(entry, null);
        DispatchEvent(MenuUIEventType.GOTO_NETWORK_ROOM);
    }

    private void onRefreshServerList(GeneralEvent e)
    {
        setupView.StartLoadingTween();
        
        _networkManager.fetchMasterServerList(onFetchComplete);   
    }

    private void onJoinListedServer(GeneralEvent e)
    {
        ServerListEntry entry = (ServerListEntry)_uiServerData[_selectedRoomIndex];
        Uri uri = new Uri(string.Format("http://{0}:{1}", entry.ip, entry.port));
        
        Debug.Log("Joining Server: " + uri.AbsoluteUri);
        
        _networkManager.onClientConnect += onClientConnect;
        _networkManager.StartClient(uri);
    }
    
    private void onJoinServer(GeneralEvent e)
    {
        string serverIpAddress = e.data as string;
        _networkManager.networkAddress = serverIpAddress;

        _networkManager.onClientConnect += onClientConnect;
        
        Uri uri = new Uri(serverIpAddress + ":11666");
        Debug.Log("Joining Server: " + uri.AbsoluteUri);
        _networkManager.StartClient(uri);
    }
    
    private void onBackButton(GeneralEvent e)
    {
        view.RemoveListener(MenuUIEventType.BACK, onBackButton);
        // Maybe throw up a modal dialog to ask if they are sure?
        // PhotonNetwork.LeaveRoom();
        _networkManager.Disconnect();
        DispatchEvent(MenuUIEventType.GOTO_MAIN_MENU);
    }


    private void onClientConnect(NetworkConnection conn)
    {
        Debug.Log("Client Joined Server");
        if(NetworkClient.connection.connectionId == conn.connectionId)
        {
            _networkManager.onClientConnect -= onClientConnect;
            DispatchEvent(MenuUIEventType.GOTO_NETWORK_ROOM);
        }
    }

    private void onFetchComplete(bool result, List<ServerListEntry> serverList)
    {
        setupView.StopLoadingTween();
        
        if(!result || serverList == null)
        {
            return;
        }

        _uiServerData.Clear();
        
        for(int i = 0; i < serverList.Count; ++i)
        {
            _uiServerData.Add(serverList[i]);
        }
        
        setupView.SetLobbyDataProvider(_uiServerData);
    }
    
    private void onRoomClicked(int index, bool isSelected)
    {
        setupView.joinRoomButton._button.interactable =  setupView._serverListRect.selectedIndex >= 0;
        if(isSelected)
        {
            if(index >= 0 && index < _uiServerData.Count)
            {
                ServerListEntry entry = (ServerListEntry)_uiServerData[index];
                Debug.Log("Item: " + entry.name);
                _selectedRoomIndex = index;
            }
        }
        else
        {
            _selectedRoomIndex = -1;
        }
    }
}

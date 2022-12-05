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
        _networkManager.Disconnect();
        
        viewFactory.CreateAsync<MultiplayerSetupView>("GUI/MainMenu/MultiplayerSetupView", (v) =>
        {
            view = v;
    
            view.AddListener(MenuUIEventType.CREATE_SERVER_AS_HOST, onCreateServerAsHost);
            view.AddListener(MenuUIEventType.CREATE_SERVER, onCreateServer);
            view.AddListener(MenuUIEventType.JOIN_LISTED_SERVER, onJoinListedServer);
            view.AddListener(MenuUIEventType.REFRESH_SERVER_LIST, onRefreshServerList);
            view.AddListener(MenuUIEventType.JOIN_SERVER, onJoinServer);
            view.AddListener(MenuUIEventType.BACK, onBackButton);

            setupView.SetSelectedItemCallback(onRoomClicked);

            _networkManager.FetchMasterServerList(onFetchComplete);

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

        _networkManager.FetchExternalIpAddress((wasSuccessfull, ipAddress) =>
        {
            // Inform the master server
            ServerListEntry entry = new ServerListEntry
            {
                name = _serverName,
                ip = ipAddress,
                serverIp = ipAddress,
                port = 11666, // for now
                players = 1,
                capacity = 10 // FOr now
            };

            _networkManager.serverEntry = entry;
            
            _networkManager.AddServerToMasterList(entry, null);
            DispatchEvent(MenuUIEventType.GOTO_NETWORK_ROOM);
        });
    }

    private void onRefreshServerList(GeneralEvent e)
    {
        setupView.StartLoadingTween();
        
        _networkManager.FetchMasterServerList(onFetchComplete);   
    }

    private void onJoinListedServer(GeneralEvent e)
    {
        ServerListEntry entry = (ServerListEntry)_uiServerData[_selectedRoomIndex];
        Uri uri = new Uri(string.Format("http://{0}:{1}", entry.serverIp, entry.port));
        
        Debug.Log("Joining Server: " + uri.AbsoluteUri);
        
        joinServer(uri);
    }
    
    private void onJoinServer(GeneralEvent e)
    {
        string serverIpAddress = e.data as string;
        _networkManager.networkAddress = serverIpAddress;

        Uri uri = new Uri(serverIpAddress + ":11666");
        joinServer(uri);
    }

    private void joinServer(Uri uri)
    {
        _networkManager.onClientConnect += onClientConnect;
        
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


    private void onClientConnect()
    {
        Debug.Log("Client Joined Server");
        NetworkClient.Ready();
        
        _networkManager.onClientConnect -= onClientConnect;
        _networkManager.onClientCurrentSession += onClientCurrentSession;
    }

    private void onClientCurrentSession(CurrentSessionUpdate msg)
    {
        _networkManager.onClientCurrentSession -= onClientCurrentSession;

        if(msg.sessionState == NetworkManager.SessionState.IN_LOBBY)
        {
            DispatchEvent(MenuUIEventType.GOTO_NETWORK_ROOM);
        }
        else
        {
            DispatchEvent(MenuUIEventType.GOTO_MULTIPLAYER_GAME);
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

        // for(int i = 0; i < 100000; ++i)
        // {
        //     int capacityUsed = Random.Range(2, 16);
        //     ServerListEntry entry = new ServerListEntry
        //     {
        //         capacity = capacityUsed,
        //         players =  Random.Range(0, capacityUsed),
        //         ip = string.Format("{0}.{1}.{2}.{3}", Random.Range(0, 256), Random.Range(0, 256), Random.Range(0, 256), Random.Range(0, 256)),
        //         port = Random.Range(2000, 60000),
        //         name = string.Format("Neat Server: {0}", i )
        //     };
        //     _uiServerData.Add(entry);
        // }
        
        setupView.SetLobbyDataProvider(_uiServerData);
    }
    
    private void onRoomClicked(int index, bool isSelected)
    {
        setupView.joinRoomButton.interactable =  setupView._serverListRect.selectedIndex >= 0;
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

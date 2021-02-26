using System.Collections.Generic;
using GhostGen;
using Mirror;
using UnityEngine;

public class MultiplayerRoomController : BaseController 
{
    private NetworkManager _networkManager;
    private bool _isServer;
    
    public MultiplayerRoomController(bool isServer)
    {
        _isServer = isServer;
        _networkManager = Singleton.instance.networkManager;
    }
    
    public override void Start()
    {
        viewFactory.CreateAsync<MultiplayerRoomView>("GUI/MainMenu/MultiplayerRoomView", (v) =>
        {
            view = v;
    
            view.AddListener(MenuUIEventType.CONTINUE, onStartMultiplayer);
            view.AddListener(MenuUIEventType.TOGGLE, onReadyButton);
            view.AddListener(MenuUIEventType.BACK, onBackButton);

            
            if(!_isServer)
            {
                _networkManager.onClientLocalDisconnect += onClientLocalDisconnect;
            }


            if(NetworkClient.active)
            {
                _networkManager.onClientConfirmReadyUp += onClientConfirmReadyUp;
                _networkManager.onClientSyncLobbyPlayers += onClientSyncLobbyPlayers;
                _networkManager.onClientStartMatchLoad += onClientStartMatchLoad;
            }

            if(_networkManager.isPureServer)
            {
                _networkManager.onServerConnect += onServerConnect;
                _networkManager.onServerDisconnect += onServerDisconnect;
                _networkManager.onServerConfirmReadyUp += onServerConfirmReadyUp;
                
                roomView.readyButton.gameObject.SetActive(false);
            }

            var netPlayerMap = _networkManager.isPureServer
                ? _networkManager.GetServerPlayerMap()
                : _networkManager.GetClientPlayerMap();
                
            _setupPlayers(netPlayerMap);
        });
    }
    
    public override void RemoveView()
    {
        if(!_isServer)
        {
            _networkManager.onClientLocalDisconnect -= onClientLocalDisconnect;
        }

        
        _networkManager.onClientConfirmReadyUp -= onClientConfirmReadyUp;
        _networkManager.onClientSyncLobbyPlayers -= onClientSyncLobbyPlayers;
        _networkManager.onClientStartMatchLoad -= onClientStartMatchLoad;
    
        _networkManager.onServerConnect -= onServerConnect;
        _networkManager.onServerDisconnect -= onServerDisconnect;
        _networkManager.onServerConfirmReadyUp -= onServerConfirmReadyUp;
        
        base.RemoveView();
    }

    private MultiplayerRoomView roomView
    {
        get { return view as MultiplayerRoomView; }
    }


    private void onServerConnect(NetworkConnection conn)
    {
        _setupPlayers(_networkManager.GetServerPlayerMap());    
    }

    private void onServerDisconnect(NetworkConnection conn)
    {
        _setupPlayers(_networkManager.GetServerPlayerMap());
    }

    private void onServerConfirmReadyUp(NetworkConnection conn, ConfirmReadyUp msg)
    {
        if(NetworkServer.active)
        {
            roomView.startButton.gameObject.SetActive(msg.allPlayersReady);    
        }
        
        _setupPlayers(_networkManager.GetServerPlayerMap());
    }
    
    private void onClientConfirmReadyUp(NetworkConnection conn, ConfirmReadyUp msg)
    {
        if(NetworkServer.active)
        {
            roomView.startButton.gameObject.SetActive(msg.allPlayersReady);    
        }
        
        _setupPlayers(_networkManager.GetClientPlayerMap());
    }

    private void onClientSyncLobbyPlayers(NetworkConnection conn, SyncLobbyPlayers msg)
    {
        _setupPlayers(_networkManager.GetClientPlayerMap());
    }

    private void onClientStartMatchLoad(NetworkConnection conn, StartMatchLoad msg)
    {
        DispatchEvent(MenuUIEventType.GOTO_MULTIPLAYER_GAME);
    }
    
    private void onClientLocalDisconnect(NetworkConnection conn)
    {
        _networkManager.Disconnect();
        DispatchEvent(MenuUIEventType.GOTO_MULTIPLAYER_LOBBY);
    }
    
    private void onBackButton(GeneralEvent e)
    {
        view.RemoveListener(MenuUIEventType.BACK, onBackButton);
        // Maybe throw up a modal dialog to ask if they are sure?
        // PhotonNetwork.LeaveRoom();
        
        _networkManager.Disconnect();
        DispatchEvent(MenuUIEventType.GOTO_MULTIPLAYER_LOBBY);
    }

    private void onStartMultiplayer(GeneralEvent e)
    {
        view.RemoveListener(MenuUIEventType.CONTINUE, onStartMultiplayer);

        // NetworkClient.Send(new RequestMatchStart(), Channels.DefaultReliable);
        // _networkManager.OnRe
        
        DispatchEvent(MenuUIEventType.GOTO_MULTIPLAYER_GAME);
    }
    
    private void onReadyButton(GeneralEvent e)
    {
        var netPlayerMap = _networkManager.GetClientPlayerMap();

        PlayerSlot localSlot = _networkManager.localPlayerSlot;
        if(_networkManager.localPlayerSlot != PlayerSlot.NONE)
        {
            NetPlayer player = netPlayerMap[localSlot];
            bool requestedReadyState = !player.isReadyUp;
            
            Debug.Log("Ready Button State: " + requestedReadyState);
            RequestReadyUp readyRequest = new RequestReadyUp(requestedReadyState);

            NetworkClient.Send(readyRequest, Channels.DefaultReliable);
        }
    }

    private void onLeftRoom()
    {
        DispatchEvent(MenuUIEventType.GOTO_MULTIPLAYER_LOBBY);
    }

    private void _setupPlayers(Dictionary<PlayerSlot, NetPlayer> netPlayerMap)
    {
        roomView.ClearPlayerViews();
        
        List<NetPlayer> playerList = new List<NetPlayer>(netPlayerMap.Values);
        playerList.Sort((a, b) =>
        {
            return a.playerSlot.CompareTo(b.playerSlot);
        });
        
        int count = playerList.Count;
        for(int i = 0; i < (int)PlayerSlot.MAX_PLAYERS; ++i)
        {
            if(i < count)
            {
                roomView.SetPlayer(i, playerList[i]);
            }
            else
            {
                roomView.SetPlayer(i, new NetPlayer(-1));
            }
        }
    }
}

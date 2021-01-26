using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEditor;
using UnityEngine;

public class MultiplayerRoomController : BaseController 
{
    private NetworkManager _networkManager;


    public MultiplayerRoomController()
    {
        _networkManager = Singleton.instance.networkManager;
    }
    
    public void Start()
    {
        viewFactory.CreateAsync<MultiplayerRoomView>("GUI/MainMenu/MultiplayerRoomView", (v) =>
        {
            view = v;
    
            view.AddListener(MenuUIEventType.CONTINUE, onStartMultiplayer);
            view.AddListener(MenuUIEventType.TOGGLE, onReadyButton);
            view.AddListener(MenuUIEventType.BACK, onBackButton);

            _networkManager.onPlayerPropertiesUpdate += onPlayerPropertiesUpdate;
            _networkManager.onPlayerConnected += onPlayerConnectionStatusChanged;
            _networkManager.onPlayerDisconnected += onPlayerConnectionStatusChanged;
            _networkManager.onNetworkDisconnected += onNetworkDisconnected;
            
            _networkManager.onCustomEvent += onCustomEvent;
    
            _setupPlayers();
            _viewInitialization();
        });
    }
    
    public override void RemoveView()
    {
        _networkManager.onPlayerPropertiesUpdate -= onPlayerPropertiesUpdate;
        _networkManager.onPlayerConnected -= onPlayerConnectionStatusChanged;
        _networkManager.onPlayerDisconnected -= onPlayerConnectionStatusChanged;
        _networkManager.onNetworkDisconnected -= onNetworkDisconnected;
        
        _networkManager.onCustomEvent -= onCustomEvent;
    
        base.RemoveView();
    }

    public NetworkPlayer[] GetPlayerList()
    {
        if(!PhotonNetwork.IsMasterClient)
        {
            Debug.LogError("Trying To call get player List when you are not the master client, thats a paddlin'");
            return null;
        }

        NetworkPlayer[] playerStateList = new NetworkPlayer[NetworkManager.kMaxPlayers];

        Player[] playerList = PhotonNetwork.PlayerList;
        for (int i = 0; i < NetworkManager.kMaxPlayers; ++i)
        {
            if(i < playerList.Length)
            {
                playerStateList[i] = new NetworkPlayer(playerList[i]);
            }
            else
            {
                playerStateList[i] = null;
            }
        }

        return playerStateList;
    }

    
    private MultiplayerRoomView roomView
    {
        get { return view as MultiplayerRoomView; }
    }
    
    

    private void onPlayerPropertiesUpdate(Player targetPlayer, Hashtable properties)
    {
        if(roomView == null)
        {
            return;
        }
  
        int index = roomView.GetIndexForPlayerId(targetPlayer.ActorNumber);
        if(index < 0)
        {
            Debug.LogFormat("Index for ActorNumber: {0} not found", targetPlayer.ActorNumber);
            return;
        }

       const string key = "isReady";
        
        bool isReady = false;
        if(properties.ContainsKey(key))
        {
            isReady = (bool)properties[key];
        }
        
        
        Debug.LogFormat("OnPropertyUpdate Player: {0} ready: {1}", key, isReady);
        // roomView.SetIsReady(index, isReady);
        roomView.SetPlayer(index, targetPlayer, isReady);
    }

    private void onNetworkDisconnected(DisconnectCause cause)
    {
        DispatchEvent(MenuUIEventType.GOTO_MAIN_MENU);
    }
    
    private void onPlayerConnectionStatusChanged(Player newPlayer)
    {
        _setupPlayers();
        // onRoomPropertiesUpdate(PhotonNetwork.CurrentRoom.CustomProperties);
    }
    
    
    private void onBackButton(GhostGen.GeneralEvent e)
    {
        view.RemoveListener(MenuUIEventType.BACK, onBackButton);
        // Maybe throw up a modal dialog to ask if they are sure?
        _networkManager.onLeftRoom += onLeftRoom;
        PhotonNetwork.LeaveRoom();
    }

    private void onStartMultiplayer(GhostGen.GeneralEvent e)
    {
        RaiseEventOptions options = new RaiseEventOptions();
        options.Receivers = ReceiverGroup.All;

        PhotonNetwork.RaiseEvent(NetworkOpCode.START_GAMEPLAY_LOAD, null, options, SendOptions.SendReliable);
    }
    
    private void onReadyButton(GhostGen.GeneralEvent e)
    {
        const string key = "isReady";
       
        Hashtable currentProperties = PhotonNetwork.LocalPlayer.CustomProperties;
        bool isReady = false;
        if(currentProperties.ContainsKey(key))
        {
            isReady = !(bool)currentProperties[key];
        }
        else
        {
            isReady = true;
        }

        Hashtable newProperties = currentProperties;//new Hashtable();
        newProperties[key] = isReady;
        Debug.LogFormat("Setting Property Player: {0} ready: {1}", key, isReady);
        PhotonNetwork.LocalPlayer.SetCustomProperties(newProperties);
    }

    private void onLeftRoom()
    {
        _networkManager.onLeftRoom -= onLeftRoom;
        DispatchEvent(MenuUIEventType.GOTO_MULTIPLAYER_LOBBY);
    }

    private void onCustomEvent(byte eventCode, object content, int senderId)
    {
        if(eventCode == NetworkOpCode.START_GAMEPLAY_LOAD)
        {
            DispatchEvent(MenuUIEventType.GOTO_MULTIPLAYER_GAME);
        }
    }

    private void _viewInitialization()
    {
        roomView.SetTitle(PhotonNetwork.CurrentRoom.Name);
        bool isMaster = PhotonNetwork.IsMasterClient;
        roomView.IsMasterClient(isMaster);
    }
    
    private void _setupPlayers()
    {
        List<Player> playerList = new List<Player>(PhotonNetwork.PlayerList);
        playerList.Sort((a, b) =>
        {
            if(a == null || b == null) { return 0; }
            return a.ActorNumber.CompareTo(b.ActorNumber);
        });

        const string key = "isReady";
        
        int count = playerList.Count;
        for(int i = 0; i < NetworkManager.kMaxPlayers; ++i)
        {
            if(i < count)
            {
                bool isReady = false;
                if(playerList[i].CustomProperties.ContainsKey(key))
                {
                    isReady = (bool)playerList[i].CustomProperties[key];
                }
                roomView.SetPlayer(i, playerList[i], isReady);
            }
            else
            {
                roomView.SetPlayer(i, null, false);
            }
        }
    
        roomView.IsMasterClient(PhotonNetwork.IsMasterClient);
    }
}

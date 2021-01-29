using System.Collections.Generic;
using GhostGen;
using Mirror;
using UnityEngine;

public class ClientMultiplayerRoomController : BaseController 
{
    private NetworkManager _networkManager;


    public ClientMultiplayerRoomController()
    {
        _networkManager = Singleton.instance.networkManager;
    }
    
    public override void Start()
    {
        viewFactory.CreateAsync<ClientMultiplayerRoomView>("GUI/MainMenu/ClientMultiplayerRoomView", (v) =>
        {
            view = v;
    
            view.AddListener(MenuUIEventType.CONTINUE, onStartMultiplayer);
            view.AddListener(MenuUIEventType.TOGGLE, onReadyButton);
            view.AddListener(MenuUIEventType.BACK, onBackButton);

            _networkManager.onClientDisconnect += onClientDisconnect;
            
    
            _setupPlayers();
            _viewInitialization();
        });
    }
    
    public override void RemoveView()
    {
        _networkManager.onClientDisconnect -= onClientDisconnect;
        base.RemoveView();
    }

    // public NetPlayer[] GetPlayerList()
    // {
    //     if(!PhotonNetwork.IsMasterClient)
    //     {
    //         Debug.LogError("Trying To call get player List when you are not the master client, thats a paddlin'");
    //         return null;
    //     }
    //
    //     NetPlayer[] playerStateList = new NetPlayer[NetworkManager.kMaxPlayers];
    //
    //     Player[] playerList = PhotonNetwork.PlayerList;
    //     for (int i = 0; i < NetworkManager.kMaxPlayers; ++i)
    //     {
    //         if(i < playerList.Length)
    //         {
    //             playerStateList[i] = new NetPlayer(playerList[i]);
    //         }
    //         else
    //         {
    //             playerStateList[i] = null;
    //         }
    //     }
    //
    //     return playerStateList;
    // }

    
    private ClientMultiplayerRoomView roomView
    {
        get { return view as ClientMultiplayerRoomView; }
    }
    
    

    // private void onPlayerPropertiesUpdate(Player targetPlayer, Hashtable properties)
    // {
    //     if(roomView == null)
    //     {
    //         return;
    //     }
    //
    //     int index = roomView.GetIndexForPlayerId(targetPlayer.ActorNumber);
    //     if(index < 0)
    //     {
    //         Debug.LogFormat("Index for ActorNumber: {0} not found", targetPlayer.ActorNumber);
    //         return;
    //     }
    //
    //    const string key = "isReady";
    //     
    //     bool isReady = false;
    //     if(properties.ContainsKey(key))
    //     {
    //         isReady = (bool)properties[key];
    //     }
    //     
    //     
    //     Debug.LogFormat("OnPropertyUpdate Player: {0} ready: {1}", key, isReady);
    //     // roomView.SetIsReady(index, isReady);
    //     roomView.SetPlayer(index, targetPlayer, isReady);
    // }
    //
    // private void onNetworkDisconnected(DisconnectCause cause)
    // {
    //     DispatchEvent(MenuUIEventType.GOTO_MAIN_MENU);
    // }
    //
    // private void onPlayerConnectionStatusChanged(Player newPlayer)
    // {
    //     _setupPlayers();
    //     // onRoomPropertiesUpdate(PhotonNetwork.CurrentRoom.CustomProperties);
    // }
    private void onClientDisconnect(NetworkConnection conn)
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
        // RaiseEventOptions options = new RaiseEventOptions();
        // options.Receivers = ReceiverGroup.All;
        //
        // PhotonNetwork.RaiseEvent(NetworkOpCode.START_GAMEPLAY_LOAD, null, options, SendOptions.SendReliable);
    }
    
    private void onReadyButton(GeneralEvent e)
    {
        const string key = "isReady";
       
        // Hashtable currentProperties = PhotonNetwork.LocalPlayer.CustomProperties;
        // bool isReady = false;
        // if(currentProperties.ContainsKey(key))
        // {
        //     isReady = !(bool)currentProperties[key];
        // }
        // else
        // {
        //     isReady = true;
        // }
        //
        // Hashtable newProperties = currentProperties;//new Hashtable();
        // newProperties[key] = isReady;
        // Debug.LogFormat("Setting Property Player: {0} ready: {1}", key, isReady);
        // PhotonNetwork.LocalPlayer.SetCustomProperties(newProperties);
    }

    private void onLeftRoom()
    {
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
        // roomView.SetTitle(PhotonNetwork.CurrentRoom.Name);
        // bool isMaster = PhotonNetwork.IsMasterClient;
        // roomView.IsMasterClient(isMaster);
    }
    
    private void _setupPlayers()
    {
        List<NetPlayer> playerList = new List<NetPlayer>();//PhotonNetwork.PlayerList);
        playerList.Sort((a, b) =>
        {
            if(a == null || b == null) { return 0; }
            return a.id.CompareTo(b.id);
        });

        const string key = "isReady";
        
        int count = playerList.Count;
        for(int i = 0; i < NetworkManager.kMaxPlayers; ++i)
        {
            if(i < count)
            {
                bool isReady = false;
                // if(playerList[i].CustomProperties.ContainsKey(key))
                // {
                //     isReady = (bool)playerList[i].CustomProperties[key];
                // }
                roomView.SetPlayer(i, playerList[i], isReady);
            }
            else
            {
                roomView.SetPlayer(i, null, false);
            }
        }
    
        // roomView.IsMasterClient(PhotonNetwork.IsMasterClient);
    }
}

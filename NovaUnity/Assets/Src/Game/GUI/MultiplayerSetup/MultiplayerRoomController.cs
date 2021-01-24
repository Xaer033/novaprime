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
    
            view.AddListener(MenuUIEventType.TOGGLE, onReadyButton);
            view.AddListener(MenuUIEventType.BACK, onBackButton);
            
            _networkManager.onPlayerConnected += onPlayerConnectionStatusChanged;
            _networkManager.onPlayerDisconnected += onPlayerConnectionStatusChanged;
            _networkManager.onLeftRoom += onLeftRoom;
            _networkManager.onCustomEvent += onCustomEvent;
    
            _setupPlayers();
            _viewInitialization();
        });
    }
    
    public override void RemoveView()
    {
        _networkManager.onPlayerConnected -= onPlayerConnectionStatusChanged;
        _networkManager.onPlayerDisconnected -= onPlayerConnectionStatusChanged;
        _networkManager.onLeftRoom -= onLeftRoom;
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
    
    
    // private void _addButtonCallbacks(bool isMaster)
    // {
    //     _roomView.leaveButton.onClick.AddListener(_onLeaveRoomButton);
    //
    //     if (isMaster)
    //     {
    //         _roomView.startButton.onClick.AddListener(_onStartGameButton);
    //     }
    //     else
    //     {
    //         _roomView.readyToggle.onValueChanged.AddListener(_onReadyButton);
    //     }
    // }

    
    private void onPlayerConnectionStatusChanged(Player newPlayer)
    {
        
        // _removeButtonCallbacks();
        _setupPlayers();
        // _addButtonCallbacks(PhotonNetwork.isMasterClient);
    }
    
    
    private void onBackButton(GhostGen.GeneralEvent e)
    {
        view.RemoveListener(MenuUIEventType.BACK, onBackButton);
        // Maybe throw up a modal dialog to ask if they are sure?
        PhotonNetwork.LeaveRoom();
    }
    
    private void onReadyButton(GhostGen.GeneralEvent e)
    {
        bool isSelected = (bool)e.data;
        RaiseEventOptions options = new RaiseEventOptions();
        options.Receivers = ReceiverGroup.All;
        
        PhotonNetwork.RaiseEvent(NetworkOpCode.READY_TOGGLE, isSelected, options, SendOptions.SendReliable);
    }

    private void onLeftRoom()
    {
        DispatchEvent(MenuUIEventType.GOTO_MULTIPLAYER_LOBBY);
    }

    private void onCustomEvent(byte eventCode, object content, int senderId)
    {
        if(eventCode == NetworkOpCode.READY_TOGGLE)
        {
            int index = roomView.GetIndexForPlayerId(senderId);
            if(index >= 0)
            {
                roomView.SetIsReady(index, (bool)content);
            }
        }
    }

    private void _viewInitialization()
    {
        roomView.SetTitle(PhotonNetwork.CurrentRoom.Name);
        bool isMaster = PhotonNetwork.IsMasterClient;
        roomView.IsMasterClient(isMaster);
        
        // _addButtonCallbacks(isMaster);
    }
    //
    private void _setupPlayers()
    {
        List<Player> playerList = new List<Player>(PhotonNetwork.PlayerList);
        playerList.Sort((a, b) =>
        {
            if(a == null || b == null)
            {
                return 0;
            }
            
            return a.ActorNumber.CompareTo(b.ActorNumber);
        });
    
        int count = playerList.Count;
        for(int i = 0; i < NetworkManager.kMaxPlayers; ++i)
        {
            if(i < count)
            {
                roomView.SetPlayer(i, playerList[i]);
            }
            else
            {
                roomView.SetPlayer(i, null);
            }
        }
    
        roomView.IsMasterClient(PhotonNetwork.IsMasterClient);
    }
}

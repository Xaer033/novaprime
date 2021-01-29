using GhostGen;
using Mirror;
using UnityEngine;

public class MultiplayerSetupController : BaseController 
{
    private NetworkManager _networkManager;


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
            view.AddListener(MenuUIEventType.JOIN_SERVER, onJoinServer);
            view.AddListener(MenuUIEventType.BACK, onBackButton);

            // _networkManager.onPlayerPropertiesUpdate += onPlayerPropertiesUpdate;
            // _networkManager.onPlayerConnected += onPlayerConnectionStatusChanged;
            // _networkManager.onPlayerDisconnected += onPlayerConnectionStatusChanged;
            // _networkManager.onNetworkDisconnected += onNetworkDisconnected;
            
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
    
    
    private MultiplayerSetupView roomView
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
        _networkManager.onServerStarted += onServerStarted;
        _networkManager.StartServer();
    }

    private void onCreateServerAsHost(GeneralEvent e)
    {
        _networkManager.onServerStarted += onServerStarted;
        _networkManager.StartHost();
    }
    
    private void onServerStarted()
    {
        _networkManager.onServerStarted -= onServerStarted;
        DispatchEvent(MenuUIEventType.GOTO_NETWORK_ROOM);
    }
    
    private void onJoinServer(GeneralEvent e)
    {
        string serverIpAddress = e.data as string;
        _networkManager.networkAddress = serverIpAddress;

        _networkManager.onClientConnect += onClientConnect;
        _networkManager.StartClient();
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
}

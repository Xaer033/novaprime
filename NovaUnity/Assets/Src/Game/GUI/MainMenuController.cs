using GhostGen;
using UnityEngine;

public class MainMenuController : BaseController
{
    private NetworkManager _networkManager;

    public MainMenuController()
    {
        _networkManager = Singleton.instance.networkManager;
    }
    
    public override void Start ()
    {
        viewFactory.CreateAsync<MainMenuView>("GUI/MainMenu/MainMenuView", v =>
        {
            view = v;
            view.AddListener(MenuUIEventType.PLAY, onPlay);
            view.AddListener(MenuUIEventType.GOTO_MULTIPLAYER_LOBBY, onMultiplayerPlay);
            view.AddListener(MenuUIEventType.CREDITS, onCredits);
            view.AddListener(MenuUIEventType.QUIT, onQuit);

            Singleton.instance.gui.screenFader.FadeIn();
        });
	}
    
    
    public override void RemoveView()
    {
        
        
        base.RemoveView();
        
    }

    private void onConnectedToMaster()
    {
        // if(!PhotonNetwork.OfflineMode)
        // {
        //     PhotonNetwork.JoinLobby();
        // }
    }
    
    private MainMenuView mainMenuView
    {
        get { return view as MainMenuView; }
    }
    
    private void onPlay(GeneralEvent e)
    {
        DispatchEvent(MenuUIEventType.GOTO_SINGLEPLAYER_GAME);
    }

    private void onMultiplayerPlay(GeneralEvent e)
    {
        // bool result = _networkManager.StartHost()
        // if(result)
        // {
        //     mainMenuView._canvasGroup.interactable = false;
        // }
        
        // GOTO "Host" or "join" option
        DispatchEvent(MenuUIEventType.GOTO_MULTIPLAYER_LOBBY);
    }
    
    private void onCredits(GeneralEvent e)
    {
        Debug.Log("Credits!");
    }

    private void onQuit(GeneralEvent e)
    {
        Application.Quit();
//        viewFactory.RemoveView(_mainMenuView);
    }

    private void onJoinedLobby()
    {
        mainMenuView._canvasGroup.interactable = true;
        DispatchEvent(MenuUIEventType.GOTO_MULTIPLAYER_LOBBY);
    }

}

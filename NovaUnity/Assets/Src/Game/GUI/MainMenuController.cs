using GhostGen;
using UnityEngine;

public class MainMenuController : BaseController
{
    private NetworkManager _networkManager;

    public MainMenuController()
    {
        _networkManager = Singleton.instance.networkManager;
    }
    
    public void Start ()
    {
        _setupView();
        
        _networkManager.onJoinedLobby += onJoinedLobby;
	}
    
    private void _setupView()
    {
        viewFactory.CreateAsync<MainMenuView>("GUI/MainMenu/MainMenuView", v =>
        {
            view = v;
            view.AddListener(MenuUIEventType.PLAY, onPlay);
            view.AddListener(MenuUIEventType.PLAY_MULTIPLAYER, onMultiplayerPlay);
            view.AddListener(MenuUIEventType.CREDITS, onCredits);
            view.AddListener(MenuUIEventType.QUIT, onQuit);

            Singleton.instance.gui.screenFader.FadeIn();
        });
    }
    
    private MainMenuView mainMenuView
    {
        get { return view as MainMenuView; }
    }
    
    private void onPlay(GeneralEvent e)
    {
        DispatchEvent(MenuUIEventType.CHANGE_STATE, false, NovaGameState.SINGLEPLAYER_GAMEPLAY);
    }

    private void onMultiplayerPlay(GeneralEvent e)
    {
        bool result = _networkManager.Initialize();
        if(result)
        {
            mainMenuView._canvasGroup.interactable = false;
        }
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
        DispatchEvent(MenuUIEventType.PLAY_MULTIPLAYER);
    }

    public override void RemoveView()
    {
        _networkManager.onJoinedLobby -= onJoinedLobby;
        base.RemoveView();
        
    }
}

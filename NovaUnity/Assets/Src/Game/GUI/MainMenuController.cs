using GhostGen;
using UnityEngine;

public class MainMenuController : BaseController 
{
    public void Start ()
    {
        _setupView();
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
        DispatchEvent(MenuUIEventType.PLAY_MULTIPLAYER);
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
}

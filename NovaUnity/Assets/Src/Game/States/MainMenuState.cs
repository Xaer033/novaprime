using GhostGen;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuState : IGameState
{
	private GameStateMachine _stateMachine;
	
    private MainMenuController _mainMenuController;
    private MultiplayerLobbyController _multiplayerLobbyController;
    private MultiplayerSetupController _multiplayerSetupController;
    private MultiplayerRoomController _multiplayerRoomController;

    private NetworkManager _networkManager;

	public void Init( GameStateMachine stateMachine, object changeStateData )
	{
		_stateMachine = stateMachine;

		_networkManager = Singleton.instance.networkManager;
		_networkManager.Disconnect();
		
		PlayerActions pAction = new PlayerActions();
		pAction.Menu.Enable();

		_multiplayerSetupController = new MultiplayerSetupController();
		_multiplayerSetupController.AddListener(MenuUIEventType.GOTO_NETWORK_ROOM, onMultiplayerRoomMenu);
		_multiplayerSetupController.AddListener(MenuUIEventType.GOTO_MAIN_MENU, onMainMenu);
		_multiplayerSetupController.AddListener(MenuUIEventType.GOTO_MULTIPLAYER_GAME, onStartMultiplayerGame);
		
		_mainMenuController = new MainMenuController();
		_mainMenuController.AddListener(MenuUIEventType.GOTO_MULTIPLAYER_LOBBY, onMultiplayerSetupMenu);
		_mainMenuController.AddListener(MenuUIEventType.GOTO_SINGLEPLAYER_GAME, onStartSingleplayer);
		_mainMenuController.Start();

		SceneManager.LoadScene("MenuScene");
	}
	
	public void FixedStep(float fixedDeltaTime)
	{
	    
	}
	
    public void Step( float p_deltaTime )
	{
		
    }

    public void LateStep( float p_deltaTime )
    {
		
    }

    public void Exit( )
	{
	//	_controller.getUI().rem
		_mainMenuController?.RemoveView();
		_mainMenuController = null;
		
		_multiplayerLobbyController?.RemoveView();
		_multiplayerLobbyController = null;
		
		_multiplayerRoomController?.RemoveView();
		_multiplayerRoomController = null;
		
		_multiplayerSetupController?.RemoveView();
		_multiplayerSetupController = null;

	}

	private void onCreateServer(GeneralEvent e)
	{
		
	}
		 
	private void onJoinServer(GeneralEvent e)
	{
		
	}

	private void onMultiplayerSetupMenu(GeneralEvent e)
	{
		_multiplayerLobbyController?.RemoveView();
		_multiplayerRoomController?.RemoveView();
		_mainMenuController?.RemoveView();
		_multiplayerSetupController?.Start();
	}
	
    private void onMultiplayerLobbyMenu(GeneralEvent e)
    {
	    _mainMenuController?.RemoveView();
	    _multiplayerRoomController?.RemoveView();
		_multiplayerSetupController?.RemoveView();
	    _multiplayerLobbyController?.Start();
    }
    
    private void onMultiplayerRoomMenu(GeneralEvent e)
    {
	    _multiplayerLobbyController?.RemoveView();
	    _mainMenuController?.RemoveView();
		_multiplayerSetupController?.RemoveView();

		
		_multiplayerRoomController = new MultiplayerRoomController(NetworkServer.active);
		_multiplayerRoomController.AddListener(MenuUIEventType.GOTO_MULTIPLAYER_LOBBY, onMultiplayerSetupMenu);
		_multiplayerRoomController.AddListener(MenuUIEventType.GOTO_MULTIPLAYER_GAME, onStartMultiplayerGame);
		_multiplayerRoomController.AddListener(MenuUIEventType.GOTO_MAIN_MENU, onMainMenu);
	    _multiplayerRoomController.Start();
    }

    private void onMainMenu(GeneralEvent e)
    {
	    _multiplayerLobbyController?.RemoveView();
	    _multiplayerRoomController?.RemoveView();
		_multiplayerSetupController?.RemoveView();
	    _mainMenuController?.Start();
    }
    
    private void onStartSingleplayer(GeneralEvent e)
    {
		_networkManager.Disconnect();
        _networkManager.StartSingleplayer(onSingleplayerMatchBegin);
    }


    private void onSingleplayerMatchBegin()
    {
	    _stateMachine.ChangeState(NovaGameState.SINGLEPLAYER_GAMEPLAY);
    }
    
    private void onStartMultiplayerGame(GeneralEvent e)
    {
	    _stateMachine.ChangeState(NovaGameState.MULTILAYER_GAMEPLAY);
    }
}

using GhostGen;
using Photon.Pun;
using UnityEngine;

public class MainMenuState : IGameState
{
	private GameStateMachine _stateMachine;
    private MainMenuController _mainMenuController;
    private MultiplayerLobbyController _multiplayerLobbyController;
    private MultiplayerRoomController _multiplayerRoomController;

	public void Init( GameStateMachine stateMachine, object changeStateData )
	{
		_stateMachine = stateMachine;
		
		PhotonNetwork.OfflineMode = true;
		PhotonNetwork.JoinRoom(NetworkManager.kSingleplayerRoom);
		
		PlayerActions pAction = new PlayerActions();
		pAction.Menu.Enable();

		_multiplayerLobbyController = new MultiplayerLobbyController();
		_multiplayerLobbyController.AddListener(MenuUIEventType.CREATE_SERVER, onCreateServer);
		_multiplayerLobbyController.AddListener(MenuUIEventType.JOIN_SERVER, onJoinServer);
		_multiplayerLobbyController.AddListener(MenuUIEventType.GOTO_NETWORK_ROOM, onMultiplayerRoomMenu);
		_multiplayerLobbyController.AddListener(MenuUIEventType.BACK, onMultiplayerBack);

		_multiplayerRoomController = new MultiplayerRoomController();
		_multiplayerRoomController.AddListener(MenuUIEventType.GOTO_MULTIPLAYER_LOBBY, onMultiplayerLobbyMenu);
		_multiplayerRoomController.AddListener(MenuUIEventType.GOTO_MULTIPLAYER_GAME, onStartMultiplayerGame);
		_multiplayerRoomController.AddListener(MenuUIEventType.GOTO_MAIN_MENU, onMainMenu);
		
		_mainMenuController = new MainMenuController();
		_mainMenuController.AddListener(MenuUIEventType.GOTO_MULTIPLAYER_LOBBY, onMultiplayerLobbyMenu);
		_mainMenuController.AddListener(MenuUIEventType.GOTO_SINGLEPLAYER_GAME, onStartSingleplayer);
		_mainMenuController.Start();
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
		_mainMenuController.RemoveView();
		_mainMenuController = null;
		
		_multiplayerLobbyController.RemoveView();
		_multiplayerLobbyController = null;
		
		_multiplayerRoomController.RemoveView();
		_multiplayerRoomController = null;

	}

	private void onCreateServer(GhostGen.GeneralEvent e)
	{
		
	}
		 
	private void onJoinServer(GhostGen.GeneralEvent e)
	{
		
	}

	private void onMultiplayerBack(GhostGen.GeneralEvent e)
	{
		_multiplayerLobbyController.RemoveView();
		_multiplayerRoomController.RemoveView();
		_mainMenuController.Start();
	}
	
    private void onMultiplayerLobbyMenu(GhostGen.GeneralEvent e)
    {
	    _mainMenuController.RemoveView();
	    _multiplayerRoomController.RemoveView();
	    _multiplayerLobbyController.Start();
    }
    
    private void onMultiplayerRoomMenu(GhostGen.GeneralEvent e)
    {
	    _mainMenuController.RemoveView();
	    _multiplayerLobbyController.RemoveView();
	    _multiplayerRoomController.Start();
    }

    private void onMainMenu(GhostGen.GeneralEvent e)
    {
	    _multiplayerLobbyController.RemoveView();
	    _multiplayerRoomController.RemoveView();
	    _mainMenuController.Start();
    }
    
    private void onStartSingleplayer(GhostGen.GeneralEvent e)
    {
	    _stateMachine.ChangeState(NovaGameState.SINGLEPLAYER_GAMEPLAY);
    }
    
    private void onStartMultiplayerGame(GhostGen.GeneralEvent e)
    {
	    _stateMachine.ChangeState(NovaGameState.MULTILAYER_GAMEPLAY);
    }
}

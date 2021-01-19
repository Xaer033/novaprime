using GhostGen;

public class MainMenuState : IGameState
{
	private GameStateMachine _stateMachine;
    private MainMenuController _mainMenuController;
    private MultiplayerLobbyController _multiplayerLobbyController;

	public void Init( GameStateMachine stateMachine, object changeStateData )
	{
		_stateMachine = stateMachine;
		
		PlayerActions pAction = new PlayerActions();
		pAction.Menu.Enable();

		_multiplayerLobbyController = new MultiplayerLobbyController();
		_multiplayerLobbyController.AddListener(MenuUIEventType.CREATE_SERVER, onCreateServer);
		_multiplayerLobbyController.AddListener(MenuUIEventType.JOIN_SERVER, onJoinServer);
		_multiplayerLobbyController.AddListener(MenuUIEventType.BACK, onMultiplayerBack);
		
		_mainMenuController = new MainMenuController();
		_mainMenuController.AddListener(MenuUIEventType.PLAY_MULTIPLAYER, onMultiplayerMenu);
		_mainMenuController.AddListener(MenuUIEventType.CHANGE_STATE, onChangeState);
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
		_mainMenuController.Start();
	}
	
    private void onMultiplayerMenu(GhostGen.GeneralEvent e)
    {
	    _mainMenuController.RemoveView();
	    _multiplayerLobbyController.Start();
    }
    
    private void onChangeState(GhostGen.GeneralEvent e)
    {
	    string newState = e.data as string; 
	    _stateMachine.ChangeState(newState);
    }
}

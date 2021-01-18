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
		
		_mainMenuController = new MainMenuController();
		_mainMenuController.AddListener(MenuUIEventType.CHANGE_STATE, onChangeState);
		_mainMenuController.Start(stateMachine);
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
		
	}

    private void onChangeState(GhostGen.GeneralEvent e)
    {
	    string newState = e.data as string; 
	    _stateMachine.ChangeState(newState);
    }
}

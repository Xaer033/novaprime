using GhostGen;

public class MainMenuState : IGameState
{
    private MainMenuController _mainMenuController;

	public void Init( GameStateMachine stateMachine, object changeStateData )
	{
		PlayerActions pAction = new PlayerActions();
		pAction.Menu.Enable();
		
		_mainMenuController = new MainMenuController();
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
	}
    
}

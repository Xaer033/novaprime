using GhostGen;
using System.Collections;
using Photon.Pun.UtilityScripts;
using UnityEngine;

public class MainMenuState : IGameState
{
    private MainMenuController _mainMenuController;

	public void Init( GameStateMachine stateMachine, object changeStateData )
	{
        _mainMenuController = new MainMenuController();
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

	}
    
}

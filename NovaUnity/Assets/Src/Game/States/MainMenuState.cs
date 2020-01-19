using GhostGen;
using System.Collections;
using Photon.Pun.UtilityScripts;
using UnityEngine;

public class MainMenuState : IGameState
{
    private MainMenuController _mainMenuController;

	public void Init( GameStateMachine stateMachine, object changeStateData )
	{
		Debug.Log ("Entering In MainMenu State");
        _mainMenuController = new MainMenuController();
        _mainMenuController.Start();
	}
    
    public void Step( float p_deltaTime )
	{
		
    }

    public void Exit( )
	{
	//	_controller.getUI().rem
		Debug.Log ("Exiting In MainMenu State");

	}
    
}

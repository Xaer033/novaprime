using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using GhostGen;
using DG.Tweening;

public class IntroState : IGameState
{
    private GameStateMachine _stateMachine;
    private bool _gotoSplash = false;

    public void Init( GameStateMachine stateMachine, object changeStateData )
	{
        DOTween.Init(true, true, LogBehaviour.ErrorsOnly);

        _gotoSplash = true;
        _stateMachine = stateMachine;
        // Singleton.instance.networkManager.onNetworkStart += onNetworkStart;
        // Singleton.instance.networkManager.CreateSession("Help", null);
        // Singleton.instance.networkManager.StartSinglePlayer();
        
    }

    public void FixedStep(float fixedDeltaTime)
    {
	    
    }
    
    
    public void Step( float p_deltaTime )
	{
		if (_gotoSplash) 
		{
			_stateMachine.ChangeState (NovaGameState.MAIN_MENU);
			_gotoSplash = false;
		}

        //Transform camTransform = Camera.main.transform;
        //Vector3 lookPos = camTransform.position + camTransform.forward * 10.0f;

        //Vector3 grav = -Input.gyro.gravity;
        //grav.x = -grav.x;
        //Camera.main.transform.LookAt(lookPos, grav.normalized);
    }

    public void LateStep( float p_deltaTime )
    {
		
    }

    public void Exit( )
	{
		
		// Singleton.instance.networkManager.onNetworkStart -= onNetworkStart;
	//	_controller.getUI().rem
		//_backButton.onClick.RemoveAllListeners ();
	}

    private void onNetworkStart()
    {
	    _gotoSplash = true;
    }
}

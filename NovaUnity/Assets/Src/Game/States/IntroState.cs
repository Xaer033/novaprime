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
		Debug.Log ("Entering In Intro State");
        DOTween.Init(true, true, LogBehaviour.ErrorsOnly);

        _stateMachine = stateMachine;
        _gotoSplash = true;
        
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

    public void Exit( )
	{
	//	_controller.getUI().rem
		Debug.Log ("Exiting In Intro State");
		//_backButton.onClick.RemoveAllListeners ();
	}
}

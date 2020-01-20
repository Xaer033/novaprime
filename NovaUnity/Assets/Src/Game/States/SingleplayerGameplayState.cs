using DG.Tweening;
using GhostGen;
using System.Collections;
using UnityEngine;

public class SingleplayerGameplayState : IGameState
{
    private IGameModeController _gameModeController;
    private NotificationDispatcher _gameModeDispatcher;

    private GameStateMachine    _stateMachine;

    public void Init( GameStateMachine stateMachine, object changeStateData )
	{       
        Debug.Log ("Entering In GamePlay State");

		_stateMachine = stateMachine;

        // *TEMP*
        Singleton.instance.gui.screenFader.alpha = 0.0f;


        _gameModeController = new SinglePlayerCampaignMode();
        _gameModeController.AddListener(GameEventType.GAME_OVER, onGameOver);
        _gameModeController.Start(null);
    }

    
    public void FixedStep(float fixedDeltaTime)
    {
	    _gameModeController.FixedStep(fixedDeltaTime);    
    }
    
    public void Step( float p_deltaTime )
	{
        _gameModeController.Step(p_deltaTime);
    }

    public void Exit()
	{
		Debug.Log ("Exiting In Gameplay State");

        _gameModeController.RemoveListener(GameEventType.GAME_OVER, onGameOver);
        _gameModeController.CleanUp();   
    }

    
    private void onGameOver(GeneralEvent e)
    {
        _stateMachine.ChangeState(NovaGameState.MAIN_MENU); ;
    }

}

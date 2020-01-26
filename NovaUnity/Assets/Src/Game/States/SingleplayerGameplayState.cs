using DG.Tweening;
using GhostGen;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        string name = SceneManager.GetActiveScene().name;
        if (Application.isEditor || name != "GameplayScene")
        {
            onSceneUnloaded(null);
        }
        else
        {
            AsyncOperation async = SceneManager.UnloadSceneAsync("GameplayScene");
            async.completed += onSceneUnloaded;
        }
    }

    private void onSceneUnloaded(AsyncOperation asyncOp)
    {
        string name = SceneManager.GetActiveScene().name;
        if (Application.isEditor && name == "GameplayScene")
        {
            onSceneLoaded(null);
        }
        else
        {
            AsyncOperation async = SceneManager.LoadSceneAsync("GameplayScene", LoadSceneMode.Single);
            async.completed += onSceneLoaded;
        }

    }

    private void onSceneLoaded(AsyncOperation asyncOp)
    {        
        _gameModeController = new SinglePlayerCampaignMode();
        _gameModeController.AddListener(GameEventType.GAME_OVER, onGameOver);
        _gameModeController.Start(null);
    }
    
    public void FixedStep(float fixedDeltaTime)
    {
        if (_gameModeController != null)
        {
	        _gameModeController.FixedStep(fixedDeltaTime);
        }
    }
    
    public void Step( float p_deltaTime )
	{
        if (_gameModeController != null)
        {
            _gameModeController.Step(p_deltaTime);
        }
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

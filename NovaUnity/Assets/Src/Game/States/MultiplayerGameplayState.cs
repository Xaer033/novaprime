using System.Collections;
using System.Collections.Generic;
using GhostGen;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MultiplayerGameplayState : IGameState
{
    private IGameModeController _gameModeController;
    private NotificationDispatcher _gameModeDispatcher;

    private GameStateMachine    _stateMachine;

    public void Init( GameStateMachine stateMachine, object changeStateData )
	{       
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
            ;
                
            AsyncOperation async = SceneManager.LoadSceneAsync("GameplayScene", LoadSceneMode.Single);
            async.completed += onSceneLoaded;
        }

    }

    private void onSceneLoaded(AsyncOperation asyncOp)
    {        
        _gameModeController = new MultiplayerCampaignMode();
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

    public void LateStep(float deltaTime)
    {
        if (_gameModeController != null)
        {
            _gameModeController.LateStep(deltaTime);
        }
    }
    
    public void Exit()
	{
        _gameModeController.RemoveListener(GameEventType.GAME_OVER, onGameOver);
        _gameModeController.CleanUp();   
    }

    
    private void onGameOver(GeneralEvent e)
    {
        _stateMachine.ChangeState(NovaGameState.MAIN_MENU); ;
    }
}

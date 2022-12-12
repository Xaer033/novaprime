using DG.Tweening;
using GhostGen;
using System.Collections;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SingleplayerGameplayState : IGameState
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
            AsyncOperation async = SceneManager.LoadSceneAsync("GameplayScene", LoadSceneMode.Single);
            async.completed += onSceneLoaded;
        }
    }

    private void onSceneLoaded(AsyncOperation asyncOp)
    {        
        if(NetworkServer.active)
        {
            NetworkServer.SpawnObjects();            
        }
        
        startGameSystems();

        if(NetworkClient.active)
        {
            NetworkClient.Send(new PlayerMatchLoadComplete(), Channels.Reliable);
        }
    }

    private void startGameSystems()
    {
        _gameModeController = new SinglePlayerCampaignMode();
        _gameModeController?.AddListener(GameEventType.GAME_OVER, onGameOver);
        _gameModeController?.Start(null);
    }
    
    public void FixedStep(float fixedDeltaTime)
    {
	    _gameModeController?.FixedStep(fixedDeltaTime);
    }
    
    public void Step( float p_deltaTime )
	{
        _gameModeController?.Step(p_deltaTime);   
    }

    public void LateStep(float deltaTime)
    {
            _gameModeController?.LateStep(deltaTime);
    }
    
    public void Exit()
	{
        _gameModeController?.RemoveListener(GameEventType.GAME_OVER, onGameOver);
        _gameModeController?.CleanUp();   
    }
    
    private void onGameOver(GeneralEvent e)
    {
        _stateMachine.ChangeState(NovaGameState.MAIN_MENU); ;
    }
}

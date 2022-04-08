using Fusion;
using GhostGen;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SingleplayerGameplayState : IGameState
{
    private IGameModeController _gameModeController;
    private NotificationDispatcher _gameModeDispatcher;

    private GameStateMachine    _stateMachine;

    private NetworkManager _networkManager;

    public void Init(GameStateMachine stateMachine, object changeStateData)
	{       
		_stateMachine = stateMachine;

        // *TEMP*
        Singleton.instance.gui.screenFader.alpha = 0.0f;

        _networkManager = Singleton.instance.networkManager;
        _networkManager.onSceneLoadDone += onSceneLoaded;
        
        // Scene scene = SceneManager.GetSceneByBuildIndex(0)
        _networkManager.runner.SetActiveScene(2);
    }

    private void onSceneLoaded(NetworkRunner runner)
    {        
        startGameSystems();
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

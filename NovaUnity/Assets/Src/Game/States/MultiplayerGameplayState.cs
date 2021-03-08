using System.Collections.Generic;
using GhostGen;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MultiplayerGameplayState : IGameState
{
    const string kIsLoadedKey = "isGameplayLoaded";
    
    private IGameModeController _gameModeController;
    private NotificationDispatcher _gameModeDispatcher;
    private NetworkManager _networkManager;
    
    private GameStateMachine    _stateMachine;
    private HashSet<int> _playersFinishedLoading = new HashSet<int>();

    public void Init( GameStateMachine stateMachine, object changeStateData )
	{       
		_stateMachine = stateMachine;
        Singleton.instance.gui.screenFader.alpha = 0.0f;

        _playersFinishedLoading.Clear();

        if(NetworkServer.active)
        {
            StartMatchLoad startMatchMessage = new StartMatchLoad();
            NetworkServer.SendToAll(startMatchMessage, Channels.DefaultReliable);
        }
        
        _networkManager = Singleton.instance.networkManager;
        _networkManager.onClientMatchBegin += onClientMatchBegin;
        
        // PhotonNetwork.IsMessageQueueRunning = false;
        AsyncOperation async = SceneManager.LoadSceneAsync("GameplayScene", LoadSceneMode.Single);
        async.completed += onSceneLoaded;
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
            Debug.Log("SceneLoader: " + _networkManager.localPlayerSlot);
            
            ClientScene.Ready(NetworkClient.connection);
            NetworkClient.Send(new PlayerMatchLoadComplete(), Channels.DefaultReliable);
        }
    }
    
    public void FixedStep(float fixedDeltaTime)
    {
        
	    _gameModeController?.FixedStep(fixedDeltaTime);
    }
    
    public void Step( float deltaTime )
	{
        
        _gameModeController?.Step(deltaTime);
    
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


    private void onClientMatchBegin(NetworkConnection conn, MatchBegin msg)
    {
        _networkManager.onClientMatchBegin -= onClientMatchBegin;
        // handleAllPlayersLoaded();
        // Un freeze players here or something
    }

    private void startGameSystems()
    {
        _gameModeController = new MultiplayerCampaignMode();
        _gameModeController?.AddListener(GameEventType.GAME_OVER, onGameOver);
        _gameModeController?.Start(null);
    }
    
    private void onGameOver(GeneralEvent e)
    {
        // Maybe respawn!
        
        // _stateMachine.ChangeState(NovaGameState.MAIN_MENU); ;
        
    }
}

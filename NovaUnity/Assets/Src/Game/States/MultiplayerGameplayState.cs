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

        _playersFinishedLoading.Clear();

        // *TEMP*
        Singleton.instance.gui.screenFader.alpha = 0.0f;
        
        _networkManager = Singleton.instance.networkManager;
        _networkManager.onClientMatchBegin += onClientMatchBegin;
        
        // PhotonNetwork.IsMessageQueueRunning = false;
        AsyncOperation async = SceneManager.LoadSceneAsync("GameplayScene", LoadSceneMode.Single);
        async.completed += onSceneLoaded;
    }

    private void onSceneLoaded(AsyncOperation asyncOp)
    {
        Debug.Log("SceneLoader: " + _networkManager.localPlayerSlot);
        NetworkClient.Send(new PlayerMatchLoadComplete(), Channels.DefaultReliable);
    }
    
    public void FixedStep(float fixedDeltaTime)
    {
        if (_gameModeController != null)
        {
	        _gameModeController.FixedStep(fixedDeltaTime);
        }
    }
    
    public void Step( float deltaTime )
	{
        if (_gameModeController != null)
        {
            _gameModeController.Step(deltaTime);
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


    private void onClientMatchBegin(NetworkConnection conn, MatchBegin msg)
    {
        _networkManager.onClientMatchBegin -= onClientMatchBegin;
        handleAllPlayersLoaded();
    }

    private void handleAllPlayersLoaded()
    {
        List<NetPlayer> playerList = new List<NetPlayer>(4);
        var netPlayerMap = _networkManager.GetClientPlayerMap();
        playerList.AddRange(netPlayerMap.Values);
        
        playerList.Sort((a, b) =>
        {
            return a.playerSlot.CompareTo(b.playerSlot);
        });
        
        _gameModeController = new MultiplayerCampaignMode();
        _gameModeController.AddListener(GameEventType.GAME_OVER, onGameOver);
        _gameModeController.Start(playerList);
        
        ClientScene.Ready(NetworkClient.connection);
    }
    
    private void onGameOver(GeneralEvent e)
    {
        // Maybe respawn!
        
        // _stateMachine.ChangeState(NovaGameState.MAIN_MENU); ;
        
    }
}

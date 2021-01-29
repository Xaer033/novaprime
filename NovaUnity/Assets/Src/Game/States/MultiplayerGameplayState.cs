using System.Collections.Generic;
using GhostGen;
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
        
        // PhotonNetwork.IsMessageQueueRunning = false;
        AsyncOperation async = SceneManager.LoadSceneAsync("GameplayScene", LoadSceneMode.Single);
        async.completed += onSceneLoaded;
    }

    private void onSceneLoaded(AsyncOperation asyncOp)
    {
        // PhotonNetwork.IsMessageQueueRunning = true;
        //
        // RaiseEventOptions options = new RaiseEventOptions();
        // options.Receivers = ReceiverGroup.MasterClient;
        //
        // Hashtable currentProperties = PhotonNetwork.LocalPlayer.CustomProperties;
        // Hashtable newProperties = currentProperties;
        // newProperties[kIsLoadedKey] = true;
        // PhotonNetwork.LocalPlayer.SetCustomProperties(newProperties);
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


    private void onCustomEvent(byte netOpCode, object eventContent, int sender)
    {
        switch(netOpCode)
        {
            case NetworkOpCode.ALL_PLAYERS_LOADED:    handleAllPlayersLoaded();   break; 
        }
    }

    // private void onPlayerPropertiesUpdate(Player targetPlayer, Hashtable updatedProperties)
    // {
    //     if(!PhotonNetwork.IsMasterClient) { return; }
    //     
    //     bool isFinishedLoading = (bool)updatedProperties[kIsLoadedKey];
    //     if(isFinishedLoading)
    //     {
    //         _playersFinishedLoading.Add(targetPlayer.ActorNumber);
    //         if(_playersFinishedLoading.Count >= PhotonNetwork.CurrentRoom.PlayerCount)
    //         {
    //             RaiseEventOptions options = new RaiseEventOptions();
    //             options.Receivers = ReceiverGroup.All;
    //             PhotonNetwork.RaiseEvent(NetworkOpCode.ALL_PLAYERS_LOADED, null, options, SendOptions.SendReliable);
    //         }
    //     }
    // }

    private void handleAllPlayersLoaded()
    {
        List<NetPlayer> playerList = new List<NetPlayer>(4);
        // for(int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; ++i)
        // {
        //     Player photonPlayer = PhotonNetwork.PlayerList[i];
        //     NetPlayer netPlayer = new NetPlayer(photonPlayer);
        //     
        //     playerList.Add(netPlayer);
        // }
        //
        playerList.Sort((a, b) =>
        {
            if(a == null || b == null) { return 0; }
            return a.id.CompareTo(b.id);
        });
        
        _gameModeController = new MultiplayerCampaignMode();
        _gameModeController.AddListener(GameEventType.GAME_OVER, onGameOver);
        _gameModeController.Start(playerList);
    }
    
    private void onGameOver(GeneralEvent e)
    {
        // Maybe respawn!
        
        // _stateMachine.ChangeState(NovaGameState.MAIN_MENU); ;
        
    }
}

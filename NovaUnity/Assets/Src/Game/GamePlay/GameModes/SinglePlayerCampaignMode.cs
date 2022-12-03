using System.Collections.Generic;
using GhostGen;

public class SinglePlayerCampaignMode : NotificationDispatcher, IGameModeController
{
    private PlayFieldController _playFieldController;
    
    public void Start(object context)
    {
        GameplayResources gameplayResources = Singleton.instance.gameplayResources;

        NetPlayer mockNetPlayer = new NetPlayer
        {
            playerSlot = PlayerSlot.P1,
            nickName   = PlayerSlot.P1.ToString()
        };
        
        List<NetPlayer> playerList = new List<NetPlayer>(4);
        playerList.Add(mockNetPlayer);

        _playFieldController = new PlayFieldController(gameplayResources);
        _playFieldController.Start();
    }

    public void FixedStep(float fixedDeltaTime)
    {
        _playFieldController?.FixedStep(fixedDeltaTime); 
    }
    
    public void Step(float deltaTime)
    {
        _playFieldController?.Step(deltaTime);
    }

    public void LateStep(float deltaTime)
    {
        _playFieldController?.LateStep(deltaTime);
    }
    
    public void CleanUp()
    {
        _playFieldController?.CleanUp();
    }
}

using System.Collections.Generic;
using GhostGen;

public class SinglePlayerCampaignMode : NotificationDispatcher, IGameModeController
{
    private PlayFieldController _playFieldController;
    
    public void Start(object context)
    {
        GameplayResources gameplayResources = Singleton.instance.gameplayResources;
        NetPlayer mockNetPlayer = new NetPlayer(-1, PlayerSlot.P1, PlayerSlot.P1.ToString());//PhotonNetwork.LocalPlayer);
        List<NetPlayer> playerList = new List<NetPlayer>(4);
        playerList.Add(mockNetPlayer);

        _playFieldController = new PlayFieldController(gameplayResources);
        _playFieldController.Start();
    }

    public void FixedStep(float fixedDeltaTime)
    {
        if (_playFieldController != null)
        {
            _playFieldController.FixedStep(fixedDeltaTime);
        }
    }
    
    public void Step(float deltaTime)
    {
        if (_playFieldController != null)
        {
            _playFieldController.Step(deltaTime);
        }
    }

    public void LateStep(float deltaTime)
    {
        if (_playFieldController != null)
        {
            _playFieldController.LateStep(deltaTime);
        } 
    }
    
    public void CleanUp()
    {
        if (_playFieldController != null)
        {
            _playFieldController.CleanUp();
        }
    }
}

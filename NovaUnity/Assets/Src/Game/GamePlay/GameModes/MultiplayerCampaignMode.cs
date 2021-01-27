using System.Collections.Generic;
using GhostGen;

public class MultiplayerCampaignMode : NotificationDispatcher, IGameModeController
{
    //private PassAndPlayFieldController  _playFieldController        = new PassAndPlayFieldController();
    //private GameOverPopupController     _gameOverPopupController    = new GameOverPopupController();
//    private TuckMatchCore _tuckMatchCore;
//    private PlayFieldController _playFieldController = new PlayFieldController();
    private List<NetPlayer> _playerList = new List<NetPlayer>(4);
    private PlayFieldController _playFieldController;
    
    public void Start(object context)
    {
        GameplayResources gameplayResources = Singleton.instance.gameplayResources;
        _playerList = context as List<NetPlayer>;
        bool isServer = true; // TODO: Put real value here
        
        _playFieldController = new PlayFieldController(_playerList, isServer, gameplayResources);
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

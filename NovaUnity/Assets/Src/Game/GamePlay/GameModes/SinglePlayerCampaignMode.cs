using GhostGen;

public class SinglePlayerCampaignMode : NotificationDispatcher, IGameModeController
{   
    //private PassAndPlayFieldController  _playFieldController        = new PassAndPlayFieldController();
    //private GameOverPopupController     _gameOverPopupController    = new GameOverPopupController();
//    private TuckMatchCore _tuckMatchCore;
//    private PlayFieldController _playFieldController = new PlayFieldController();
//    private List<PlayerState> _playerList = new List<PlayerState>(PlayerGroup.kMaxPlayerCount);
    private PlayFieldController _playFieldController;
    
    public void Start(object context)
    {
        GameplayResources gameplayResources = Singleton.instance.gameplayResources;
        
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

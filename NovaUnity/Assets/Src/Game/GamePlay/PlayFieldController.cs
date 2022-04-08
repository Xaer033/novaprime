using System.Collections.Generic;
using GhostGen;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayFieldController : NotificationDispatcher
{
    private GameplayResources _gameplayResources;
    private GameplayCamera    _gameplayCamera;
    private GameState         _gameState;
    private GameSystems       _gameSystems;
    private PlayerActions     _pAction;

    private IInputGenerator  _playerInput;
    private IInputGenerator  _gruntInput;
    private PlayerController _playerController;
    private GruntController  _gruntController;

    private GameplayCamera _cam;

    private List<NetPlayer> _playerList;
    private bool            _isServer;

    public PlayFieldController(GameplayResources gameplayResources)
    {
        _gameplayResources = gameplayResources;
    }
    public void Start()
    {
        Random.InitState(666);

        _gameState   = new GameState();
        _gameSystems = new GameSystems(_gameState, _gameplayResources, NetworkServer.active);
        _gameSystems.Start();

//        AvatarSystem aSystem = _gameSystems.GetSystem<AvatarSystem>();
//        _playerController = aSystem.Spawn<PlayerController>( "player", Vector3.up * 2.0f);
//        _playerInput = _playerController.GetInput();
//        
//        aSystem.Spawn<GruntController>( "grunt", Vector3.right * 2 + Vector3.up * 2.0f);
//        _gruntController = aSystem.Spawn<GruntController>( "grunt", Vector3.left * 4.0f + Vector3.up * 2.0f);
//        _gruntInput = _gruntController.GetInput();

        _pAction = new PlayerActions();
        _pAction.Gameplay.Enable();

        _cam = GameObject.FindObjectOfType<GameplayCamera>();

        _addCallbacks();
    }

    public void Restart()
    {
        _pAction.Gameplay.Disable();
        ScreenFader fader = Singleton.instance.gui.screenFader;
        fader.FadeInOut(0.35f, 0.35f, () =>
        {
            CleanUp();
            Start();

            // NetworkClient.Send(new PlayerMatchLoadComplete(), Channels.DefaultReliable);
        }, () =>
        {
            _pAction.Gameplay.Enable();
        });
    }

    public void Step(float deltaTime)
    {
        if (_gameSystems != null)
        {
            _gameSystems.Step(deltaTime);
        }

        if (_pAction.Gameplay.reset.triggered)
        {
            Restart();
        }
    }

    public void FixedStep(float fixedDeltaTime)
    {
        if (_gameSystems != null)
        {
            _gameSystems.FixedStep(fixedDeltaTime);
        }
    }

    public void LateStep(float deltaTime)
    {
        if (_gameSystems != null)
        {
            _gameSystems.LateStep(deltaTime);
        }



        //Debug STUFF
        if (Keyboard.current.f2Key.wasPressedThisFrame)
        {
            _cam.ClearTargets();

            IInputGenerator currentInput = _gruntController.input;
            if (currentInput == _gruntInput)
            {
                _playerController.input = _gruntInput;
                _gruntController.input  = _playerInput;
                _cam.AddTarget(_gruntController.view.cameraTarget);
            }
            else
            {
                _playerController.input = _playerInput;
                _gruntController.input  = _gruntInput;
                _cam.AddTarget(_playerController.view.cameraTarget);
            }

        }

        if (_pAction.Gameplay.exit.triggered)
        {
            Singleton.instance.gameStateMachine.ChangeState(NovaGameState.MAIN_MENU);
        }
    }

    public void CleanUp()
    {
        if (_gameSystems != null)
        {
            _gameSystems.CleanUp();
        }

        _gruntController = null;
        _gruntInput      = null;

        _playerController = null;
        _playerInput      = null;

        _removeCallbacks();

        RemoveAllListeners();
    }

    private void onGameOver(bool gameOverPopup = true)
    {
        if (!gameOverPopup)
        {
            Singleton.instance.gui.screenFader.FadeOut(0.5f, () =>
            {
                DispatchEvent(GameEventType.GAME_OVER);
            });
        }
        else
        {
            //MatchOverEvent matchOver = MatchOverEvent.Create(_playerList);
            //_gameOverPopupController.Start(matchOver.playerRanking, () =>
            //{
            DispatchEvent(GameEventType.GAME_OVER);
            //});
        }
    }

    private void _addCallbacks()
    {
        _gameSystems.AddListener(GamePlayEventType.AVATAR_SPAWNED, onAvatarSpawned);
    }

    private void _removeCallbacks()
    {
        _gameSystems.RemoveListener(GamePlayEventType.AVATAR_SPAWNED, onAvatarSpawned);
    }

    private void onAvatarSpawned(GeneralEvent e)
    {
        IAvatarController controller = (IAvatarController)e.data;
        string            id         = controller.unit.id;

        if (id == "player")
        {
            _playerController = controller as PlayerController;
            _playerInput      = _playerController.input;
        }
        else if (id == "grunt" && _gruntInput == null)
        {
            _gruntController = controller as GruntController;
            _gruntInput      = _gruntController.input;
        }
    }
}

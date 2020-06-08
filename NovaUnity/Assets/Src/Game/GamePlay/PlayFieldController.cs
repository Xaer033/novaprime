using System.Collections;
using System.Collections.Generic;
using GhostGen;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayFieldController : NotificationDispatcher
{
    private GameplayCamera _gameplayCamera;
    private GameState _gameState;
    private GameSystems _gameSystems;
    private PlayerActions _pAction;

    private IInputGenerator _playerInput;
    private IInputGenerator _gruntInput;
    private PlayerController _playerController;
    private GruntController _gruntController;

    private GameplayCamera _cam;
    
    public void Start()
    {
        UnityEngine.Random.InitState(666);

        _gameState = new GameState();
        _gameSystems = new GameSystems(_gameState);
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
        CleanUp();
        Start();
    }
    
    public void Step(float deltaTime)
    {
        if (_gameSystems != null)
        {
            _gameSystems.Step(deltaTime);
        }
    }

    public void FixedStep(float fixedDeltaTime)
    {
        if (_gameSystems != null)
        {
            _gameSystems.FixedStep(fixedDeltaTime);
        }
        
        //Debug STUFF
        if (Keyboard.current.f2Key.wasPressedThisFrame)
        {
            _cam.ClearTargets();
            
            IInputGenerator currentInput = _gruntController.GetInput();
            if (currentInput == _gruntInput)
            {
                _playerController.SetInput(_gruntInput);
                _gruntController.SetInput(_playerInput);
                _cam.AddTarget(_gruntController.GetView()._viewRoot);
            }
            else
            {
                _playerController.SetInput(_playerInput);
                _gruntController.SetInput(_gruntInput);
                _cam.AddTarget(_playerController.GetView()._viewRoot);
            }
            
        }

        if (_pAction.Gameplay.exit.triggered)
        {
            Application.Quit();
        }
        
    }

    public void LateStep(float deltaTime)
    {
        if (_gameSystems != null)
        {
            _gameSystems.LateStep(deltaTime);
        }

        if (_pAction.Gameplay.reset.triggered)
        {
            Restart();
        }
    }
    
    public void CleanUp()
    {
        if (_gameSystems != null)
        {
            _gameSystems.CleanUp();
        }

        _gruntController = null;
        _gruntInput = null;

        _playerController = null;
        _playerInput = null;

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
    
    private void   _addCallbacks()
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
        string id = controller.GetUnit().id;
        
        if (id == "player")
        {
            _playerController = controller as PlayerController;
            _playerInput = _playerController.GetInput();
        }
        else if (id == "grunt" && _gruntInput == null)
        {
            _gruntController = controller as GruntController;
            _gruntInput = _gruntController.GetInput();
        }
    }
}

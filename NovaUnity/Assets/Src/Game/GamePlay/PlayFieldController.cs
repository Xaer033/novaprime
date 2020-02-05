using System.Collections;
using System.Collections.Generic;
using GhostGen;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayFieldController : NotificationDispatcher
{
    private PlayerController _pController;
    private GameplayCamera _gameplayCamera;
    private GameState _gameState;
    private GameSystems _gameSystems;
    private PlayerActions _pAction;
    
    public void Start()
    {
        _addCallbacks();
        
        UnityEngine.Random.InitState(666);

        _gameState = new GameState();
        _gameSystems = new GameSystems(_gameState);
        
        _gameSystems.Start();
        
        PlayerState p1State = PlayerState.Create(Vector3.up * 2.0f);
        _gameState.playerStateList.Add(p1State);
        
        PlayerAvatarView p1View = GameObject.Instantiate<PlayerAvatarView>(Singleton.instance.gameplayResources.playerAvatar);
        
        _gameplayCamera = GameObject.FindObjectOfType<GameplayCamera>();
        if (_gameplayCamera == null)
        {
            _gameplayCamera = GameObject.Instantiate<GameplayCamera>(Singleton.instance.gameplayResources.gameplayCamera);
        }
        _gameplayCamera.target = p1View;
        
        _pAction = new PlayerActions();
        _pAction.Gameplay.Enable();
        
        PlayerInput p1Input = new PlayerInput(0, _gameplayCamera);
        
        _pController = new PlayerController(p1State, p1View, p1Input);
        _pController.Start(_gameSystems);
        
    }

    public void Step(float deltaTime)
    {
        if (_pController != null)
        {
            _pController.Step(deltaTime);
            
           
        }

        if (_gameSystems != null)
        {
            _gameSystems.Step(deltaTime);
        }
    }

    public void FixedStep(float fixedDeltaTime)
    {
        if (_pController != null)
        {
            _pController.FixedStep(fixedDeltaTime);
        }

        if (_gameSystems != null)
        {
            _gameSystems.FixedStep(fixedDeltaTime);
        }
        
        //Debug STUFF
        if (_pAction.Gameplay.reset.triggered)
        {
            _gameState.playerStateList[0].position = Vector3.zero;
        }

        if (_pAction.Gameplay.exit.triggered)
        {
            Application.Quit();
        }
    }

    public void CleanUp()
    {
        
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
  
    }

    private void _removeCallbacks()
    {

    }

}

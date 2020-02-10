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
    
    public void Start()
    {
        _addCallbacks();
        
        UnityEngine.Random.InitState(666);

        _gameState = new GameState();
        _gameSystems = new GameSystems(_gameState);
        _gameSystems.Start();
        
        AvatarSystem aSystem = _gameSystems.GetSystem<AvatarSystem>();
        PlayerController pController = aSystem.Spawn<PlayerController>( "player", Vector3.up * 2.0f);
        pController.playerNumber = 0;

        
        
        IAvatarController gruntController = aSystem.Spawn<IAvatarController>( "grunt", Vector3.right * 2 + Vector3.up * 2.0f);

        
        _pAction = new PlayerActions();
        _pAction.Gameplay.Enable();
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
    
    private void   _addCallbacks()
    {
  
    }

    private void _removeCallbacks()
    {

    }

}

﻿using System.Collections;
using System.Collections.Generic;
using GhostGen;
using UnityEngine;

public class PlayFieldController : NotificationDispatcher
{
    private PlayerController _pController;
    private GameplayCamera _gameplayCamera;
    
    public void Start()
    {
        _addCallbacks();

        PlayerAvatarView playerView = GameObject.Instantiate<PlayerAvatarView>(Singleton.instance.gameplayResources.playerAvatar);
        
        _pController = new PlayerController();
        _pController.Start(playerView);

        _gameplayCamera = GameObject.FindObjectOfType<GameplayCamera>();
        if (_gameplayCamera == null)
        {
            _gameplayCamera = GameObject.Instantiate<GameplayCamera>(Singleton.instance.gameplayResources.gameplayCamera);
        }
    }

    public void Step(float deltaTime)
    {
        if (_pController != null)
        {
            FrameInput p1Input = PlayerInput.GetInput(0);
            _pController.Step(deltaTime, p1Input);

            if (_gameplayCamera != null)
            {
                _gameplayCamera.target = _pController.position;
            }
        }
    }

    public void FixedStep(float fixedDeltaTime)
    {
        if (_pController != null)
        {
            _pController.FixedStep(fixedDeltaTime);
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

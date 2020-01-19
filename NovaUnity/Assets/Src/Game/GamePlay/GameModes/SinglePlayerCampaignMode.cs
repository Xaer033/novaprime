using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GhostGen;

public class SinglePlayerCampaignMode : NotificationDispatcher, IGameModeController
{   
    //private PassAndPlayFieldController  _playFieldController        = new PassAndPlayFieldController();
    //private GameOverPopupController     _gameOverPopupController    = new GameOverPopupController();
//    private TuckMatchCore _tuckMatchCore;
//    private PlayFieldController _playFieldController = new PlayFieldController();
//    private List<PlayerState> _playerList = new List<PlayerState>(PlayerGroup.kMaxPlayerCount);

    private PlayerController _pController;
    
    public void Start(object context)
    {
        _addCallbacks();

        PlayerAvatarView playerView = GameObject.Instantiate<PlayerAvatarView>(Singleton.instance.gameplayResources.playerAvatar);
        _pController = new PlayerController();
        _pController.Start(playerView);
    }

    public void FixedStep(float fixedDeltaTime)
    {
        if (_pController != null)
        {
            _pController.FixedStep(fixedDeltaTime);
        }
    }
    
    public void Step(float deltaTime)
    {
        if (_pController != null)
        {
            _pController.Step(deltaTime);
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

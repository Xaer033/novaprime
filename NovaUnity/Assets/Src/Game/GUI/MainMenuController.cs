using System;
using System.Collections;
using System.Collections.Generic;
using GhostGen;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

public class MainMenuController : BaseController 
{
    private GameStateMachine _gameStateMachine;
    
	public void Start (GameStateMachine stateMachine)
    {
        _gameStateMachine = stateMachine;
        _setupView();
	}
    
    private void _setupView()
    {
        viewFactory.CreateAsync<MainMenuView>("GUI/MainMenu/MainMenuView", v =>
        {
            view = v;
            view.AddListener(MenuUIEventType.PLAY, onPlay);
            view.AddListener(MenuUIEventType.CREDITS, onCredits);
            view.AddListener(MenuUIEventType.QUIT, onQuit);
            Singleton.instance.gui.screenFader.FadeIn();
        });
    }
    
    private MainMenuView mainMenuView
    {
        get { return view as MainMenuView; }
    }
    
    private void onPlay(GeneralEvent e)
    {
        _gameStateMachine.ChangeState(NovaGameState.SINGLEPLAYER_GAMEPLAY);
    }
    
    private void onCredits(GeneralEvent e)
    {
        Debug.Log("Credits!");
    }

    private void onQuit(GeneralEvent e)
    {
        Application.Quit();
//        viewFactory.RemoveView(_mainMenuView);
    }
}

﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class MainMenuController : BaseController 
{
    private MainMenuView    _mainMenuView;

	public void Start () 
	{
        _setupView();
	}
    
    private void _setupView()
    {
        viewFactory.CreateAsync<MainMenuView>("GUI/MainMenu/MainMenuView", v =>
        {
            _mainMenuView = v;
            Singleton.instance.gui.screenFader.FadeIn(3.35f, () =>
            {
                _mainMenuView.buttonGroupOne._quitButton.onClick.AddListener(onQuit);
            });
        });
    }
   

    private void onCredits()
    {
        Debug.Log("Credits!");
    }

    private void onQuit()
    {
        Application.Quit();
//        viewFactory.RemoveView(_mainMenuView);
    }
}

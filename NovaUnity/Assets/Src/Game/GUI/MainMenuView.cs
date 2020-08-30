using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GhostGen;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class MainMenuView : UIView
{
    
    [System.Serializable]
    public class ButtonGroupOne
    {
        public GameObject _container;
        public GButton _startButton;
        public GButton _creditsButton;
        public GButton _quitButton;
    }

    [System.Serializable]
    public class ButtonGroupTwo
    {
        public GameObject _container;
        public GButton _singlePlayerBtn;
        public GButton _passAndPlayBtn;
        public GButton _onlineBtn;
        public GButton _backBtn;
    }

    // public ButtonGroupOne buttonGroupOne;
    // public ButtonGroupTwo buttonGroupTwo;

    public GButton _startButton;
    public GButton _creditsButton;
    public GButton _quitButton;
    
    void Awake()
    {
        if(_startButton)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(_startButton.gameObject);
            _startButton.AddListener(UIEvent.TRIGGERED, onButton);
        }
        
        if(_creditsButton)
        {
            _creditsButton.AddListener(UIEvent.TRIGGERED, onButton);
        }
        
        if(_quitButton)
        {
            _quitButton.AddListener(UIEvent.TRIGGERED, onButton);
        }
    }

    private void onButton(GeneralEvent e)
    {
        string eventName = getEventForEvent(e);
        if(!string.IsNullOrEmpty(eventName))
        {
            DispatchEvent(eventName);
        }
    }

    private string getEventForEvent(GeneralEvent e)
    {
        if(e.data == _startButton) return MenuUIEventType.PLAY;
        if(e.data == _creditsButton) return MenuUIEventType.CREDITS;
        if(e.data == _quitButton) return MenuUIEventType.QUIT;

        return "";
    }
}

using GhostGen;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MainMenuView : UIView
{
    // public ButtonGroupOne buttonGroupOne;
    // public ButtonGroupTwo buttonGroupTwo;

    public CanvasGroup _canvasGroup;
    public GButton _startButton;
    public GButton _multiplayerButton;
    public GButton _creditsButton;
    public GButton _quitButton;
    
    void Awake()
    {
        if(_startButton)
        {
            bool hasGamepad = Gamepad.all.Count > 0;
            if(hasGamepad)
            {
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(_startButton.gameObject);
            }
            
            _startButton.AddListener(UIEvent.TRIGGERED, onButton);
        }

        if(_multiplayerButton)
        {
            _multiplayerButton.AddListener(UIEvent.TRIGGERED, onButton);
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
        if((Object)e.data == _startButton) return MenuUIEventType.PLAY;
        if((Object)e.data == _multiplayerButton) return MenuUIEventType.GOTO_MULTIPLAYER_LOBBY;
        if((Object)e.data == _creditsButton) return MenuUIEventType.CREDITS;
        if((Object)e.data == _quitButton) return MenuUIEventType.QUIT;

        return "";
    }
}

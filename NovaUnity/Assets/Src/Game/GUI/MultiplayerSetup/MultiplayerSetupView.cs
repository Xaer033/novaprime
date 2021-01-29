using System.Collections.Generic;
using GhostGen;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

public class MultiplayerSetupView : UIView
{
    
    public TextMeshProUGUI roomTitle;
    
    public GButton serverButton;
    public GButton hostButton;
    public GButton clientButton;
    public GButton backButton;

    public TMPro.TMP_InputField _serverField;
    

    void Awake()
    {
        if(serverButton != null)
        {
            serverButton.AddListener(UIEvent.TRIGGERED, onButton);
        }
        
        if(hostButton != null)
        {
            hostButton.AddListener(UIEvent.TRIGGERED, onButton);
        }

        if(clientButton != null)
        {
            clientButton.AddListener(UIEvent.TRIGGERED, onButton);
        }
        
        if(backButton != null)
        {
            backButton.AddListener(UIEvent.TRIGGERED, onButton);
        }
    }

    private void onButton(GeneralEvent e)
    {
        string eventName = getEventForEvent(e);
        if(!string.IsNullOrEmpty(eventName))
        {
            string serverIPAddress = "";
            if(eventName == MenuUIEventType.JOIN_SERVER && _serverField != null)
            {
                serverIPAddress = _serverField.text;
            }
            DispatchEvent(eventName, false, serverIPAddress);
        }
    }

    private string getEventForEvent(GeneralEvent e)
    {   
        if((GButton)e.data == serverButton) return MenuUIEventType.CREATE_SERVER;
        if((GButton)e.data == hostButton) return MenuUIEventType.CREAT_SERVER_AS_HOST;
        if((GButton)e.data == clientButton) return MenuUIEventType.JOIN_SERVER;
        if((GButton)e.data == backButton) return MenuUIEventType.BACK;

        return "";
    }
    protected override void OnViewUpdate()
    {
        if(IsInvalid(InvalidationFlag.STATIC_DATA))
        {
           
        }

        if (IsInvalid(InvalidationFlag.DYNAMIC_DATA))
        {
          
        }
    }
    
}

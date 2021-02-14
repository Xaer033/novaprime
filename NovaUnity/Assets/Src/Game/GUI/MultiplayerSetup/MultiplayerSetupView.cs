using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GhostGen;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerSetupView : UIView
{
    
    public TextMeshProUGUI roomTitle;
    
    public GButton serverButton;
    public GButton hostButton;
    public GButton clientButton;
    public GButton backButton;
    public GButton joinRoomButton;
    public GButton refreshButton;

    public TMP_InputField _serverNameInput;
    public TMP_InputField _serverAddressField;
    
    public ListScrollRect   _serverListRect = null;
    public RoomItemView     _serverEntryPrefab = null;
    public ToggleGroup      _serverToggleGroup;
    public CanvasGroup      _serverListCanvasGroup;
    public Image loadingCircle;

    private Tween _spinTween;


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

        if(joinRoomButton != null)
        {
            joinRoomButton.AddListener(UIEvent.TRIGGERED, onButton);
        }
        
        if(refreshButton != null)
        {
            refreshButton.AddListener(UIEvent.TRIGGERED, onButton);
        }
        
        if(_serverListRect != null)
        {
            _serverListRect.itemRendererFactory = itemRendererFactory;
        }
    }

    public void SetLobbyDataProvider(List<object> dataProvider)
    {
        _serverListRect.dataProvider = dataProvider;
        
    }
    
    public void SetSelectedItemCallback(Action<int, bool> callback)
    {
        if(_serverListRect != null)
        {
            _serverListRect.onSelectedItem += callback;
        }
    }
    
    private void onButton(GeneralEvent e)
    {
        string eventName = getEventForEvent(e);
        
        if(!string.IsNullOrEmpty(eventName))
        {
            string serverIPAddress = "";
            if(eventName == MenuUIEventType.JOIN_SERVER && _serverAddressField != null)
            {
                serverIPAddress = _serverAddressField.text;
            }

            if(_serverEntryPrefab != null)
            {
                if(eventName == MenuUIEventType.CREAT_SERVER_AS_HOST ||
                eventName == MenuUIEventType.CREATE_SERVER)
                {
                    serverIPAddress = _serverNameInput.text;
                }
            }
            
            DispatchEvent(eventName, false, serverIPAddress);
        }
    }

    public void StartLoadingTween()
    {
        if(_spinTween != null)
        {
            _spinTween.Kill();
        }

        if(loadingCircle != null)
        {
            _spinTween = loadingCircle.rectTransform.DOLocalRotate(new Vector3(0, 0, 360), 0.5f, RotateMode.LocalAxisAdd);
            _spinTween.SetLoops(-1);
            
            loadingCircle.gameObject.SetActive(true);    
            
        }
        
        if(_serverListCanvasGroup != null)
        {
            _serverListCanvasGroup.interactable = false;
        }
    }

    public void StopLoadingTween()
    {
        if(_spinTween != null && loadingCircle != null)
        {
            _spinTween.Kill();

            loadingCircle.gameObject.SetActive(false);
        }

        if(_serverListCanvasGroup != null)
        {
            _serverListCanvasGroup.interactable = true;
        }
    }
    
    private string getEventForEvent(GeneralEvent e)
    {   
        if((GButton)e.data == serverButton)     return MenuUIEventType.CREATE_SERVER;
        if((GButton)e.data == hostButton)       return MenuUIEventType.CREAT_SERVER_AS_HOST;
        if((GButton)e.data == clientButton)     return MenuUIEventType.JOIN_SERVER;
        if((GButton)e.data == refreshButton)    return MenuUIEventType.REFRESH_SERVER_LIST;
        if((GButton)e.data == joinRoomButton)   return MenuUIEventType.JOIN_LISTED_SERVER;
        if((GButton)e.data == backButton)       return MenuUIEventType.BACK;

        return "";
    }
    
    
    private GameObject itemRendererFactory(int itemType, Transform parent)
    {
        RoomItemView view = GameObject.Instantiate(_serverEntryPrefab, parent, false);
        view.toggle.group = _serverToggleGroup;
        return view.gameObject;
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

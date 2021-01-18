using System;
using System.Collections;
using System.Collections.Generic;
using GhostGen;
using UnityEngine;
using UnityEngine.UI;
using Object = System.Object;

public class MultiplayerLobbyView : UIView
{
    public ListScrollRect   _listScrollRect = null;
    public RoomItemView     _listItemPrefab = null;
    public ToggleGroup      _toggleGroup;
    
    public GButton           _joinButton;
    public GButton           _backButton;
    public GButton           _createButton;
    
    void Awake()
    {
        if(_listScrollRect != null)
        {
            _listScrollRect.itemRendererFactory = itemRendererFactory;
        }
        
        if(_joinButton)
        {
            _joinButton._button.interactable = false;
            _joinButton.AddListener(UIEvent.TRIGGERED, onButton);
        }
        
        if(_backButton)
        {
            _backButton.AddListener(UIEvent.TRIGGERED, onButton);
        }
        
        if(_createButton)
        {
            _createButton.AddListener(UIEvent.TRIGGERED, onButton);
        }
    }

    public void SetSelectedItemCallback(Action<int, bool> callback)
    {
        if(_listScrollRect != null)
        {
            _listScrollRect.onSelectedItem += callback;
        }
    }
    
    public void SetLobbyDataProvider(List<Hashtable> dataProvider)
    {
        
    }
    private GameObject itemRendererFactory(int itemType, Transform parent)
    {
        RoomItemView view = GameObject.Instantiate(_listItemPrefab, parent, false);
        view.toggle.group = _toggleGroup;
        return view.gameObject;
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
        if((Object)e.data == _joinButton)   return MenuUIEventType.JOIN_SERVER;
        if((Object)e.data == _backButton)   return MenuUIEventType.BACK;
        if((Object)e.data == _createButton) return MenuUIEventType.CREATE_SERVER;

        return "";
    }
    
    protected override void OnViewUpdate()
    {
        base.OnViewUpdate();
    }
    
}

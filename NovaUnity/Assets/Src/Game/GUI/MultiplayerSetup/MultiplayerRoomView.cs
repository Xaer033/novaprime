using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GhostGen;
using UnityEngine.Assertions;
using System;
using ExitGames.Client.Photon;
using Photon.Realtime;
using TMPro;

public class MultiplayerRoomView : UIView
{
    public TextMeshProUGUI roomTitle;
    
    public Button startButton;
    public Button leaveButton;
    public Toggle readyToggle;

    public RoomPlayerItemView playerItemViewPrefab;
    public Transform playerGroup;

    private List<RoomPlayerItemView> _playerItemViewList = new List<RoomPlayerItemView>(4);
    private List<bool> _playerIsReady = new List<bool>(4);
    private string _roomTitle;
    private bool _isMaster;

    void Awake()
    {

        if(startButton != null)
        {
            startButton.onClick.AddListener(onStartButton);
        }
        
        if(leaveButton != null)
        {
            leaveButton.onClick.AddListener(onBackButton);
        }

        if(readyToggle != null)
        {
            readyToggle.onValueChanged.AddListener(onToggle);
        }
        
        for (int i = 0; i < NetworkManager.kMaxPlayers; ++i)
        {
            RoomPlayerItemView pView = GameObject.Instantiate<RoomPlayerItemView>(playerItemViewPrefab, playerGroup, false);
            pView.gameObject.SetActive(false);
            _playerItemViewList.Add(pView);
            _playerIsReady.Add(false);
        }
    }

    public void IsMasterClient(bool isMaster)
    {
        if (_isMaster != isMaster)
        {
            _isMaster = isMaster;
            invalidateFlag = InvalidationFlag.STATIC_DATA;
        }
    }

    public void SetTitle(string titleText)
    {
        if(_roomTitle != titleText)
        {
            _roomTitle = titleText;
            invalidateFlag = InvalidationFlag.STATIC_DATA;
        }
    }

    public void SetPlayer(int index, Player player)
    {
        Assert.IsTrue(index < _playerItemViewList.Count);
    
        RoomPlayerItemView pView = _playerItemViewList[index];
        if(player != null)
        {
            pView.gameObject.SetActive(true);
            pView.playerId = player.ActorNumber;
            pView.playerName.text = player.NickName;
            pView.checkmark.gameObject.SetActive(false);
        }
        else
        {
            pView.gameObject.SetActive(false);
        }
    }

    public int GetIndexForPlayerId(int playerId)
    {
        int count = _playerItemViewList.Count;
        for(int i = 0; i < count; ++i)
        {
            if(_playerItemViewList[i] == null)
            {
                continue;
            }
            if(playerId == _playerItemViewList[i].playerId)
            {
                return i;
            }
        }
        return -1;
    }

    public void SetIsReady(int index, bool isActive)
    {
        Assert.IsTrue(index < _playerIsReady.Count);

        if (_playerIsReady[index] != isActive)
        {
            _playerIsReady[index] = isActive;
            invalidateFlag = InvalidationFlag.DYNAMIC_DATA;
        }
    }

    private void onBackButton()
    {
        DispatchEvent(MenuUIEventType.BACK);
    }

    private void onStartButton()
    {
        DispatchEvent(MenuUIEventType.CONTINUE);
    }

    private void onToggle(bool isSelected)
    {
        DispatchEvent(MenuUIEventType.TOGGLE, false, isSelected);
    }
    
    protected override void OnViewUpdate()
    {
        if(IsInvalid(InvalidationFlag.STATIC_DATA))
        {
            readyToggle.gameObject.SetActive(!_isMaster);
            startButton.gameObject.SetActive(_isMaster);
            
            roomTitle.text = _roomTitle;
        }

        if (IsInvalid(InvalidationFlag.DYNAMIC_DATA))
        {
            int count = _playerIsReady.Count;
            for (int i = 0; i < count; ++i)
            {
                _playerItemViewList[i].checkmark.gameObject.SetActive(_playerIsReady[i]);
            }
        }
    }
    
}

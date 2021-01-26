using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GhostGen;
using UnityEngine.Assertions;
using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class MultiplayerRoomView : UIView
{
    public class PlayerData
    {
        public bool isReady;
        public string nickName;
        public int actorNumber = -1;
    }
    
    public TextMeshProUGUI roomTitle;
    
    public GButton startButton;
    public GButton leaveButton;
    public GButton readyButton;

    public RoomPlayerItemView playerItemViewPrefab;
    public Transform playerGroup;

    private List<RoomPlayerItemView> _playerItemViewList = new List<RoomPlayerItemView>(4);
    private List<bool> _playerIsReady = new List<bool>(4);
    private Dictionary<int, PlayerData> _playerDataMap = new Dictionary<int, PlayerData>(4);
    
    private string _roomTitle;
    private bool _isMaster;

    void Awake()
    {

        if(startButton != null)
        {
            startButton.AddListener(UIEvent.TRIGGERED, onButton);
        }
        
        if(leaveButton != null)
        {
            leaveButton.AddListener(UIEvent.TRIGGERED, onButton);
        }

        if(readyButton != null)
        {
            readyButton.AddListener(UIEvent.TRIGGERED, onButton);
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

    public void SetPlayer(int index, Player player, bool isReady)
    {
        Assert.IsTrue(index < _playerItemViewList.Count);
    
        RoomPlayerItemView pView = _playerItemViewList[index];
        if(player != null)
        {
            // pView.gameObject.SetActive(true);
            // pView.actorNumber = player.ActorNumber;
            // pView.playerName.text = player.NickName;
            // pView.checkmark.gameObject.SetActive(false);

            PlayerData pData = new PlayerData();
            pData.actorNumber = player.ActorNumber;
            pData.isReady = isReady;
            pData.nickName = player.NickName;
            _playerDataMap[index] = pData;
        }
        else
        {
            // pView.gameObject.SetActive(false);
            _playerDataMap[index] = null;
        }

        invalidateFlag = InvalidationFlag.DYNAMIC_DATA;
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
            if(playerId == _playerItemViewList[i].actorNumber)
            {
                return i;
            }
        }
        return -1;
    }

    public void SetIsReady(int index, bool isReady)
    {
        Assert.IsTrue(index < _playerIsReady.Count);
        Assert.IsTrue(index >= 0);

        if(index < 0 || index >= _playerIsReady.Count)
        {
            return;
        }
        
        if (_playerIsReady[index] != isReady)
        {
            _playerIsReady[index] = isReady;
            _playerDataMap[index].isReady = isReady;
            invalidateFlag = InvalidationFlag.DYNAMIC_DATA;
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
        if((GButton)e.data == startButton) return MenuUIEventType.CONTINUE;
        if((GButton)e.data == leaveButton) return MenuUIEventType.BACK;
        if((GButton)e.data == readyButton) return MenuUIEventType.TOGGLE;

        return "";
    }
    protected override void OnViewUpdate()
    {
        if(IsInvalid(InvalidationFlag.STATIC_DATA))
        {
            readyButton.gameObject.SetActive(!_isMaster);
            startButton.gameObject.SetActive(_isMaster);
            
            roomTitle.text = _roomTitle;
        }

        if (IsInvalid(InvalidationFlag.DYNAMIC_DATA))
        {
            int localPlayerIndex = GetIndexForPlayerId(PhotonNetwork.LocalPlayer.ActorNumber);
            int count = NetworkManager.kMaxPlayers;
            for (int i = 0; i < count; ++i)
            {
                PlayerData pData = _playerDataMap[i];
                RoomPlayerItemView playerView = _playerItemViewList[i];
                
                playerView.gameObject.SetActive(pData != null);

                if(pData != null)
                {
                    bool isReady = pData.isReady;
                    playerView.checkmark.gameObject.SetActive(isReady);
                    playerView.playerName.text = pData.nickName;
                    playerView.actorNumber = pData.actorNumber;
                    
                    if(i == localPlayerIndex)
                    {
                        readyButton.text = isReady ? "Unready" : "Ready";
                    }
                }
            }
        }
    }
    
}

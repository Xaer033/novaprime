using System.Collections.Generic;
using GhostGen;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

public class MultiplayerRoomView : UIView
{
    public TextMeshProUGUI roomTitle;
    
    public GButton startButton;
    public GButton leaveButton;
    public GButton readyButton;

    public RoomPlayerItemView playerItemViewPrefab;
    public Transform playerGroup;

    private List<RoomPlayerItemView> _playerItemViewList = new List<RoomPlayerItemView>(4);
    // private List<bool> _playerIsReady = new List<bool>(4);
    private Dictionary<int, PlayerRoomData> _playerDataMap = new Dictionary<int, PlayerRoomData>(4);
    
    private string _roomTitle;
    private bool _isMaster;

    void Awake()
    {
        if(startButton != null)
        {
            startButton.AddListener(UIEvent.TRIGGERED, onButton);
            startButton.gameObject.SetActive(false);
        }
        
        if(leaveButton != null)
        {
            leaveButton.AddListener(UIEvent.TRIGGERED, onButton);
        }

        if(readyButton != null)
        {
            readyButton.AddListener(UIEvent.TRIGGERED, onButton);
        }
        
        for (int i = 0; i < (int)PlayerSlot.MAX_PLAYERS; ++i)
        {
            RoomPlayerItemView pView = Instantiate<RoomPlayerItemView>(playerItemViewPrefab, playerGroup, false);
            pView.gameObject.SetActive(false);
            _playerItemViewList.Add(pView);
        }
    }

    public void ClearPlayerViews()
    {
        _playerDataMap.Clear();
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

    public void SetPlayer(int index, NetPlayer player)
    {
        Assert.IsTrue(index < _playerItemViewList.Count);
    
        RoomPlayerItemView pView = _playerItemViewList[index];
        if(player.connectionId >= 0)
        {
            PlayerRoomData pData = new PlayerRoomData();
            pData.playerSlot = player.playerSlot;
            pData.isReady = player.isReadyUp;
            pData.nickName = player.nickName;
            _playerDataMap[index] = pData;
        }
        else
        {
            // pView.gameObject.SetActive(false);
            _playerDataMap[index] = null;
        }

        invalidateFlag = InvalidationFlag.DYNAMIC_DATA;
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
            // readyButton.gameObject.SetActive(!_isMaster);
            // startButton.gameObject.SetActive(_isMaster);
            
            roomTitle.text = _roomTitle;
        }

        if (IsInvalid(InvalidationFlag.DYNAMIC_DATA))
        {
            int localPlayerIndex = 0;//GetIndexForPlayerId(PhotonNetwork.LocalPlayer.ActorNumber);
            int count = (int)PlayerSlot.MAX_PLAYERS;
            for (int i = 0; i < count; ++i)
            {
                PlayerRoomData pData = _playerDataMap[i];
                RoomPlayerItemView playerView = _playerItemViewList[i];
                
                playerView.gameObject.SetActive(pData != null);

                if(pData != null)
                {
                    bool isReady = pData.isReady;
                    playerView.checkmark.gameObject.SetActive(isReady);
                    playerView.playerName.text = pData.nickName;
                    playerView.playerSlot = (int)pData.playerSlot;
                    
                    if(i == localPlayerIndex)
                    {
                        readyButton.text = isReady ? "Unready" : "Ready";
                    }
                }
            }
        }
    }
    
}

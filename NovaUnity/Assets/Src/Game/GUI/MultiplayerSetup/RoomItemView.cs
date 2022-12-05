using System;
using System.Collections;
using GhostGen;
using TMPro;
using UnityEngine.UI;

public class RoomItemView : UIView, IListItemView
{
    public Toggle toggle;
    public TextMeshProUGUI roomName;
    public TextMeshProUGUI serverIp;
    public TextMeshProUGUI playerCount;


    private ServerListEntry _data;
    private event Action<IListItemView, bool> _onSelected;

    private void Start()
    {
        //onTriggered += OnSelected;
        toggle.onValueChanged.AddListener(OnButtonClick);
    }
    

    public event Action<IListItemView, bool> OnSelected
    {
        add { _onSelected += value; }
        remove { _onSelected -= value; }
    }

    public int GetItemType()
    {
        return 0;
    }
    
    public bool isSelected { get; set; }

    public object viewData
    {
        get { return _data; }
        set
        {
            if(_data != value)
            {
                _data = value as ServerListEntry;
                invalidateFlag = InvalidationFlag.DYNAMIC_DATA;
            }
        }
    }

    private void OnButtonClick(bool value)
    {
        if(_onSelected != null)
        {
            _onSelected(this, value);
        }
    }

    protected override void OnViewUpdate()
    {
        base.OnViewUpdate();
        if(IsInvalid(InvalidationFlag.DYNAMIC_DATA) && _data != null)
        {
            roomName.text = _data.name;
            serverIp.text = _data.serverIp;
            playerCount.text = string.Format("{0}/{1}", _data.players, _data.capacity);
            
            // RREEEEEEEEEEEEEH
            toggle.onValueChanged.RemoveListener(OnButtonClick);
            toggle.isOn = isSelected;
            toggle.onValueChanged.AddListener(OnButtonClick);
        }
    }
}

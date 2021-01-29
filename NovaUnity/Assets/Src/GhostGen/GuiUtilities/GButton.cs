using GhostGen;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GButton : UIView
{
    public Image _image;
    public Button _button;
    public TextMeshProUGUI _text;

    private string _textValue;
    private Sprite _spriteValue;
    
    void Awake()
    {
        if(_button)
        {
            _button.onClick.AddListener(onClicked);
        }
    }

    public Sprite sprite
    {
        set
        {
            if(_spriteValue != value)
            {
                _spriteValue = value;
                invalidateFlag = InvalidationFlag.DYNAMIC_DATA;
            }
        }
    }
    
    public string text
    {
        set
        {
            if(_textValue != value)
            {
                _textValue = value;
                invalidateFlag = InvalidationFlag.DYNAMIC_DATA;
            }
        }
    }

    protected override void OnViewUpdate()
    {
        base.OnViewUpdate();
        if(IsInvalid(InvalidationFlag.DYNAMIC_DATA))
        {
            if(_image != null)
            {
                _image.sprite = _spriteValue;            
            }
        }
    }
    
    public override void OnViewDispose()
    {
        if(_button)
        {
            _button.onClick.RemoveListener(onClicked);
        }
        
        base.OnViewDispose();
    }

    private void onClicked()
    {
        DispatchEvent(UIEvent.TRIGGERED, true, this);
    }
}

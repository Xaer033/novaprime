using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class ScreenFader : MonoBehaviour
{
    public const float kDefaultFadeDuration = 0.25f;


    private Image _fadeBG;
    private CanvasGroup _canvasGroup;
    private Action _endCallback;
    private Tween _currentTween;

    // Use this for initialization
    void Awake()
    {
        _fadeBG = GetComponentInChildren<Image>();
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public Tween FadeIn(float duration = kDefaultFadeDuration, Action callback = null)
    {
        return _fade(1.0f, 0.0f, duration, callback);
    }

    public Tween FadeOut(float duration = kDefaultFadeDuration, Action callback = null)
    {
        return _fade(0.0f, 1.0f, duration, callback);
    }

    public Tween FadeTo(float endAlpha, float duration = kDefaultFadeDuration, Action callback = null)
    {
        return _fade(_canvasGroup.alpha, endAlpha, duration, callback);
    }

    public Tween FadeInOut(float fadedInDelay, float duration, Action onFadeOut, Action onFadeIn)
    {
        if(_currentTween != null)
        {
            _currentTween.Kill();
        }
        
        Sequence sequence = DOTween.Sequence();
        
        float halfDuration = duration / 2.0f;
        
        _canvasGroup.alpha = 0.0f;
        Tween fadeOutTween =  _canvasGroup.DOFade(1.0f, halfDuration);
        fadeOutTween.SetEase(Ease.OutQuad);
        
        Tween fadeInTween =  _canvasGroup.DOFade(0.0f, halfDuration);
        fadeInTween.SetEase(Ease.InQuad);
        
        
        sequence.Insert(0.0f, fadeOutTween);
        if(onFadeOut != null)
        {
            sequence.InsertCallback(halfDuration, ()=> onFadeOut());
        }
        sequence.Insert(halfDuration + fadedInDelay, fadeInTween);
        if(onFadeIn != null)
        {
            sequence.InsertCallback(duration + fadedInDelay, ()=> onFadeIn());
        }

        return sequence;
    }
    
    public Tween Fade(float startAlpha, float endAlpha, float duration = kDefaultFadeDuration, Action callback = null)
    {
        return _fade(startAlpha, endAlpha, duration, callback);
    }

    public Color color
    {
        get { return _fadeBG.color; }
        set { _fadeBG.color = value; }
    }

    public float alpha
    {
        set { _canvasGroup.alpha = value; }
        get { return _canvasGroup.alpha; }
    }

    private Tween _fade(float startAlpha, float endAlpha, float duration, Action callback)
    {
        Assert.IsNotNull(_canvasGroup);

        _canvasGroup.alpha = startAlpha;
        _endCallback = callback;
        if(_currentTween != null)
        {
            _currentTween.Kill();
        }

        _currentTween = _canvasGroup.DOFade(endAlpha, duration);
        _currentTween.SetEase(Ease.InOutQuad);
        _currentTween.OnComplete(OnFadeComplete);
//        OnFadeComplete();
        return _currentTween;
    }


    private void OnFadeComplete()
    {
        if(_endCallback != null)
        {
            _endCallback();
            _endCallback = null;
        }

        _currentTween = null;
    }
}


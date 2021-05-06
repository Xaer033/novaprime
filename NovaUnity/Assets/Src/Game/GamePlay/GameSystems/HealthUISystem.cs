using System.Collections.Generic;
using GhostGen;
using UnityEngine;

public class HealthUISystem : NotificationDispatcher, IGameSystem
{
    public const int kMaxUIView = 35;
    public float scaleConst = 10.0f;

    private GameSystems _gameSystems;
    private GuiCameraTag _guiCameraTag;
    private GameplayResources _gameplayResources;

    private Stack<HealthUIView> _viewPool = new Stack<HealthUIView>(kMaxUIView);
    private Dictionary<IAvatarController, HealthUIView> _inUseMap = new Dictionary<IAvatarController, HealthUIView>(kMaxUIView);
    private RectTransform _canvasRectTransform;
    
    private Camera _camera;
    private Vector3 _savedScale;
    
    public Canvas canvas
    {
        get { return _guiCameraTag.dynamicCanvas; }
    }
    
    public int priority { get; set; }

    public HealthUISystem(GameplayResources gameplayResources)
    {
        _gameplayResources = gameplayResources;
    }
    
    public void Start(bool hasAuthority, GameSystems gameSystems, GameState gameState)
    {
        _gameSystems = gameSystems;
        _gameSystems.onLateStep += LateStep;
        _gameSystems.AddListener(GamePlayEventType.AVATAR_SPAWNED, onAvatarSpawned);
        _gameSystems.AddListener(GamePlayEventType.AVATAR_DAMAGED, onAvatarDamaged);
        _gameSystems.AddListener(GamePlayEventType.AVATAR_DESTROYED, onAvatarDestroyed);
        
        _guiCameraTag = GameObject.FindObjectOfType<GuiCameraTag>();
        _canvasRectTransform = _guiCameraTag.uiCanvas.transform as RectTransform;

        HealthUIView healthUIPrefab = _gameplayResources.healthUIPrefab;
        _savedScale = healthUIPrefab.transform.localScale;

        for(int i = 0; i < kMaxUIView; ++i)
        {
            HealthUIView view = GameObject.Instantiate<HealthUIView>(
                healthUIPrefab, 
                canvas.transform, false);
            _viewPool.Push(view);
        }
    }

    public void LateStep(float deltaTime)
    {
        _updateHealthViews();
    }

    private void _updateHealthViews()
    {
        if(cam == null || canvas == null)
        {
            return;
        }

        foreach(var pair in _inUseMap)
        {
            IAvatarController c = pair.Key;
            HealthUIView view = pair.Value;

            AvatarView aView = c.view as AvatarView;
            Vector3 worldHealthPos = aView.GetHealthPosition();
            Vector3 anchorPos = getScreenPositionFromWorldPosition(worldHealthPos, cam);
            view.rectTransform.anchoredPosition = anchorPos;

            float dist = Vector3.Distance(cam.transform.position, worldHealthPos);
            dist = dist < 0.001f ? 0.001f : dist;

            float scaleMod = scaleConst * (1.0f / dist);
            view.transform.localScale = _savedScale * scaleMod;
        }
    }
    
    private Vector2 getScreenPositionFromWorldPosition(Vector3 worldPostion, Camera camera)
    {
        Vector2 viewportPosition = camera.WorldToViewportPoint(worldPostion);
        Vector2 screenPos = new Vector2(
             ((viewportPosition.x * _canvasRectTransform.sizeDelta.x) - (_canvasRectTransform.sizeDelta.x * 0.5f)),
             ((viewportPosition.y * _canvasRectTransform.sizeDelta.y) - (_canvasRectTransform.sizeDelta.y * 0.5f)));
        
        return new Vector2(screenPos.x, screenPos.y);
    }

    private Camera cam
    {
        get
        {
//            return _guiCameraTag.cam;
            if(_camera == null)
            {
                _camera = GameObject.FindObjectOfType<GameplayCamera>().gameCamera;
            }
            return _camera;
        }
    }

    public void CleanUp()
    {
        _gameSystems.onLateStep -= LateStep;
        _gameSystems.RemoveListener(GamePlayEventType.AVATAR_SPAWNED, onAvatarSpawned);
        _gameSystems.RemoveListener(GamePlayEventType.AVATAR_DESTROYED, onAvatarDestroyed);
        _gameSystems.RemoveListener(GamePlayEventType.AVATAR_DAMAGED, onAvatarDamaged);
        
        foreach(var pair in _inUseMap)
        {
            pair.Value.KillTween(false);
            GameObject.Destroy(pair.Value.gameObject);        
        }

        while(_viewPool.Count > 0)
        {
            HealthUIView v = _viewPool.Pop();
            v.KillTween(false);
            GameObject.Destroy(v.gameObject);
        }
    }

    public void ShowHealthOnAvatar(IAvatarController c)
    {
        HealthUIView view = getView(c);

        if(view != null)
        {
            float maxHealth = (float) c.unit.stats.maxHealth;
            maxHealth = maxHealth == 0 ? 1 : maxHealth;
            
            float healthPercentage = (float)c.state.health / maxHealth;
            view.SetHealthFill(healthPercentage, ()=> recycleView(c));
        }
    }

    public void RemoveView(IAvatarController c)
    {

        recycleView(c);
        //Debug.Log("Creep Remove");
    }

    private HealthUIView popView(IAvatarController c)
    {
        HealthUIView view = null;
        if(_viewPool.Count > 0)
        {
            view = _viewPool.Pop();
            //Debug.Log("Creep Added");

            //view.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("No More CreepHealthView!");
        }
        return view;
    }

    private void recycleView(IAvatarController c)
    {
        HealthUIView view;
        if(_inUseMap.TryGetValue(c, out view))
        {
            _inUseMap.Remove(c);
            _viewPool.Push(view);

            view.KillTween(false);
            view._canvasGroup.alpha = 0;
            //view.gameObject.SetActive(false);
        }
    }

    private HealthUIView getView(IAvatarController c)
    {
        HealthUIView view = null;
        
        if(c != null && !_inUseMap.TryGetValue(c, out view))
        {
            view = popView(c);
            if(view != null)
            {
                _inUseMap.Add(c, view);
            }
        }
        
        return view;
    }

    private void onAvatarSpawned(GeneralEvent e)
    {
        IAvatarController c = e.data as IAvatarController;
//        AddView(c);
    }
    
    private void onAvatarDestroyed(GeneralEvent e)
    {
        IAvatarController c = e.data as IAvatarController;
        RemoveView(c);
    }

    private void onAvatarDamaged(GeneralEvent e)
    {
        AttackResult attackResult = (AttackResult)e.data;
        IAvatarController c = attackResult.target as IAvatarController;
        ShowHealthOnAvatar(c);
    }
}

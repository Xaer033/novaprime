using System.Collections;
using System.Collections.Generic;
using GhostGen;
using UnityEngine;

public class HealthUISystem : NotificationDispatcher, IGameSystem
{
    public const int kMaxUIView = 35;
    public float scaleConst = 30.0f;

    private GuiCameraTag _guiCameraTag;
    
    public Canvas canvas
    {
        get { return _guiCameraTag.dynamicCanvas; }
    }


    private Stack<HealthUIView> _viewPool = new Stack<HealthUIView>(kMaxUIView);
    private Dictionary<IAvatarController, HealthUIView> _inUseMap = new Dictionary<IAvatarController, HealthUIView>(kMaxUIView);
    private RectTransform _canvasRectTransform;
    
    private Camera _camera;
    private Vector3 _savedScale;


    public void Start(GameSystems gameSystems, GameState gameState)
    {
        _guiCameraTag = GameObject.FindObjectOfType<GuiCameraTag>();
        _canvasRectTransform = _guiCameraTag.uiCanvas.transform as RectTransform;

        HealthUIView healthUIPrefab = Singleton.instance.gameplayResources.healthUIPrefab;
        _savedScale = healthUIPrefab.transform.localScale;

        for(int i = 0; i < kMaxUIView; ++i)
        {
            HealthUIView view = GameObject.Instantiate<HealthUIView>(
                healthUIPrefab, 
                canvas.transform, false);
            _viewPool.Push(view);
        }
        
        gameSystems.AddListener(GamePlayEventType.AVATAR_SPAWNED, onAvatarSpawned);
        gameSystems.AddListener(GamePlayEventType.AVATAR_DAMAGED, onAvatarDamaged);
        gameSystems.AddListener(GamePlayEventType.AVATAR_DESTROYED, onAvatarDestroyed);
    }

    public void FixedStep(float deltaTime)
    {
        if(cam == null || canvas == null)
        {
            return;
        }

        foreach(var pair in _inUseMap)
        {
            IAvatarController c = pair.Key;
            HealthUIView view = pair.Value;

            Vector3 worldHealthPos = c.GetView().GetHealthPosition();
            Vector3 anchorPos = getScreenPositionFromWorldPosition(worldHealthPos, cam);
            view.rectTransform.anchoredPosition = anchorPos;

            float dist = Vector3.Distance(cam.transform.position, worldHealthPos);
            dist = dist < 0.001f ? 0.001f : dist;

            float scaleMod = scaleConst * (1.0f / dist);
            view.transform.localScale = _savedScale * scaleMod;
        }
    }

    public void LateStep(float deltaTime)
    {
        
    }

    // Update is called once per frame
    public void Step(float deltaTime)
    {
        
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
        RemoveListener(GamePlayEventType.AVATAR_SPAWNED, onAvatarSpawned);
        RemoveListener(GamePlayEventType.AVATAR_DESTROYED, onAvatarDestroyed);
        RemoveListener(GamePlayEventType.AVATAR_DAMAGED, onAvatarDamaged);
        
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
            float health = (float)c.GetState().health / (float)c.GetUnit().stats.maxHealth;
            view.SetHealthFill(health, ()=> recycleView(c));
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
        HealthUIView view;
        if(!_inUseMap.TryGetValue(c, out view))
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

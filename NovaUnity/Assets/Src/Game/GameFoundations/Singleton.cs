using System;
using GhostGen;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class Singleton : MonoBehaviour
{
    [SerializeField]
    private bool firstScene = false;

    public GameConfig gameConfig { get; private set; }
    public GameStateMachine gameStateMachine { get; private set; }
    public SessionFlags sessionFlags { get; private set; }

    public GuiManager gui { get { return gameConfig.guiManager; } }
    public GameplayResources gameplayResources { get { return gameConfig.gameplayResources; } }
    public NetworkManager networkManager { get; private set; }

    public NotificationDispatcher notificationDispatcher { get; private set; }

    private IStateFactory _stateFactory;

    private Transform _sceneRoot;
    private GameObject _singleGameObject;

    private static object _lock = new object();
    private static bool applicationIsQuitting = false;
    private static Singleton _instance = null;
    
    
    public void Awake()
    {
        if (firstScene)
        {
            _instance = this;
            _initialize();
        }
        else
        {
            if (_instance != null)
            {
                Destroy(gameObject);
            }
        }
    }
    public void Start()
    {
        gameStateMachine.ChangeState(gameConfig.initalState);
    }
    
    public void Update()
    {
        gameStateMachine.Step(Time.deltaTime);
        gui.Step(Time.deltaTime);

    }

    public void FixedUpdate()
    {
        gameStateMachine.FixedStep(Time.fixedDeltaTime);
    }

    public static Singleton instance
    {
        get
        {
            if (applicationIsQuitting)
            {
                Debug.LogWarning("[Singleton] Instance " +
                    "already destroyed on application quit." +
                    " Won't create again - returning null.");
                return null;
            }

            if (_instance == null)
            {
                _instance = FindObjectOfType<Singleton>();

                if (FindObjectsOfType<Singleton>().Length > 1)
                {
                    Debug.LogError("[Singleton] Something went really wrong " +
                        " - there should never be more than 1 singleton!" +
                        " Reopening the scene might fix it.");
                    return _instance;
                }

                if (_instance == null)
                {
                    lock (_lock)
                    {
                        GameObject singleton = new GameObject();
                        _instance = singleton.AddComponent<Singleton>();
                        _instance._initialize();
                    }
                }
                else
                {
                    Debug.Log("[Singleton] Using instance already created: " +
                        _instance.gameObject.name);
                }
            }
            return _instance;
        }
    }

    private void _initialize()
    {
        name = "(singleton)";
        DontDestroyOnLoad(gameObject);
        
        _stateFactory = new NovaStateFactory();
        gameStateMachine = new GameStateMachine(_stateFactory);
        sessionFlags = new SessionFlags();
        notificationDispatcher = new NotificationDispatcher();

        networkManager = gameObject.AddComponent<NetworkManager>();
        gameConfig = Resources.Load<GameConfig>("GameConfig");
        Input.multiTouchEnabled = false; //TODO: This needs to go elsewere 

        _postInit();
    }

    private void _postInit()
    {
        //AbstractPostInit[] postInits = GameObject.FindObjectsOfType<AbstractPostInit>();

        gameConfig.PostInit();
        gui.PostInit();
//        networkManager.PostInit();

    }
    

    public Transform sceneRoot
    {
        get
        {
            if(!_sceneRoot)
            {
                GameObject obj = GameObject.FindGameObjectWithTag("Root");
                if(obj)
                {
                    _sceneRoot = obj.transform;
                }
            }

            return _sceneRoot;
        }
    }

    public void OnDestroy()
    {
        applicationIsQuitting = true;
    }
}

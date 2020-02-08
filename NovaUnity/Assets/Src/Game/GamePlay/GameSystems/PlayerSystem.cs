using System.Collections;
using System.Collections.Generic;
using GhostGen;
using UnityEngine;

public class PlayerSystem : NotificationDispatcher, IGameSystem
{
    public const int kMaxPlayerCount = 4;

    private GameState _gameState;
    private GameSystems _gameSystems;
    
    private List<PlayerController>      _playerControllerList;
    private List<PlayerAvatarView>      _playerViewList;
    private List<PlayerInput>           _playerInputList;
//    private List<PlayerActions>         _playerActionList;

    private GameplayCamera _camera;
    
    public PlayerSystem()
    {
        _playerControllerList     = new List<PlayerController>(kMaxPlayerCount);
        _playerViewList           = new List<PlayerAvatarView>(kMaxPlayerCount);
        _playerInputList          = new List<PlayerInput>(kMaxPlayerCount);
//        _playerActionList         = new List<PlayerActions>(kMaxPlayerCount);
    }
    
    public void Start(GameSystems gameSystems, GameState gameState)
    {
        _gameSystems = gameSystems;
        _gameState = gameState;
    }

    public void FixedStep(float deltaTime)
    {
        for (int i = 0; i < _playerControllerList.Count; ++i)
        {
            _playerControllerList[i].FixedStep(deltaTime);
        }
    }

    public void Step(float deltaTime)
    {
        for (int i = 0; i < _playerControllerList.Count; ++i)
        {
            _playerControllerList[i].Step(deltaTime);
        }
    }

    public PlayerController Spawn(int playerNumber, Vector3 position)
    {
        PlayerState state = PlayerState.Create(playerNumber, position);
        _gameState.playerStateList.Add(state);
        
        PlayerAvatarView view = GameObject.Instantiate<PlayerAvatarView>(Singleton.instance.gameplayResources.playerAvatar);

        GameplayCamera cam = _getGameplayCamera();
        if (cam != null)
        {
            cam.AddTarget(view.transform);
        }
        else
        {
            Debug.LogError("Could not find or create gameplay camera! Aborting player creation");
        }
        
        PlayerInput input = new PlayerInput(playerNumber, cam.gameCamera);
        
        PlayerController controller = new PlayerController(state, view, input);
        controller.Start(_gameSystems);
        
        _playerControllerList.Add(controller);
        _playerInputList.Add(input);
        _playerViewList.Add(view);
        
        return controller;
    }

    private GameplayCamera _getGameplayCamera()
    {
        if (_camera != null)
        {
            return _camera;
        }

        _camera = GameObject.FindObjectOfType<GameplayCamera>();
        if (_camera == null)
        {
            _camera = GameObject.Instantiate<GameplayCamera>(Singleton.instance.gameplayResources.gameplayCamera);
        }
        return _camera;
    }
}

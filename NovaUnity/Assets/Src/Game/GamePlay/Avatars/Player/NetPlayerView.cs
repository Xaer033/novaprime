using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;

public class NetPlayerView : NetworkBehaviour, INetworkRunnerCallbacks
{

    [Networked]
    public PlayerSlot playerSlot { get; set; }

    private PlayerController _controller;
    private IInputGenerator  _inputGenerator;
    private FrameInput       _lastInput;

    
    public void Initialize(UnitMap.Unit unit, AvatarState state, PlayerSlot slot)
    {
        _inputGenerator = new PlayerInputGenerator(PlayerSlot.P1, FindObjectOfType<GameplayCamera>().gameCamera);
        _controller     = new PlayerController(unit, state, GetComponent<PlayerView>(), _inputGenerator);
    }

    void Update()
    {
        _lastInput = _inputGenerator?.GetInput() ?? default(FrameInput);
    }
    
    public override void FixedUpdateNetwork()
    {
        if (GetInput<FrameInput>(out var playerInput))
        {
            _controller?.FixedStep(Runner.DeltaTime, playerInput);
            _inputGenerator?.Clear();
        }
    }
    
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        input.Set(_lastInput);
    }
    
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        
    }
    
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
}

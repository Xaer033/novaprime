using System;
using UnityEngine;

public class NetworkEntity : MonoBehaviour
{
    // public event Func<IAvatarView, NetworkWriter, bool, bool>    onSerialize;
    // public event Action<IAvatarView, NetworkReader, bool>        onDeserialize;
    // public event Action<IAvatarView> onStartAuthority;
    // public event Action<IAvatarView> onStopAuthority;
    // public event Action<IAvatarView> onStartClient;
    // public event Action<IAvatarView> onStopClient;
    // public event Action<IAvatarView> onStartServer;
    // public event Action<IAvatarView> onStopServer;
    // public event Action<IAvatarView> onStartLocalPlayer;

    protected  IAvatarView _view;
    
    protected virtual void Awake()
    {
        _view = GetComponent<IAvatarView>();
    }
    
    //
    //
    // public override void OnStartAuthority()
    // {
    //     onStartAuthority?.Invoke(_view);
    // }
    //
    // public override void OnStopAuthority()
    // {
    //     onStopAuthority?.Invoke(_view);
    // }
    //
    // public override void OnStartClient()
    // {
    //     onStartClient?.Invoke(_view);
    // }
    //
    // public override void OnStartServer()
    // {
    //     onStartServer?.Invoke(_view);
    // }
    //
    // public override void OnStartLocalPlayer()
    // {
    //     onStartLocalPlayer?.Invoke(_view);
    // }
    //
    // public override void OnStopClient()
    // {
    //     onStopClient?.Invoke(_view);
    // }
    //
    // public override void OnStopServer()
    // {
    //     onStopServer?.Invoke(_view);
    // }  
}

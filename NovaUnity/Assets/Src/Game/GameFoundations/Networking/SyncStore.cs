using System;
using Mirror;
using UnityEngine;

public class SyncStore : NetworkBehaviour
{
    public readonly SyncDictionary<PlayerSlot, NetPlayer> playerMap = new SyncDictionary<PlayerSlot, NetPlayer>();
    public event Action<SyncDictionary<PlayerSlot, NetPlayer>.Operation, PlayerSlot, NetPlayer> onPlayerMapChanged;

    private void Awake()
    {
        Debug.Log("OnStoreAwake!");
    }

    protected void Start()
    {
        playerMap.Callback += OnPlayerMapChanged;
    }

    protected void OnDestroy()
    {
        playerMap.Callback -= OnPlayerMapChanged;
    }


    private void OnPlayerMapChanged(SyncDictionary<PlayerSlot, NetPlayer>.Operation op, PlayerSlot slot, NetPlayer player)
    {
        onPlayerMapChanged?.Invoke(op, slot, player);
    }
}

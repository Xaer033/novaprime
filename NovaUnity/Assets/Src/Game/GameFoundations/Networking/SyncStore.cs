using System;
using System.Collections.Generic;
using UnityEngine;

public class SyncStore : MonoBehaviour
{
    public readonly Dictionary<PlayerSlot, NetPlayer> playerMap = new Dictionary<PlayerSlot, NetPlayer>();
    public event Action<Dictionary<PlayerSlot, NetPlayer>, PlayerSlot, NetPlayer> onPlayerMapChanged;
}

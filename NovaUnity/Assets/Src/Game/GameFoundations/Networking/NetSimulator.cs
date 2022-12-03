using System;
using Fusion;

public class NetSimulator : NetworkBehaviour
{
    public event Action<NetSimulator> onFixedNetworkUpdate;

    public override void FixedUpdateNetwork()
    {
        onFixedNetworkUpdate?.Invoke(this);
    }
}

using System.Collections.Generic;
using GhostGen;

public class NetworkSystem : NotificationDispatcher, IGameSystem
{
    private NetworkManager _networkManager;
    
    public int priority { get; set; }
    public bool isServer { get; private set; }

    public NetworkSystem(bool isAServer)
    {
        isServer = isAServer;
        _networkManager = Singleton.instance.networkManager;
    }

    public void Start(GameSystems gameSystems, GameState gameState)
    {
    }

    public void CleanUp()
    {
        
    }
}

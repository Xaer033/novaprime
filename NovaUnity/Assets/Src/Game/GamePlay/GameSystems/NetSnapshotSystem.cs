using System.Collections.Generic;
using GhostGen;
using Mirror;

public class NetSnapshotSystem : NotificationDispatcher, IGameSystem
{
    const int   SNAPSHOT_RATE         = 60;
    const float SNAPSHOT_INTERVAL     = 1.0f / SNAPSHOT_RATE;
    const int   SNAPSHOT_OFFSET_COUNT = 2;
    const float INTERPOLATION_OFFSET  = SNAPSHOT_INTERVAL * SNAPSHOT_OFFSET_COUNT;
    
    const float INTERPOLATION_TIME_ADJUSTMENT_NEGATIVE_THRESHOLD = SNAPSHOT_INTERVAL * -0.5f;
    const float INTERPOLATION_TIME_ADJUSTMENT_POSITIVE_THRESHOLD = SNAPSHOT_INTERVAL * 2;


    private GameSystems     _gameSystems;
    private GameState       _gameState;
    private NetworkManager  _networkManager;
    
    
    private FloatIntegratorEMA              _clientTimeOffsetAvg;
    private FloatIntegratorEMA              _clientSnapshotDeliveryDeltaAvg;
    private float?                          _clientLastSnapshotReceived;
    private float                           _clientMaxServerTimeReceived;
    private float                           _clientInterpolationTime;
    private float                           _clientInterpolationTimeScale;
    
    private RingBuffer<NetFrameSnapshot> _clientSnapshotBuffer = new RingBuffer<NetFrameSnapshot>(32);
    private List<GameState.Snapshot>      _clientSnapshots      = new List<GameState.Snapshot>();


    public int priority { get; set; }

    public NetSnapshotSystem()
    {
        _networkManager = Singleton.instance.networkManager;
    }
    
    // Start is called before the first frame update
    public void Start(GameSystems gameSystems, GameState gameState)
    {
        _gameSystems    = gameSystems;
        _gameState      = gameState;
        
        _gameSystems.onFixedStep += onFixedStep;
        
        _networkManager.onClientFrameSnapshot += onClientFrameSnapshot;
        
        


    }

    public void CleanUp()
    {
        _gameSystems.onFixedStep -= onFixedStep;
    }

    private void onFixedStep(float fixedDeltaStep)
    {
        
    }

    private void onClientFrameSnapshot(NetworkConnection conn, NetFrameSnapshot msg)
    {
         if(_networkManager.isHostClient)
        {
            return;
        }

        if(_clientSnapshotBuffer.IsEmpty)
        {
            _clientSnapshotBuffer.PushBack(msg);
            return;
        }

        if(msg.sendTime >= _clientSnapshotBuffer.Back().sendTime)
        {
            _clientSnapshotBuffer.PushBack(msg);
            _clientSnapshotBuffer.PopFront(); // Remove outdated snapshot
        }
    }
}

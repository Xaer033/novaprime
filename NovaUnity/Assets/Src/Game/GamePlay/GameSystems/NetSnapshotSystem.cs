using System;
using System.Collections.Generic;
using GhostGen;
using Mirror;
using UnityEngine;

public class NetSnapshotSystem : NotificationDispatcher, IGameSystem
{
    const int   SNAPSHOT_RATE         = 60;
    const float SNAPSHOT_INTERVAL     = 1.0f / SNAPSHOT_RATE;
    const int   SNAPSHOT_OFFSET_TICK_COUNT = 3;
    const float INTERPOLATION_OFFSET  = SNAPSHOT_INTERVAL * SNAPSHOT_OFFSET_TICK_COUNT;

    const float INTERPOLATION_TIME_ADJUSTMENT_NEGATIVE_THRESHOLD = SNAPSHOT_INTERVAL * -0.5f;
    const float INTERPOLATION_TIME_ADJUSTMENT_POSITIVE_THRESHOLD = SNAPSHOT_INTERVAL * 2;

    public event Action<float, GameState.Snapshot, GameState.Snapshot> onInterpolationUpdate;

    private GameSystems    _gameSystems;
    private GameState      _gameState;
    private NetworkManager _networkManager;


    private FloatIntegratorEMA     _clientTimeOffsetAvg;
    private FloatIntegratorEMA     _clientSnapshotDeliveryDeltaAvg;
    private float?                 _clientLastSnapshotReceived;
    private float                  _clientMaxServerTimeReceived;
    private float                  _clientInterpolationTime;
    private float                  _clientInterpolationTimeScale;
    private List<NetFrameSnapshot> _clientSnapshotList = new List<NetFrameSnapshot>();


    private uint _serverSendSequence;

    public int priority { get; set; }

    public NetSnapshotSystem()
    {
        _networkManager = Singleton.instance.networkManager;
    }

    // Start is called before the first frame update
    public void Start(bool hasAuthority, GameSystems gameSystems, GameState gameState)
    {
        _gameSystems = gameSystems;
        _gameState   = gameState;

        _gameSystems.onFixedStep += onFixedStep;
        _gameSystems.onStep      += onStep;

        _networkManager.onClientFrameSnapshot += onClientFrameSnapshotReceived;

        _clientInterpolationTimeScale = 1.0f;

        // moving avg integrator to track client offset vs server
        _clientTimeOffsetAvg            = new FloatIntegratorEMA(SNAPSHOT_RATE);
        _clientSnapshotDeliveryDeltaAvg = new FloatIntegratorEMA(SNAPSHOT_RATE);
    }

    public void CleanUp()
    {
        _gameSystems.onFixedStep -= onFixedStep;
        _gameSystems.onStep      -= onStep;
        
        
        _networkManager.onClientFrameSnapshot -= onClientFrameSnapshotReceived;
    }

    private void onFixedStep(float fixedDeltaTime)
    {
        if(NetworkServer.active)
        {
            _serverFixedStep(fixedDeltaTime);
        }
    }

    private void onStep(float deltaTime)
    {
        if(_networkManager.isHostClient)
        {
            return;
        }
        
        if(NetworkClient.active)
        {
            _clientStep(deltaTime);
        }
    }

    private void _clientStep(float deltaTime)
    {
        float adjustedDeltaTime = Time.unscaledDeltaTime * _clientInterpolationTimeScale;
        clientUpdateInterpolationTime(adjustedDeltaTime);
        clientInterpolateSnapshots();
    }

    private void _serverFixedStep(float fixedDeltaTime)
    {
        NetChannelHeader channelHeader = new NetChannelHeader
        {
            sequence     = _serverSendSequence,
            frameTick    = NetworkManager.frameTick,
            sendTime     = TimeUtil.Now()
        };

        NetFrameSnapshot snapshot = new NetFrameSnapshot
        {
            header   = channelHeader,
            snapshot = NetUtility.Snapshot(_gameState)
        };

        NetworkServer.SendToAll(snapshot, Channels.Unreliable);
        _serverSendSequence++;
    }

    private void _clientFixedStep(float fixedDeltaTime)
    {
        // Apply Snapshots
    }

    private void clientUpdateInterpolationTime(float deltaTime)
    {
        _clientInterpolationTime += _clientSnapshotList.Count > 0 ? deltaTime : 0;
    }

    private void onClientFrameSnapshotReceived(NetFrameSnapshot msg)
    {
        if(_networkManager.isHostClient)
        {
            return;
        }
        
        double now      = TimeUtil.Now();

        // this is our first snapshot
        if(_clientSnapshotList.Count == 0)
        {
            _clientInterpolationTime = (float) msg.header.sendTime - INTERPOLATION_OFFSET;
        }

        _clientSnapshotList.Add(msg);
        _clientMaxServerTimeReceived = Math.Max(_clientMaxServerTimeReceived, (float) msg.header.sendTime);

        if(_clientLastSnapshotReceived.HasValue)
        {
            _clientSnapshotDeliveryDeltaAvg.Integrate((float) now - _clientLastSnapshotReceived.Value);
        }

        // for next time we receive a snapshot
        _clientLastSnapshotReceived = (float)now;

        // this is the difference between latest time we've received from the server and our local interpolation time 
        var diff = _clientMaxServerTimeReceived - _clientInterpolationTime;

        _clientTimeOffsetAvg.Integrate(diff);

        // this is the difference between our current time offset and the wanted interpolation offset
        var diffWanted = _clientTimeOffsetAvg.average - INTERPOLATION_OFFSET;

        // if diffWanted is positive it means that we are *a head* of where we want to be (i.e. more offset than needed) 
        if(diffWanted > INTERPOLATION_TIME_ADJUSTMENT_POSITIVE_THRESHOLD)
        {
            _clientInterpolationTimeScale = 1.01f;
        }
        else if(diffWanted < INTERPOLATION_TIME_ADJUSTMENT_NEGATIVE_THRESHOLD)
        {
            _clientInterpolationTimeScale = 0.99f;
        }
        else
        {
            _clientInterpolationTimeScale = 1.0f;
        }

        // Debug.Log($"diff: {diff:F3}, diffWanted: {diffWanted:F3}, timeScale:{_clientInterpolationTimeScale:F3}, deliveryDeltaAvg:{_clientSnapshotDeliveryDeltaAvg.average}");
    }

    private void clientInterpolateSnapshots()
    {
        if(_clientSnapshotList.Count <= 0)
        {
            return;
        }

        var interpFrom  = default(GameState.Snapshot);
        var interpTo    = default(GameState.Snapshot);
        var interpAlpha = default(float);

        for(int i = 0; i < _clientSnapshotList.Count; ++i)
        {
            if(i + 1 == _clientSnapshotList.Count)
            {
                if(_clientSnapshotList[0].header.sendTime > _clientInterpolationTime)
                {
                    interpFrom  = interpTo = _clientSnapshotList[0].snapshot;
                    interpAlpha = 0;
                }
                else
                {
                    interpFrom  = interpTo = _clientSnapshotList[i].snapshot;
                    interpAlpha = 0;
                }
            }
            else
            {
                int f = i;
                int t = i + 1;

                if(_clientSnapshotList[f].header.sendTime <= _clientInterpolationTime &&
                   _clientSnapshotList[t].header.sendTime >= _clientInterpolationTime)
                {
                    interpFrom = _clientSnapshotList[f].snapshot;
                    interpTo   = _clientSnapshotList[t].snapshot;

                    var range   = _clientSnapshotList[t].header.sendTime - _clientSnapshotList[f].header.sendTime;
                    var current = _clientInterpolationTime - _clientSnapshotList[f].header.sendTime;

                    interpAlpha = Mathf.Clamp01((float)(current / range));

                    break;
                }
            }
        }

        onInterpolationUpdate?.Invoke(interpAlpha, interpFrom, interpTo);
    }
}

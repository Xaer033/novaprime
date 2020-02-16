using UnityEngine;

public class EnemyState : AvatarState
{
    public static EnemyState Create(string uuid, Vector3 position)
    {
        EnemyState state = new EnemyState();
        state.position = position;
        state.health = 100;
        state.velocity = Vector3.zero;
        state.timeScale = 1.0f;
        state.uuid = uuid;
        state.machineGunState = new MachineGunState();
        return state;
    }

    public MachineGunState machineGunState;
    public LocomotionState locomotionState;
    public AiState aiState;
    public string targetUUID;
    
    
    
}

public enum LocomotionState
{
    NONE = 0,
    STANDING,
    WALKING,
    RUNNING,
    FLYING
}

public enum AiState
{
    NONE = 0,
    IDLE,
    ACTIVE_HUNTING,
    TARGETING,
    ATTACKING
}

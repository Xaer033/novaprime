using UnityEngine;

public class PlayerState : AvatarState
{
    public static PlayerState Create(string uuid,  UnitStats stats, Vector3 position)
    {
        PlayerState state = new PlayerState();
        state.playerNumber = 0;
        state.position = position;
        state.previousPosition = position;
        state.health = stats.maxHealth;
        state.velocity = Vector3.zero;
        state.timeScale = 1.0f;
        state.machineGunState = new MachineGunState();
        state.stateType = PlayerActivityType.NONE;
        state.uuid = uuid;
        state.stats = stats;
        return state;
    }

    public int playerNumber;
    public PlayerActivityType stateType;

    public UnitStats stats;
    public MachineGunState machineGunState;
}

public enum PlayerActivityType
{
    NONE = 0,
    ACTIVE,
    DEAD
}

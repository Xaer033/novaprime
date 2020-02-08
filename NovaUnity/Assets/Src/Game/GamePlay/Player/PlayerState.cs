using UnityEngine;

public class PlayerState
{
    public static PlayerState Create(int playerNumber, Vector3 position)
    {
        PlayerState state = new PlayerState();
        state.playerNumber = playerNumber;
        state.position = position;
        state.health = 100;
        state.velocity = Vector3.zero;
        state.timeScale = 1.0f;
        state.machineGunState = new MachineGunState();
        state.stateType = PlayerActivityType.NONE;

        return state;
    }

    public int playerNumber;
    public Vector3 position;
    public Vector3 velocity;
    
    public int health;
    public float timeScale;
    public PlayerActivityType stateType;

    public MachineGunState machineGunState;
}

public enum PlayerActivityType
{
    NONE = 0,
    ACTIVE,
    DEAD
}

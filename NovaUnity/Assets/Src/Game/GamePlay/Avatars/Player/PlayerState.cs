using System.Text;
using UnityEngine;

public class PlayerState : AvatarState
{
    public static PlayerState Create(string uuid,  int playerNumber, Vector3 position)
    {
        PlayerState state = new PlayerState();
        state.playerNumber = playerNumber;
        state.position = position;
        state.health = 100;
        state.velocity = Vector3.zero;
        state.timeScale = 1.0f;
        state.machineGunState = new MachineGunState();
        state.stateType = PlayerActivityType.NONE;
        state.uuid = uuid;
        return state;
    }

    public int playerNumber;
    public PlayerActivityType stateType;

    public MachineGunState machineGunState;
}

public enum PlayerActivityType
{
    NONE = 0,
    ACTIVE,
    DEAD
}

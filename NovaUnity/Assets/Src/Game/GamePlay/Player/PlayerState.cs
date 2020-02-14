using System.Text;
using UnityEngine;

public class PlayerState
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
        state.uuid = uuid;//StringUtil.CreateMD5(data.name + "_" + playerNumber);
        return state;
    }


    public int playerNumber;
    public Vector3 position;
    public Vector3 velocity;
    
    public int health;
    public float timeScale;
    public PlayerActivityType stateType;

    public MachineGunState machineGunState;
    
    public float timeToWallUnstick;
    public float wallSlideVelocity;
    public float midairJumpTimer;
    public float coyoteJumpTimer;
    public Vector3 previousPosition;
    public float velocityXSmoothing;

    public string uuid;
    
}

public enum PlayerActivityType
{
    NONE = 0,
    ACTIVE,
    DEAD
}

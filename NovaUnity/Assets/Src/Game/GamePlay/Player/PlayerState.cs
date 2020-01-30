using UnityEngine;

public class PlayerState
{
    public static PlayerState Create(Vector3 position)
    {
        PlayerState state = new PlayerState();
        state.position = position;
        state.health = 100;
        state.velocity = Vector3.zero;
        state.timeScale = 1.0f;
        state.machineGunState = new MachineGunState();

        return state;
    }
    
    public Vector3 position;
    public Vector3 velocity;
    
    public int health;
    public float timeScale;


    public MachineGunState machineGunState;


}

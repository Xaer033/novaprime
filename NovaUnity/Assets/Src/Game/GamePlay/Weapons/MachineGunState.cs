using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineGunState
{

    public MachineGunState()
    {
        fireTimer = 0;
        bulletList = new List<ProjectileState>(20);
    }
    
    public float fireTimer;
    public List<ProjectileState> bulletList;
}

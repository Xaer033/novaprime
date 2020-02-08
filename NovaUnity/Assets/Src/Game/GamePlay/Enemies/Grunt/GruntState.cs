using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GruntState
{
    public int health;
    public float timeScale;
    public Vector3 position;
    public Vector3 velocity;

    public LocomotionState locomotionState;
    public AiState aiState;
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

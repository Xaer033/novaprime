using System;
using UnityEngine;

[Serializable]
public class PlatformData
{
    public PlatformType type;
    public string triggerTag;
    public PlatformCycleMode cycleMode;
    public float waitTime;
    [Range(0,2)]
    public float easeAmount =1f;
    public float speed;
    [HideInInspector]
    public Vector3 startPosition;
}

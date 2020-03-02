using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITimeWarpTarget
{
    void OnTimeWarpEnter(float timeScale);
    void OnTimeWarpExit();
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAvatarController : ITimeWarpTarget
{
    void Move(Vector3 moveDelta, bool isOnPlatform);
}

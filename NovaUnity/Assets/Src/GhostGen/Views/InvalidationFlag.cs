using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GhostGen
{
    [System.Flags]
    public enum InvalidationFlag
    {
        NONE            = 1 << 0,
        STATIC_DATA     = 1 << 1,
        DYNAMIC_DATA    = 1 << 2,
        RESIZE          = 1 << 3,

        ALL = STATIC_DATA | DYNAMIC_DATA | RESIZE
    }
}

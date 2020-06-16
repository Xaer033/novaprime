using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GhostGen
{
    public class MathUtil
    {
        public static float Ease(float x, float easeAmount)
        {
            float a = easeAmount + 1;
            return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(1 - x, a));
        }
    }
}

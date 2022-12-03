using System;
using UnityEngine;
public class TimeUtil
{
    public static double FixedTimeSinceGameStartAsDouble()
    {
        return Time.fixedUnscaledTimeAsDouble;
    }

    public static float FixedTimeSinceGameStart()
    {
        return Time.fixedUnscaledTime;
    }
    
    public static DateTime Now()
    {
        return DateTime.UtcNow;
    }
}

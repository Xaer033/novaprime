using System;
using UnityEngine;
public class TimeUtil
{
    public static double TimeSinceGameStart()
    {
        return Time.unscaledTimeAsDouble;
    }

    public static DateTime Now()
    {
        return DateTime.UtcNow;
    }
}

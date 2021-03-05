using Mirror;
using UnityEngine;

public class TimeUtil
{
    public static double Now()
    {
        return Time.unscaledTimeAsDouble;
    }

    public static int dateNow()
    {
        return System.DateTime.Now.Second;
    }
}

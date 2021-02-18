public class TimeUtil
{
    public static double timestamp()
    {
        return Singleton.instance.networkManager.time != null ? Singleton.instance.networkManager.time.Time : 0.0;
    }
}

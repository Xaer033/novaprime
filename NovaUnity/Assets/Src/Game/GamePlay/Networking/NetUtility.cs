public class NetUtility 
{
    public static GameState.Snapshot Snapshot(GameState state)
    {
        return new GameState.Snapshot(state);
    }
    
    public static PlayerState.NetSnapshot Snapshot(PlayerState state)
    {
        return new PlayerState.NetSnapshot
        {
            netId       = state.netId,
            position    = state.position,
            aimPosition = state.aimPosition
        };
    } 
    
    public static EnemyState.NetSnapshot Snapshot(EnemyState state)
    {
        return new EnemyState.NetSnapshot
        {
            netId       = state.netId,
            position    = state.position,
            aimPosition = state.aimPosition
        };
    } 
    
    public static ProjectileState.NetSnapshot Snapshot(ProjectileState state)
    {
        return  new ProjectileState.NetSnapshot
        {
            netId       = state.netId,
            position = state.position,
            angle    = state.angle
        };
    } 
    
    public static PlatformState.NetSnapshot Snapshot(PlatformState state)
    {
        return  new PlatformState.NetSnapshot
        {
            netId       = state.netId,
            position = state.position
        };
    } 
}

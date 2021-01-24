using Photon.Realtime;

public class NetworkPlayer
{
    public readonly Player player;
    public readonly int number;
    public readonly string nickName;
    public readonly string userId;
    
    
    public NetworkPlayer(Player nPlayer)
    {
        player = nPlayer;
        number = nPlayer.ActorNumber;
        nickName = nPlayer.NickName;
        userId = nPlayer.UserId;
    }
}

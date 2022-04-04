using System.Collections.Generic;
public struct SyncLobbyPlayers
{
    public NetPlayer[] playerList;

    public Dictionary<PlayerSlot, NetPlayer> GetPlayerMap()
    {
        Dictionary<PlayerSlot, NetPlayer> result = new Dictionary<PlayerSlot, NetPlayer>();
        foreach(NetPlayer player in playerList)
        {
            result[player.playerSlot] = player;
        }

        return result;
    }
}

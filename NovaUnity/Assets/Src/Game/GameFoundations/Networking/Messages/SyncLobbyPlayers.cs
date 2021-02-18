using System.Collections.Generic;
using Mirage;

[NetworkMessage]
public struct SyncLobbyPlayers
{
    // public Dictionary<int, NetPlayer> playerMap;

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

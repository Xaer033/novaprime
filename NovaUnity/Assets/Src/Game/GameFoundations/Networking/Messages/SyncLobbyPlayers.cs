using System;
using System.Collections.Generic;
using Mirror;

public struct SyncLobbyPlayers : NetworkMessage
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

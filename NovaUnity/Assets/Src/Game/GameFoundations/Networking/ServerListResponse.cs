using System;
using System.Collections.Generic;

[Serializable]
public class ServerListResponse
{
    public int count;
    public List<ServerListEntry> servers;
    public int updateFrequency;
}

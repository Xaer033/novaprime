using System;
using Mirror;
using UnityEngine;

public class PlayerNetEntity : NetworkEntity
{
   public event Action<IAvatarView, double, Vector2, Vector2, Vector2> onClientUpdate;
   public event Action<IAvatarView, Vector2, Vector2, Vector2> onServerUpdate;
   
   [ClientRpc(channel = Channels.Unreliable)]
   public void RpcClientUpdate(double sendTimestamp, Vector2 velocity, Vector2 position, Vector2 aimPosition)
   {
      onClientUpdate?.Invoke(_view, sendTimestamp, velocity, position, aimPosition);
   }

   [Command(channel = Channels.Unreliable)]
   public void CmdServerUpdate(Vector2 velocity, Vector2 position, Vector2 aimPosition)
   {
      onServerUpdate?.Invoke(_view, velocity, position, aimPosition);
   }
}

using System;
using Mirror;
using UnityEngine;

public class PlayerNetEntity : NetworkEntity
{
   public event Action<IAvatarView, double, Vector2, Vector2, Vector2> onClientReceiveUpdate;
   public event Action<IAvatarView, Vector2, Vector2, Vector2> onClientSendUpdate;
   
   [ClientRpc(channel = Channels.DefaultUnreliable)]
   public void RpcClientReceiveUpdate(double sendTimestamp, Vector2 velocity, Vector2 position, Vector2 aimPosition)
   {
      onClientReceiveUpdate?.Invoke(_view, sendTimestamp, velocity, position, aimPosition);
   }

   [Command(channel = Channels.DefaultUnreliable)]
   public void CmdClientSendUpdate(Vector2 velocity, Vector2 position, Vector2 aimPosition)
   {
      onClientSendUpdate?.Invoke(_view, velocity, position, aimPosition );
   }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror;

namespace RTS.Experimental
{
    public class RTSNetworkManager : NetworkManager
    {
        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);
            Print.Log("Client Connected");
        }
        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            base.OnServerAddPlayer(conn);

            var player = conn.identity.GetComponent<RTS_Player>();
            player.SetDisplayName($"Player {numPlayers}");
            player.SetColor(Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f));
        }
    }
}
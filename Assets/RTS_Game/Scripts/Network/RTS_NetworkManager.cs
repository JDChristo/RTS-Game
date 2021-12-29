using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror;

namespace RTS.Network
{
    public class RTS_NetworkManager : NetworkManager
    {
        [Header("SLOT")]
        [SerializeField]
        private GameObject unitSpawnerPrefab = null;
        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            base.OnServerAddPlayer(conn);

            GameObject spawner = Instantiate(unitSpawnerPrefab, conn.identity.transform.position, conn.identity.transform.rotation);
            NetworkServer.Spawn(spawner, conn);
        }
    }
}

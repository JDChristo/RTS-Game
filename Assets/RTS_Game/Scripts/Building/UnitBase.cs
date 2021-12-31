using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

using RTS.Combat;

namespace RTS.Building
{
    public class UnitBase : NetworkBehaviour
    {
        [SerializeField] private Health health;

        public static event Action<int> ServerOnPlayerDie;
        public static event Action<UnitBase> ServerOnBaseSpawned;
        public static event Action<UnitBase> ServerOnBaseDespawned;
        #region Server
        public override void OnStartServer()
        {
            base.OnStartServer();
            ServerOnBaseSpawned?.Invoke(this);
            health.ServerOnDie += Die;
        }
        public override void OnStopServer()
        {
            base.OnStopServer();
            ServerOnBaseDespawned?.Invoke(this);
            health.ServerOnDie -= Die;
        }

        [Server]
        private void Die()
        {
            ServerOnPlayerDie?.Invoke(connectionToClient.connectionId);
            NetworkServer.Destroy(gameObject);
        }
        #endregion

        #region  Client

        #endregion
    }
}

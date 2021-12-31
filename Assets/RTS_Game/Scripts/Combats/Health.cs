using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

using RTS.Building;

namespace RTS.Combat
{
    public class Health : NetworkBehaviour
    {
        [SerializeField]
        private int maxHealth = 100;

        [SyncVar(hook = nameof(HealthUpdated))]
        private int currentHealth;

        public event Action ServerOnDie;

        public event Action<int, int> ClientOnHealthChange;

        #region  Server
        public override void OnStartServer()
        {
            base.OnStartServer();
            currentHealth = maxHealth;
            UnitBase.ServerOnPlayerDie += ServerOnPlayerDie;
        }

        public override void OnStopServer()
        {
            base.OnStopServer();
            UnitBase.ServerOnPlayerDie -= ServerOnPlayerDie;
        }

        [Server]
        public void DealDamage(int damageAmount)
        {
            if (currentHealth <= 0) { return; }
            currentHealth -= damageAmount;

            if (currentHealth > 0) { return; }
            ServerOnDie?.Invoke();
        }
        [Server]
        private void ServerOnPlayerDie(int id)
        {
            if (connectionToClient.connectionId != id) { return; }
            DealDamage(currentHealth);
        }
        #endregion

        #region  Client
        private void HealthUpdated(int oldHealth, int newHealth)
        {
            ClientOnHealthChange?.Invoke(newHealth, maxHealth);
        }
        #endregion

    }
}

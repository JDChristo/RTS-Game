using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using Mirror;
using RTS.Combat;

namespace RTS.Player
{
    public class Unit : NetworkBehaviour
    {
        [SerializeField]
        private UnitMovement unitMovement;
        [SerializeField]
        private Health health;
        [SerializeField]
        private Targeter targeter;
        [SerializeField]
        private UnityEvent onSelected;

        [SerializeField]
        private UnityEvent onDeselected;

        public static event Action<Unit> ServerOnUnitSpawned;
        public static event Action<Unit> ServerOnUnitDespawned;

        public static event Action<Unit> AuthorityOnUnitSpawned;
        public static event Action<Unit> AuthorityOnUnitDespawned;

        public UnitMovement UnitMovement => unitMovement;
        public Targeter Targeter => targeter;

        #region Server
        public override void OnStartServer()
        {
            base.OnStartServer();
            ServerOnUnitSpawned?.Invoke(this);
            health.ServerOnDie += Die;
        }
        public override void OnStopServer()
        {
            base.OnStopServer();
            ServerOnUnitDespawned?.Invoke(this);
            health.ServerOnDie -= Die;
        }
        [Server]
        private void Die()
        {
            NetworkServer.Destroy(gameObject);
        }
        #endregion

        #region  Client
        public override void OnStartAuthority()
        {
            base.OnStartAuthority();
            AuthorityOnUnitSpawned?.Invoke(this);
        }
        public override void OnStopClient()
        {
            base.OnStopClient();

            if (!hasAuthority) { return; }
            AuthorityOnUnitDespawned?.Invoke(this);
        }

        [Client]
        public void Select()
        {
            if (!hasAuthority) { return; }
            onSelected?.Invoke();
        }

        [Client]
        public void Deselect()
        {
            if (!hasAuthority) { return; }
            onDeselected?.Invoke();
        }

        #endregion
    }
}
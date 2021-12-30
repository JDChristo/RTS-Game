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
        private Targeter targeter;
        [SerializeField]
        private UnityEvent onSelected;

        [SerializeField]
        private UnityEvent onDeselected;

        public static event Action<Unit> ServerOnUnitSpawned;
        public static event Action<Unit> ServerOnUnitDespawned;

        public static event Action<Unit> AuthorityOnUnitSpawned;
        public static event Action<Unit> AuthorityOnUnitDespawned;

        public UnitMovement GetUnitMovement() => unitMovement;
        public Targeter Targeter => targeter;

        #region Server
        public override void OnStartServer()
        {
            base.OnStartServer();
            ServerOnUnitSpawned?.Invoke(this);
        }
        public override void OnStopServer()
        {
            base.OnStopServer();
            ServerOnUnitDespawned?.Invoke(this);
        }
        #endregion

        #region  Client
        public override void OnStartClient()
        {
            base.OnStartClient();

            if (!isClientOnly || !hasAuthority) { return; }
            AuthorityOnUnitSpawned?.Invoke(this);
        }
        public override void OnStopClient()
        {
            base.OnStopClient();

            if (!isClientOnly || !hasAuthority) { return; }
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror;

namespace RTS.Player
{
    public class RTS_Player : NetworkBehaviour
    {
        public List<Unit> MyUnits { get; } = new List<Unit>();

        #region Server
        public override void OnStartServer()
        {
            base.OnStartServer();
            Unit.ServerOnUnitSpawned += ServerUnitSpawned;
            Unit.ServerOnUnitDespawned += ServerUnitDespawned;
        }

        public override void OnStopServer()
        {
            base.OnStopServer();
            Unit.ServerOnUnitSpawned -= ServerUnitSpawned;
            Unit.ServerOnUnitDespawned -= ServerUnitDespawned;
        }
        #endregion

        private void ServerUnitSpawned(Unit unit)
        {
            if (unit.connectionToClient.connectionId != connectionToClient.connectionId) { return; }
            MyUnits.Add(unit);
        }
        private void ServerUnitDespawned(Unit unit)
        {
            if (unit.connectionToClient.connectionId != connectionToClient.connectionId) { return; }
            MyUnits.Remove(unit);
        }

        #region Client
        public override void OnStartClient()
        {
            base.OnStartClient();

            if (!isClientOnly) { return; }
            Unit.AuthorityOnUnitSpawned += AuthorityUnitSpawned;
            Unit.AuthorityOnUnitDespawned += AuthorityUnitDespawned;
        }
        public override void OnStopClient()
        {
            base.OnStopClient();

            if (!isClientOnly) { return; }
            Unit.AuthorityOnUnitSpawned -= AuthorityUnitSpawned;
            Unit.AuthorityOnUnitDespawned -= AuthorityUnitDespawned;
        }
        #endregion

        private void AuthorityUnitSpawned(Unit unit)
        {
            if (!hasAuthority) { return; }
            MyUnits.Add(unit);
        }
        private void AuthorityUnitDespawned(Unit unit)
        {
            if (!hasAuthority) { return; }
            MyUnits.Remove(unit);
        }

    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

using Mirror;

using RTS.Combat;

namespace RTS.Building
{
    public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
    {
        [SerializeField]
        private Health health = null;
        [SerializeField]
        private GameObject unitPrefab = null;
        [SerializeField]
        private Transform unitSpawnPoint = null;

        #region  Server
        public override void OnStartServer()
        {
            base.OnStartServer();
            health.ServerOnDie += Die;
        }
        public override void OnStopServer()
        {
            base.OnStopServer();
            health.ServerOnDie -= Die;
        }
        [Server]
        private void Die()
        {
            NetworkServer.Destroy(gameObject);
        }
        [Command]
        private void CmdSpawnUnit()
        {
            GameObject unit = Instantiate(unitPrefab, unitSpawnPoint.position, unitSpawnPoint.rotation);
            NetworkServer.Spawn(unit, connectionToClient);
        }
        #endregion

        #region  Client

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!hasAuthority || eventData.button != PointerEventData.InputButton.Left) { return; }

            CmdSpawnUnit();
        }

        #endregion
    }
}
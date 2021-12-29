using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

using Mirror;

namespace RTS.Building
{
    public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
    {
        [SerializeField]
        private GameObject unitPrefab = null;
        [SerializeField]
        private Transform unitSpawnPoint = null;

        #region  Server
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
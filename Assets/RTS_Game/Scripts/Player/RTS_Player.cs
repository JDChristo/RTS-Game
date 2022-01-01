using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror;

using RTS.Building;

namespace RTS.Player
{
    public class RTS_Player : NetworkBehaviour
    {
        [SerializeField]
        private UnitBuilding[] buildings = new UnitBuilding[0];
        public List<Unit> MyUnits { get; private set; } = new List<Unit>();
        public List<UnitBuilding> MyBuilding { get; private set; } = new List<UnitBuilding>();

        #region Server
        public override void OnStartServer()
        {
            base.OnStartServer();
            Unit.ServerOnUnitSpawned += ServerUnitSpawned;
            Unit.ServerOnUnitDespawned += ServerUnitDespawned;

            UnitBuilding.ServerOnBuildingSpawned += ServerBuildingSpawned;
            UnitBuilding.ServerOnBuildingDespawned += ServerBuildingDespawned;
        }

        public override void OnStopServer()
        {
            base.OnStopServer();
            Unit.ServerOnUnitSpawned -= ServerUnitSpawned;
            Unit.ServerOnUnitDespawned -= ServerUnitDespawned;

            UnitBuilding.ServerOnBuildingSpawned -= ServerBuildingSpawned;
            UnitBuilding.ServerOnBuildingDespawned -= ServerBuildingDespawned;
        }
        [Command]
        public void CmdSpawnBuilding(int buildingID, Vector3 spawnPosition)
        {
            UnitBuilding buildingToPlace = null;

            foreach (UnitBuilding building in buildings)
            {
                if (building.ID == buildingID)
                {
                    buildingToPlace = building;
                    break;
                }
            }

            if (buildingToPlace == null) { return; }

            GameObject buildingIns = Instantiate(buildingToPlace.gameObject, spawnPosition, buildingToPlace.transform.rotation);
            NetworkServer.Spawn(buildingIns, connectionToClient);
        }

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

        private void ServerBuildingSpawned(UnitBuilding building)
        {
            if (building.connectionToClient.connectionId != connectionToClient.connectionId) { return; }
            MyBuilding.Add(building);
        }
        private void ServerBuildingDespawned(UnitBuilding building)
        {
            if (building.connectionToClient.connectionId != connectionToClient.connectionId) { return; }
            MyBuilding.Remove(building);
        }
        #endregion

        #region Client
        public override void OnStartAuthority()
        {
            base.OnStartClient();

            if (NetworkServer.active) { return; }
            Unit.AuthorityOnUnitSpawned += AuthorityUnitSpawned;
            Unit.AuthorityOnUnitDespawned += AuthorityUnitDespawned;

            UnitBuilding.AuthorityOnBuildingSpawned += AuthorityOnBuildingSpawned;
            UnitBuilding.AuthorityOnBuildingDespawned += AuthorityOnBuildingDespawned;
        }
        public override void OnStopClient()
        {
            base.OnStopClient();

            if (!isClientOnly || !hasAuthority) { return; }
            Unit.AuthorityOnUnitSpawned -= AuthorityUnitSpawned;
            Unit.AuthorityOnUnitDespawned -= AuthorityUnitDespawned;

            UnitBuilding.AuthorityOnBuildingSpawned -= AuthorityOnBuildingSpawned;
            UnitBuilding.AuthorityOnBuildingDespawned -= AuthorityOnBuildingDespawned;
        }
        #endregion

        private void AuthorityUnitSpawned(Unit unit)
        {
            MyUnits.Add(unit);
        }
        private void AuthorityUnitDespawned(Unit unit)
        {
            MyUnits.Remove(unit);
        }

        private void AuthorityOnBuildingSpawned(UnitBuilding building)
        {
            MyBuilding.Add(building);
        }
        private void AuthorityOnBuildingDespawned(UnitBuilding building)
        {
            MyBuilding.Remove(building);
        }
    }
}
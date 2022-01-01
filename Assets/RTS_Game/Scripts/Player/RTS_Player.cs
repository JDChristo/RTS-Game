using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror;

using RTS.Building;
using System;

namespace RTS.Player
{
    public class RTS_Player : NetworkBehaviour
    {
        [SerializeField]
        private float buildingRangeLimit = 5;
        [SerializeField]
        private LayerMask buildingBlockLayer;
        [SerializeField]
        private UnitBuilding[] buildings = new UnitBuilding[0];

        [SyncVar(hook = nameof(ClientResourcesUpdated))]
        private int resources = 200;

        private Color teamColor = new Color();
        public List<Unit> MyUnits { get; private set; } = new List<Unit>();
        public List<UnitBuilding> MyBuilding { get; private set; } = new List<UnitBuilding>();
        public event Action<int> ClientOnResourcesUpdated;
        public int Resources => resources;
        public Color TeamColor => teamColor;


        public bool CanPlaceBuilding(BoxCollider buildingCollider, Vector3 spawnPoint)
        {
            if (Physics.CheckBox(
                            spawnPoint + buildingCollider.center,
                             buildingCollider.size / 2,
                             Quaternion.identity,
                             buildingBlockLayer
                             ))
            {
                return false;
            }

            foreach (UnitBuilding building in MyBuilding)
            {
                if ((spawnPoint - building.transform.position).sqrMagnitude
                        <= buildingRangeLimit * buildingRangeLimit)
                {
                    return true;
                }
            }
            return false;
        }
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
        [Server]
        public void SetResources(int value)
        {
            resources = value;
        }
        [Server]
        public void SetTeamColor(Color color)
        {
            teamColor = color;
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

            if (resources < buildingToPlace.Price) { return; }

            BoxCollider buildingCollider = buildingToPlace.GetComponent<BoxCollider>();

            if (!CanPlaceBuilding(buildingCollider, spawnPosition)) { return; }

            GameObject buildingIns = Instantiate(buildingToPlace.gameObject, spawnPosition, buildingToPlace.transform.rotation);
            NetworkServer.Spawn(buildingIns, connectionToClient);

            SetResources(resources - buildingToPlace.Price);
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

        private void ClientResourcesUpdated(int oldResources, int newResources)
        {
            ClientOnResourcesUpdated?.Invoke(newResources);
        }
        #endregion
    }
}
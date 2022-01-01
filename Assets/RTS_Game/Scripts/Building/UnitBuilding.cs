using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

using RTS.Combat;

namespace RTS.Building
{
    public class UnitBuilding : NetworkBehaviour
    {
        [SerializeField] private GameObject buildingPreview;
        [SerializeField] private Sprite icon;
        [SerializeField] private int id = -1;
        [SerializeField] private int price = 100;

        public static event Action<UnitBuilding> ServerOnBuildingSpawned;
        public static event Action<UnitBuilding> ServerOnBuildingDespawned;

        public static event Action<UnitBuilding> AuthorityOnBuildingSpawned;
        public static event Action<UnitBuilding> AuthorityOnBuildingDespawned;

        public GameObject BuildingPreview => buildingPreview;
        public Sprite Icon => icon;
        public int ID => id;
        public int Price => price;

        #region Server
        public override void OnStartServer()
        {
            base.OnStartServer();
            ServerOnBuildingSpawned?.Invoke(this);
        }
        public override void OnStopServer()
        {
            base.OnStopServer();
            ServerOnBuildingDespawned?.Invoke(this);
        }
        #endregion

        #region Client
        public override void OnStartAuthority()
        {
            base.OnStartAuthority();
            AuthorityOnBuildingSpawned?.Invoke(this);
        }
        public override void OnStopClient()
        {
            base.OnStopClient();

            if (!hasAuthority) { return; }
            AuthorityOnBuildingDespawned?.Invoke(this);
        }
        #endregion

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

using RTS.Building;
using RTS.Player;

namespace RTS.Game
{
    public class GameOverHandler : NetworkBehaviour
    {
        public static event Action ServerOnGameOver;
        public static event Action<string> ClientOnGameOver;
        private List<UnitBase> bases = new List<UnitBase>();

        #region  Server
        public override void OnStartServer()
        {
            base.OnStartServer();
            UnitBase.ServerOnBaseSpawned += OnBaseSpawned;
            UnitBase.ServerOnBaseDespawned += OnBaseDespawned;
        }
        public override void OnStopServer()
        {
            base.OnStopServer();
            UnitBase.ServerOnBaseSpawned -= OnBaseSpawned;
            UnitBase.ServerOnBaseDespawned -= OnBaseDespawned;
        }
        [Server]
        public void OnBaseSpawned(UnitBase unitBase)
        {
            bases.Add(unitBase);
        }
        [Server]
        public void OnBaseDespawned(UnitBase unitBase)
        {
            bases.Remove(unitBase);
            if (bases.Count != 1) { return; }

            int playerID = bases[0].connectionToClient.connectionId;

            RpcGameOver($"Player {playerID}");

            ServerOnGameOver?.Invoke();
        }
        #endregion

        #region Client
        [ClientRpc]
        private void RpcGameOver(string winner)
        {
            ClientOnGameOver?.Invoke(winner);
        }
        #endregion
    }
}

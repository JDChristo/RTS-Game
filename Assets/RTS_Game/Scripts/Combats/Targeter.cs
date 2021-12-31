using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror;

using RTS.Game;

namespace RTS.Combat
{
    public class Targeter : NetworkBehaviour
    {
        [SerializeField]
        private Targetable target;

        public Targetable Target => target;

        #region  Server
        public override void OnStartServer()
        {
            base.OnStartServer();
            GameOverHandler.ServerOnGameOver += ServerOnGameOver;
        }

        public override void OnStopServer()
        {
            GameOverHandler.ServerOnGameOver -= ServerOnGameOver;
        }
        [Command]
        public void CmdSetTarget(GameObject targetGameObject)
        {
            if (!targetGameObject.TryGetComponent<Targetable>(out Targetable newTarget)) { return; }

            this.target = newTarget;
        }

        [Server]
        public void ClearTarget()
        {
            target = null;
        }
        [Server]
        private void ServerOnGameOver()
        {
            ClearTarget();
        }
        #endregion

    }
}

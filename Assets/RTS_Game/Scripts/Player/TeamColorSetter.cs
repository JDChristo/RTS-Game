using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror;

using RTS.Building;
using System;

namespace RTS.Player
{
    public class TeamColorSetter : NetworkBehaviour
    {
        [SerializeField] private Renderer[] colorRenderers = new Renderer[0];

        [SyncVar(hook = nameof(TeamColorUpdated))]
        private Color teamColor = new Color();

        #region  Server
        public override void OnStartServer()
        {
            base.OnStartServer();

            RTS_Player player = connectionToClient.identity.GetComponent<RTS_Player>();
            teamColor = player.TeamColor;
        }
        #endregion

        #region Client
        private void TeamColorUpdated(Color oldColor, Color newColor)
        {
            foreach (Renderer renderer in colorRenderers)
            {
                renderer.material.SetColor("_BaseColor", newColor);
            }
        }
        #endregion
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror;
using TMPro;
using System;

namespace RTS.Experimental
{
    public class RTS_Player : NetworkBehaviour
    {
        [SerializeField]
        private Renderer meshRenderer;
        [SerializeField]
        private TextMeshPro nameText;


        [SyncVar(hook = nameof(HandlePlayerName))]
        [SerializeField]
        private string displayName = "Name";

        [SyncVar(hook = nameof(HandlePlayerColor))]
        [SerializeField]
        private Color color = Color.white;

        #region Server
        [Command]
        private void CmdSetDisplayName(string newDisplayName)
        {
            if (newDisplayName.Length < 3)
            {
                return;
            }
            RpcLogNewName(newDisplayName);

            SetDisplayName(newDisplayName);
        }

        [Server]
        public void SetDisplayName(string name)
        {
            this.displayName = name;
        }

        [Server]
        public void SetColor(Color color)
        {
            this.color = color;
        }
        #endregion

        #region  Client
        private void HandlePlayerColor(Color old, Color newColor)
        {
            meshRenderer.material.SetColor("_BaseColor", newColor);
        }
        private void HandlePlayerName(string old, string newName)
        {
            nameText.text = newName;
        }
        [ContextMenu("Set My Name")]
        private void SetMyName()
        {
            CmdSetDisplayName("My New Name");
        }

        [ClientRpc]
        private void RpcLogNewName(string newDisplayName)
        {
            Debug.Log(newDisplayName);
        }


        #endregion

    }
}
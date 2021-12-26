using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror;
using TMPro;
using System;

namespace RTS.Network
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


        private void HandlePlayerColor(Color old, Color newColor)
        {
            meshRenderer.material.SetColor("_BaseColor", newColor);
        }
        private void HandlePlayerName(string old, string newName)
        {
            nameText.text = newName;
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
    }
}
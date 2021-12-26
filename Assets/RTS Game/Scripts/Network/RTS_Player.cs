using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror;

namespace RTS.Network
{
    public class RTS_Player : NetworkBehaviour
    {
        [SyncVar]
        [SerializeField]
        private string displayName = "Name";

        [SyncVar]
        [SerializeField]
        private Color color = Color.white;


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
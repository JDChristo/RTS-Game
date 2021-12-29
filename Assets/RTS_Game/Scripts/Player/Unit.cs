using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using Mirror;
namespace RTS.Player
{
    public class Unit : NetworkBehaviour
    {
        [SerializeField]
        private UnitMovement unitMovement;
        [SerializeField]
        private UnityEvent onSelected;

        [SerializeField]
        private UnityEvent onDeselected;

        public UnitMovement GetUnitMovement() => unitMovement;

        #region  Client
        [Client]
        public void Select()
        {
            if (!hasAuthority) { return; }
            onSelected?.Invoke();
        }

        [Client]
        public void Deselect()
        {
            if (!hasAuthority) { return; }
            onDeselected?.Invoke();
        }

        #endregion
    }
}
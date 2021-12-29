using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.AI;
using UnityEngine.InputSystem;

using Mirror;

namespace RTS.Player
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class UnitMovement : NetworkBehaviour
    {
        [SerializeField] private NavMeshAgent agent = null;

        private Camera mainCamera;

        #region Server

        [Command]
        private void CmdMove(Vector3 position)
        {
            if (NavMesh.SamplePosition(position, out NavMeshHit hit, 1f, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }
        }

        #endregion

        #region Client

        public override void OnStartAuthority()
        {
            mainCamera = Camera.main;
        }

        [ClientCallback]
        private void Update()
        {
            if (!hasAuthority || !Mouse.current.rightButton.wasPressedThisFrame) { return; }

            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {
                CmdMove(hit.point);
            }
        }

        #endregion
    }
}

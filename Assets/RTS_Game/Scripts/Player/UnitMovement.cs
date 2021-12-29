using UnityEngine;
using UnityEngine.AI;

using Mirror;

namespace RTS.Player
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class UnitMovement : NetworkBehaviour
    {
        [SerializeField] private NavMeshAgent agent = null;

        #region Server

        [Command]
        public void CmdMove(Vector3 position)
        {
            if (NavMesh.SamplePosition(position, out NavMeshHit hit, 1f, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }
        }

        #endregion

        #region Client

        #endregion
    }
}

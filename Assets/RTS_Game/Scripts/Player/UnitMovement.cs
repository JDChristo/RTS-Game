using UnityEngine;
using UnityEngine.AI;

using Mirror;

using RTS.Combat;

namespace RTS.Player
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class UnitMovement : NetworkBehaviour
    {
        [SerializeField]
        private NavMeshAgent agent = null;
        [SerializeField]
        private Targeter targeter;

        #region Server

        [ServerCallback]
        private void Update()
        {
            if (!agent.hasPath) { return; }
            if (agent.remainingDistance > agent.stoppingDistance) { return; }
            agent.ResetPath();
        }

        [Command]
        public void CmdMove(Vector3 position)
        {
            targeter.ClearTarget();

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

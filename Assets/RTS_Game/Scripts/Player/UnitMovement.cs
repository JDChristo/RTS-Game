using UnityEngine;
using UnityEngine.AI;

using Mirror;

using RTS.Game;
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
        [SerializeField]
        private float chaseRange = 10f;

        #region Server
        public override void OnStartServer()
        {
            base.OnStartServer();
            GameOverHandler.ServerOnGameOver += ServerOnGameOver;
        }

        public override void OnStopServer()
        {
            GameOverHandler.ServerOnGameOver -= ServerOnGameOver;
        }

        [ServerCallback]
        private void Update()
        {
            Targetable target = targeter.Target;

            if (target != null)
            {
                ChaseTarget(target);
                return;
            }
            if (!agent.hasPath) { return; }
            if (agent.remainingDistance > agent.stoppingDistance) { return; }

            agent.ResetPath();
        }

        [Command]
        public void CmdMove(Vector3 position)
        {
            ServerMove(position);
        }

        [Server]
        public void ServerMove(Vector3 position)
        {
            targeter.ClearTarget();

            if (NavMesh.SamplePosition(position, out NavMeshHit hit, 1f, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }
        }

        [Server]
        private void ServerOnGameOver()
        {
            agent.ResetPath();
        }
        #endregion

        #region Client
        private void ChaseTarget(Targetable target)
        {
            if ((target.transform.position - transform.position).sqrMagnitude > chaseRange * chaseRange)
            {
                agent.SetDestination(target.transform.position);
            }
            else if (agent.hasPath)
            {
                agent.ResetPath();
            }
        }
        #endregion
    }
}

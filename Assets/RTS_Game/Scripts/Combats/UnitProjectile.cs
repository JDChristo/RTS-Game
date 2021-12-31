using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
namespace RTS.Combat
{
    [RequireComponent(typeof(Rigidbody))]
    public class UnitProjectile : NetworkBehaviour
    {
        [SerializeField] private Rigidbody rb;
        [SerializeField] private int damageToDeal = 20;
        [SerializeField] private float lifeTime = 5f;
        [SerializeField] private float launchForce = 10f;

        void Start()
        {
            rb.velocity = transform.forward * launchForce;
        }

        [ServerCallback]
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<NetworkIdentity>(out NetworkIdentity identity))
            {
                if (identity.connectionToClient == connectionToClient) { return; }
            }

            if (other.TryGetComponent<Health>(out Health health))
            {
                health.DealDamage(damageToDeal);
            }
            DestorySelf();
        }

        public override void OnStartServer()
        {
            Invoke(nameof(DestorySelf), lifeTime);
        }

        [Server]
        private void DestorySelf()
        {
            NetworkServer.Destroy(gameObject);
        }

    }
}

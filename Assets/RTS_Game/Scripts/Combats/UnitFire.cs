using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace RTS.Combat
{
    public class UnitFire : NetworkBehaviour
    {
        [SerializeField]
        private Targeter targeter = null;
        [SerializeField]
        private GameObject projectilePrefab = null;
        [SerializeField]
        private Transform projectileSpawnPoint = null;
        [SerializeField]
        private float fireRange = 5f;
        [SerializeField]
        private float fireRate = 1f;
        [SerializeField]
        private float rotationSpeed = 20f;

        private float lastFireTime;

        [ServerCallback]
        private void Update()
        {
            if (targeter.Target == null) { return; }

            if (!CanFireAtTarget()) { return; }
            Quaternion targetRotation = Quaternion.LookRotation(targeter.Target.transform.position - transform.position);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            if (Time.time > (1 / fireRate) + lastFireTime)
            {
                Quaternion projectileRotation = Quaternion.LookRotation(targeter.Target.AimPoint.position - projectileSpawnPoint.position);
                GameObject projectileIns = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileRotation);
                NetworkServer.Spawn(projectileIns, connectionToClient);
                lastFireTime = Time.time;
            }
        }

        [Server]
        private bool CanFireAtTarget()
        {
            return (targeter.Target.transform.position - transform.position).sqrMagnitude <= fireRange * fireRange;
        }
    }
}
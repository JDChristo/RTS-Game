using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

using Mirror;

using RTS.Player;
using RTS.Combat;
using TMPro;
using UnityEngine.UI;

namespace RTS.Building
{
    public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
    {
        [SerializeField]
        private Health health = null;
        [SerializeField]
        private Unit unitPrefab = null;
        [SerializeField]
        private Transform unitSpawnPoint = null;
        [SerializeField]
        private TMP_Text remainingUnitText = null;
        [SerializeField]
        private Image unitProgressImage = null;
        [SerializeField]
        private int maxUnitQueue = 5;
        [SerializeField]
        private float spawnMoveRange = 5f;
        [SerializeField]
        private float unitSpawnDuration = 5f;

        [SyncVar(hook = nameof(ClientHandleQueuedUnits))]
        private int queuedUnits;
        [SyncVar]
        private float unitTimer;
        private float progressImageVelocity;

        private void Update()
        {
            if (isServer)
            {
                ProduceUnits();
            }

            if (isClient)
            {
                UpdateTimerDisplay();
            }
        }


        #region  Server
        public override void OnStartServer()
        {
            base.OnStartServer();
            health.ServerOnDie += Die;
        }
        public override void OnStopServer()
        {
            base.OnStopServer();
            health.ServerOnDie -= Die;
        }
        [Server]
        private void Die()
        {
            NetworkServer.Destroy(gameObject);
        }
        [Command]
        private void CmdSpawnUnit()
        {
            if (queuedUnits == maxUnitQueue) { return; }
            RTS_Player player = connectionToClient.identity.GetComponent<RTS_Player>();

            if (player.Resources < unitPrefab.ResourceCost) { return; }
            queuedUnits++;
            player.SetResources(player.Resources - unitPrefab.ResourceCost);
        }
        [Server]
        private void ProduceUnits()
        {
            if (queuedUnits == 0) { return; }

            unitTimer += Time.deltaTime;

            if (unitTimer < unitSpawnDuration) { return; }

            GameObject unit = Instantiate(unitPrefab.gameObject, unitSpawnPoint.position, unitSpawnPoint.rotation);
            NetworkServer.Spawn(unit, connectionToClient);

            Vector3 offset = Random.insideUnitSphere * spawnMoveRange;
            offset.y = unitSpawnPoint.position.y;

            UnitMovement unitMovement = unit.GetComponent<UnitMovement>();
            unitMovement.ServerMove(unitSpawnPoint.position + offset);

            queuedUnits--;
            unitTimer = 0;
        }
        #endregion

        #region  Client
        public void OnPointerClick(PointerEventData eventData)
        {
            if (!hasAuthority || eventData.button != PointerEventData.InputButton.Left) { return; }

            CmdSpawnUnit();
        }

        private void ClientHandleQueuedUnits(int oldCount, int newCount)
        {
            remainingUnitText.text = newCount.ToString();
        }
        private void UpdateTimerDisplay()
        {
            float progress = unitTimer / unitSpawnDuration;
            if (progress < unitProgressImage.fillAmount)
            {
                unitProgressImage.fillAmount = progress;
            }
            else
            {
                unitProgressImage.fillAmount = Mathf.SmoothDamp(
                    unitProgressImage.fillAmount,
                    progress,
                    ref progressImageVelocity,
                    0.1f
                );
            }
        }

        #endregion
    }
}
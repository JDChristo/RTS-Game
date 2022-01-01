using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

using RTS.Combat;
using RTS.Player;
using RTS.Game;

namespace RTS.Building
{
    public class ResourceGenerator : NetworkBehaviour
    {
        [SerializeField] private Health health;
        [SerializeField] private int resourcesPerInterval = 10;
        [SerializeField] private float interval = 2F;

        private float timer;
        private RTS_Player player;

        [ServerCallback]
        private void Update()
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                player.SetResources(player.Resources + resourcesPerInterval);
                timer += interval;
            }
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            timer = interval;
            player = connectionToClient.identity.GetComponent<RTS_Player>();

            health.ServerOnDie += ServerDie;
            GameOverHandler.ServerOnGameOver += ServerGameOver;
        }
        public override void OnStopServer()
        {
            base.OnStopServer();
            health.ServerOnDie -= ServerDie;
            GameOverHandler.ServerOnGameOver -= ServerGameOver;
        }

        private void ServerGameOver()
        {
            enabled = false;
        }

        private void ServerDie()
        {
            NetworkServer.Destroy(gameObject);
        }
    }
}

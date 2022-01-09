using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Mirror;

using RTS.Game;
using RTS.Player;

namespace RTS.Network
{
    public class RTS_NetworkManager : NetworkManager
    {
        [Header("NEW SLOT")]
        [SerializeField]
        private GameObject unitSpawnerPrefab = null;
        [SerializeField]
        private GameOverHandler gameOverHandlerPrefab = null;

        public static event Action ClientOnConnected;
        public static event Action ClientOnDisconnected;

        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);
            ClientOnConnected?.Invoke();
        }
        public override void OnClientDisconnect(NetworkConnection conn)
        {
            base.OnClientDisconnect(conn);
            ClientOnDisconnected?.Invoke();
        }
        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            base.OnServerAddPlayer(conn);
            RTS_Player player = conn.identity.GetComponent<RTS_Player>();
            player.SetTeamColor(new Color(
               UnityEngine.Random.value,
               UnityEngine.Random.value,
                UnityEngine.Random.value
            ));

        }
        public override void OnServerSceneChanged(string newSceneName)
        {
            base.OnServerChangeScene(newSceneName);
            if (SceneManager.GetActiveScene().name.StartsWith("Game"))
            {
                GameOverHandler handler = Instantiate(gameOverHandlerPrefab);
                NetworkServer.Spawn(handler.gameObject);
            }
        }
    }
}

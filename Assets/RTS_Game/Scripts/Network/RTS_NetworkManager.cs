using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Mirror;

using RTS.Game;

namespace RTS.Network
{
    public class RTS_NetworkManager : NetworkManager
    {
        [Header("NEW SLOT")]
        [SerializeField]
        private GameObject unitSpawnerPrefab = null;
        [SerializeField]
        private GameOverHandler gameOverHandlerPrefab = null;
        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            base.OnServerAddPlayer(conn);

            GameObject spawner = Instantiate(unitSpawnerPrefab, conn.identity.transform.position, conn.identity.transform.rotation);
            NetworkServer.Spawn(spawner, conn);
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

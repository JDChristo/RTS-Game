using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTS.Game;

using TMPro;
using Mirror;

namespace RTS.UI
{
    public class GameOverPopUp : MonoBehaviour
    {
        [SerializeField] private GameObject popUp = null;
        [SerializeField] private TMP_Text winnerNameText = null;

        private void Start()
        {
            GameOverHandler.ClientOnGameOver += ClientGameOver;
        }
        private void OnDestroy()
        {
            GameOverHandler.ClientOnGameOver -= ClientGameOver;
        }

        private void ClientGameOver(string winner)
        {
            winnerNameText.text = $"{winner} Has Won !!";
            popUp.SetActive(true);
        }

        public void LeaveGame()
        {
            if (NetworkServer.active && NetworkClient.isConnected)
            {
                NetworkManager.singleton.StopHost();
            }
            else
            {
                NetworkManager.singleton.StopClient();
            }
        }
    }
}

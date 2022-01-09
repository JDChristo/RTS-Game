using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyMenu : MonoBehaviour
{
    [SerializeField] private GameObject lobbyUI = null;
    private void OnEnable()
    {
        RTS.Network.RTS_NetworkManager.ClientOnConnected += HandleClientConnected;
        RTS.Network.RTS_NetworkManager.ClientOnDisconnected += HandleClientDisconnected;
    }
    private void OnDisable()
    {
        RTS.Network.RTS_NetworkManager.ClientOnConnected -= HandleClientConnected;
        RTS.Network.RTS_NetworkManager.ClientOnDisconnected -= HandleClientDisconnected;
    }
    private void HandleClientConnected()
    {
        lobbyUI.SetActive(true);
    }
    private void HandleClientDisconnected()
    {

    }

    public void LeaveLobby()
    {
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
        }
        else
        {
            NetworkManager.singleton.StopClient();
            SceneManager.LoadScene(0);
        }


    }
}

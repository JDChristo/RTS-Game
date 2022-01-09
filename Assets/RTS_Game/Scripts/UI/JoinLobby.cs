using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using Mirror;

public class JoinLobby : MonoBehaviour
{
    [SerializeField] private GameObject landingPage = null;
    [SerializeField] private TMP_InputField addressInput = null;
    [SerializeField] private Button joinBtn = null;

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
    public void Join()
    {
        string address = addressInput.text;

        NetworkManager.singleton.networkAddress = address;
        NetworkManager.singleton.StartClient();

        joinBtn.interactable = false;
    }

    private void HandleClientConnected()
    {
        joinBtn.interactable = true;

        gameObject.SetActive(false);
        landingPage.SetActive(false);
    }
    private void HandleClientDisconnected()
    {
        joinBtn.interactable = true;
    }
}

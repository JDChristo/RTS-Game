using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

using RTS.Player;
using Mirror;

namespace RTS.Game.Resources
{
    public class ResourcesDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text resourceText;
        private RTS_Player player;
        void Start()
        {

        }
        private void OnDestroy()
        {
            player.ClientOnResourcesUpdated -= ClientOnResourcesUpdated;
        }

        private void Update()
        {
            if (player == null)
            {
                player = NetworkClient.connection.identity.GetComponent<RTS_Player>();
                if (player != null)
                {
                    ClientOnResourcesUpdated(player.Resources);
                    player.ClientOnResourcesUpdated += ClientOnResourcesUpdated;
                }
            }
        }

        private void ClientOnResourcesUpdated(int resources)
        {
            resourceText.text = $"Resources: {resources}";
        }
    }
}

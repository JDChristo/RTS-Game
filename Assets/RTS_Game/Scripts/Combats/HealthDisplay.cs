using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RTS.Combat
{
    public class HealthDisplay : MonoBehaviour
    {
        [SerializeField] private Health health;
        [SerializeField] private GameObject healthParent;
        [SerializeField] private Image healthBarImage;

        private void Awake()
        {
            health.ClientOnHealthChange += HealthUpdated;
        }
        private void OnDestroy()
        {
            health.ClientOnHealthChange -= HealthUpdated;
        }

        private void OnMouseEnter()
        {
            healthParent.SetActive(true);
        }

        private void OnMouseExit()
        {
            healthParent.SetActive(false);
        }
        private void HealthUpdated(int currentHealth, int maxHealth)
        {
            healthBarImage.fillAmount = (float)currentHealth / maxHealth;
        }
    }
}

using System;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

namespace CroakCreek
{
    public class CoinCollection : MonoBehaviour
    {
        private int Coin = 0;

        [SerializeField] HealthManager healthManager;
        [SerializeField] StaminaManager staminaManager;
        [SerializeField] PanelEventHandler levelUp;
        [SerializeField] TextMeshProUGUI coinText;
        [SerializeField] PlayerController playerController;
        DisableMovement freeze;

        private void Awake()
        {
            freeze = GetComponent<DisableMovement>();
            levelUp.gameObject.SetActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("Coin"))
            {
                Coin++;
                coinText.text = "Coins: " + Coin;
                Debug.Log("Coins: " + Coin);
                Destroy(other.gameObject);

                if (Coin % 3 == 0)
                {
                    levelUp.gameObject.SetActive(true);
                    Coin = 0;
                }
            }
        }

        public void IncreaseMaxHealth()
        {
            healthManager.maxHp += 5;
            healthManager.HealFull();
            levelUp.gameObject.SetActive(false);
            Debug.Log("Max HP increased to " + healthManager.maxHp);
        }

        public void IncreaseMaxStamina()
        {
            staminaManager.maxSta += 3;
            levelUp.gameObject.SetActive(false);
            Debug.Log("Max Stamina increased to " + staminaManager.maxSta);
        }
    }
}

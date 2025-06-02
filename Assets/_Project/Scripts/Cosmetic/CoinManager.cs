using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace CroakCreek
{
    public class CoinManager : MonoBehaviour
    {
        [SerializeField] private float rotationSpeed = 85f;

        private Quaternion rotation;
        private List<Coin> coins;

        #region Singleton
        public static CoinManager Instance;

        private void Awake()
        {
            Instance = this;
            rotation = Quaternion.identity;
            coins = new List<Coin>();
        }

        #endregion

        private void Update()
        {
            rotation *= Quaternion.Euler(0f, rotationSpeed * Time.deltaTime, 0f);
            foreach (Coin coin in coins)
            {
                coin.transform.rotation = rotation;
            }
        }

        public void Register(Coin coin)
        {
            if (!coins.Contains(coin))
            {
                coins.Add(coin);
            }
        }

        public void Unregister(Coin coin)
        {
            if (coins.Contains(coin))
            {
                coins.Remove(coin);
            }
        }
    }
}

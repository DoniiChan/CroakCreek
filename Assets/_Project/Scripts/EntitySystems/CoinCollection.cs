using TMPro;
using UnityEngine;

namespace CroakCreek
{
    public class CoinCollection : MonoBehaviour
    {
        private int Coin = 0;

        public TextMeshProUGUI coinText;

        private void OnTriggerEnter(Collider other)
        {
            if(other.transform.tag == "Coin")
            {
                Coin++;
                coinText.text = "Coins: " + Coin.ToString();
                Debug.Log(Coin);
                Destroy(other.gameObject);
            }
        }
    }
}

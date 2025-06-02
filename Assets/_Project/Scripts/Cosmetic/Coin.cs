using UnityEngine;

namespace CroakCreek
{
    public class Coin : MonoBehaviour
    {
        private void OnBecameVisible()
        {
            CoinManager.Instance.Register(this);
        }

        private void OnBecameInvisible()
        {
            CoinManager.Instance.Unregister(this);
        }
    }
}

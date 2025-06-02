using UnityEngine;

namespace CroakCreek
{
    public class StaminaManager : MonoBehaviour
    {
        [SerializeField] public int maxSta = 10;
        [SerializeField] public int currentSta = 0;

        private void Awake() => currentSta = maxSta;

        public void DecreaseStamina(int amount)
        {
            currentSta = Mathf.Max(0, currentSta - amount);
        }

        public void RestoreStamina(int amount)
        {
            currentSta = Mathf.Min(maxSta, currentSta + amount);
        }
    }
}
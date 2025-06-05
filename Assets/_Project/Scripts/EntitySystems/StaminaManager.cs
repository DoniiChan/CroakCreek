using UnityEngine;
using UnityEngine.Events;

namespace CroakCreek
{
    public class StaminaManager : MonoBehaviour, IBarValueSource
    {
        [SerializeField] public int maxSta = 10;
        [SerializeField] public int currentSta = 0;

        public UnityEvent<int> StaminaUsed = new UnityEvent<int>();
        public UnityEvent<int> StaminaRestored = new UnityEvent<int>();

        // IBarValueSource implementation
        public int currentValue => currentSta;
        public int maxValue => maxSta;
        public UnityEvent<int> OnValueDecreased => StaminaUsed;
        public UnityEvent<int> OnValueIncreased => StaminaRestored;

        private void Awake() => currentSta = maxSta;

        public void DecreaseStamina(int amount)
        {
            int newSta = Mathf.Max(0, currentSta - amount);
            if (newSta < currentSta)
            {
                currentSta = newSta;
                StaminaUsed?.Invoke(currentSta);
            }
        }

        public void RestoreStamina(int amount)
        {
            int newSta = Mathf.Min(maxSta, currentSta + amount);
            if (newSta > currentSta)
            {
                currentSta = newSta;
                StaminaRestored?.Invoke(currentSta);
            }
        }

        public void RestoreFull() => RestoreStamina(maxSta);
    }
}

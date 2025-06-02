using UnityEngine;
using UnityEngine.Events;

namespace CroakCreek
{
    public class HealthManager : MonoBehaviour
    {
        [SerializeField] public int maxHp = 20;
        [SerializeField] public int currentHp = 0;

        public int Hp
        {
            get => currentHp;
            private set
            {
                var isDamage = value < currentHp;
                currentHp = Mathf.Clamp(value, 0, maxHp);
                if (isDamage)
                {
                    Damaged?.Invoke(currentHp);
                }
                else
                {
                    Healed?.Invoke(currentHp);
                }

                if (currentHp <= 0)
                {
                    Died?.Invoke();
                }
            }
        }

        public UnityEvent<int> Damaged;
        public UnityEvent<int> Healed;
        public UnityEvent Died;

        private void Awake()
        {
            currentHp = maxHp;
        }

        public void Damage(int amount) => Hp -= amount;

        public void Heal(int amount) => Hp += amount;

        public void HealFull() => Hp = maxHp;

        public void Kill() => Hp = 0;

        public void Adjust(int value) => Hp = value;
    }
}

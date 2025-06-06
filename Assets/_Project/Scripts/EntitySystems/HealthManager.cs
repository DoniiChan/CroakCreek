using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace CroakCreek
{
    public class HealthManager : MonoBehaviour, IBarValueSource
    {
        [SerializeField] public int maxHp = 20;
        [SerializeField] public int currentHp = 0;
        [SerializeField] PanelEventHandler gameOverScreen;
        [SerializeField] PlayerController playerController;
        [SerializeField] public bool isFrog;
        DisableMovement freeze;

        public UnityEvent<int> Damaged;
        public UnityEvent<int> Healed;
        public UnityEvent Died;

        void Start()
        {
            freeze = GetComponent<DisableMovement>();
            currentHp = maxHp;  // initialize at max health
        }
        public int currentValue => currentHp;
        public int maxValue => maxHp;
        public UnityEvent<int> OnValueIncreased => Healed;
        public UnityEvent<int> OnValueDecreased => Damaged;

        private void Awake() => currentHp = maxHp;

        public void Damage(int amount) => SetHp(currentHp - amount);
        public void Heal(int amount) => SetHp(currentHp + amount);
        public void HealFull() => SetHp(maxHp);
        public void Adjust(int value) => SetHp(value);

        private bool _isSettingHp = false;

        private void SetHp(int value)
        {
            if (_isSettingHp) return;  // Prevent recursion
            _isSettingHp = true;

            Debug.Log($"SetHp called. currentHp={currentHp}, value(before clamp)={value}");

            value = Mathf.Clamp(value, 0, maxHp);
            if (value == currentHp)
            {
                _isSettingHp = false;
                return;
            }

            bool isDamage = value < currentHp;
            currentHp = value;

            if (isDamage)
                Damaged?.Invoke(currentHp);
            else
                Healed?.Invoke(currentHp);

            if (currentHp <= 0)
                Died?.Invoke();

            _isSettingHp = false;
        }

        public void Kill()
        {
            SetHp(0);

            if (isFrog)
            {
                Destroy(gameObject);
            }
            else
            {
                freeze.Freeze();

                gameOverScreen.gameObject.SetActive(true);
            }
        }


        public void Respawn()
        {
            playerController.enabled = true;

            freeze.UnFreeze();

            HealFull();
            gameOverScreen.gameObject.SetActive(false);
        }

        public void Quit()
        {
            #if UNITY_STANDALONE
                        Application.Quit();
                        gameOverScreen.gameObject.SetActive(false);
            #endif
            #if UNITY_EDITOR
                        UnityEditor.EditorApplication.isPlaying = false;
                        gameOverScreen.gameObject.SetActive(false);
            #endif
        }
    }
}

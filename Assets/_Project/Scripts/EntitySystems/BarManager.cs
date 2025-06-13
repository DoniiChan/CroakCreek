using UnityEngine;
using UnityEngine.UI;

namespace CroakCreek
{
    public class BarManager : MonoBehaviour
    {
        [SerializeField] HealthManager healthManager;
        [SerializeField] StaminaManager staminaManager;
        [SerializeField] RectTransform barFill;
        [SerializeField] RectMask2D barMask;
        [SerializeField] RectTransform border;
        [SerializeField] float pixelsPerUnit = 10f;
        [SerializeField] float smoothSpeed = 5f;

        [SerializeField] Transform target;
        [SerializeField] Vector3 offset;

        private IBarValueSource barSource;
        private float fullWidth;
        private float currentDisplayPercentage;
        private float targetPercentage;

        private void Awake()
        {
            if (healthManager != null)
                barSource = healthManager;
            else if (staminaManager != null)
                barSource = staminaManager;
            else
            {
                Debug.LogError("BarManager requires either a HealthManager or StaminaManager assigned.");
                enabled = false;
                return;
            }

            barSource.OnValueIncreased.AddListener(SetValue);
            barSource.OnValueDecreased.AddListener(SetValue);
        }

        private void Start()
        {
            ResizeBar();
            SetValue(barSource.currentValue);
            currentDisplayPercentage = targetPercentage;
        }

        private void Update()
        {
            if (barSource.maxValue * pixelsPerUnit != fullWidth)
            {
                ResizeBar();
                SetValue(barSource.currentValue);
            }

            // Update bar fill smoothly
            currentDisplayPercentage = Mathf.Lerp(currentDisplayPercentage, targetPercentage, Time.deltaTime * smoothSpeed);
            UpdateBarVisual(currentDisplayPercentage);

            if (healthManager != null && healthManager.isFrog && target != null)
            {
                transform.position = Camera.main.WorldToScreenPoint(target.position + offset);
            }
        }

        private void ResizeBar()
        {
            fullWidth = (target != null) ? 200 : barSource.maxValue * pixelsPerUnit;

            Vector2 fillSize = barFill.sizeDelta;
            fillSize.x = fullWidth - 2f;
            barFill.sizeDelta = fillSize;

            RectTransform maskRect = barMask.GetComponent<RectTransform>();
            Vector2 maskSize = maskRect.sizeDelta;
            maskSize.x = fullWidth;
            maskRect.sizeDelta = maskSize;

            Vector2 borderSize = border.sizeDelta;
            borderSize.x = fullWidth;
            border.sizeDelta = borderSize;

            var p = barMask.padding;
            p.z = 0;
            barMask.padding = p;
        }

        public void SetValue(int newValue)
        {
            targetPercentage = Mathf.Clamp01((float)newValue / barSource.maxValue);
        }

        private void UpdateBarVisual(float percentage)
        {
            RectTransform maskRect = barMask.GetComponent<RectTransform>();
            Vector2 maskSize = maskRect.sizeDelta;
            maskSize.x = fullWidth * percentage;
            maskRect.sizeDelta = maskSize;
        }
    }
}

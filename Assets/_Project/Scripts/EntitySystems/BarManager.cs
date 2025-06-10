using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace CroakCreek
{
    public class BarManager : MonoBehaviour
    {
        [SerializeField] HealthManager healthManager;
        [SerializeField] StaminaManager staminaManager;
        [SerializeField] RectTransform barFill;            // Fixed width fill image
        [SerializeField] RectMask2D barMask;               // Mask that clips the fill
        [SerializeField] RectTransform border;             // Optional border image
        [SerializeField] float pixelsPerUnit = 10f;        // Width per unit (HP or Stamina)

        [SerializeField] Transform target;
        [SerializeField] Vector3 offset;

        private IBarValueSource barSource;
        private float fullWidth;

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
        }

        private void Update()
        {
            if (barSource.maxValue * pixelsPerUnit != fullWidth)
            {
                ResizeBar();
                SetValue(barSource.currentValue);
            }

            if (healthManager != null && healthManager.isFrog && target != null)
            {
                transform.position = Camera.main.WorldToScreenPoint(target.position + offset);
            }
        }

        private void ResizeBar()
        {
            fullWidth = (target != null) ? 200 : barSource.maxValue * pixelsPerUnit;

            Vector2 fillSize = barFill.sizeDelta;
            fillSize.x = fullWidth -2f;
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
            float percentage = Mathf.Clamp01((float)newValue / barSource.maxValue);

            // Apply percentage to mask width (controls visible portion)
            RectTransform maskRect = barMask.GetComponent<RectTransform>();
            Vector2 maskSize = maskRect.sizeDelta;
            maskSize.x = fullWidth * percentage;
            maskRect.sizeDelta = maskSize;
        }

    }
}

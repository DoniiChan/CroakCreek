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
        }

        private void ResizeBar()
        {
            fullWidth = barSource.maxValue * pixelsPerUnit;

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
            float visibleWidth = (float)newValue / barSource.maxValue * fullWidth;
            float clipAmount = Mathf.Clamp(fullWidth - visibleWidth, 0, fullWidth);

            var padding = barMask.padding;
            padding.z = clipAmount;
            barMask.padding = padding;
        }
    }
}

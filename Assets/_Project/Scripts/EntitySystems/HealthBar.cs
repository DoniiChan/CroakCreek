using UnityEngine;
using UnityEngine.UI;

namespace CroakCreek
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] HealthManager hp;
        [SerializeField] RectTransform barFill;       // Fixed width fill image
        [SerializeField] RectMask2D barMask;          // Mask that clips the fill
        [SerializeField] RectTransform border;        // Optional border image
        [SerializeField] float pixelsPerHp = 10f;     // Width per HP

        private float fullWidth;

        private void Start()
        {
            hp.Damaged.AddListener(SetValue);
            hp.Healed.AddListener(SetValue);

            ResizeBar();
            SetValue(hp.currentHp);
        }

        private void Update()
        {
            // Resize if maxHp changed at runtime
            if (hp.maxHp * pixelsPerHp != fullWidth)
            {
                ResizeBar();
                SetValue(hp.currentHp);
            }
        }

        private void ResizeBar()
        {
            fullWidth = hp.maxHp * pixelsPerHp;

            // Set barFill width fixed to full width
            Vector2 fillSize = barFill.sizeDelta;
            fillSize.x = fullWidth - 5f;
            barFill.sizeDelta = fillSize;

            // Set barMask size to fullWidth (same size as fill)
            RectTransform maskRect = barMask.GetComponent<RectTransform>();
            Vector2 maskSize = maskRect.sizeDelta;
            maskSize.x = fullWidth;
            maskRect.sizeDelta = maskSize;

            Vector2 borderSize = border.sizeDelta;
            borderSize.x = fullWidth;
            border.sizeDelta = borderSize;

            // Reset padding so no clipping initially
            var p = barMask.padding;
            p.z = 0; // right padding = 0 means fully visible
            barMask.padding = p;
        }

        public void SetValue(int newValue)
        {
            // Calculate how much of the right side to mask (clip)
            float visibleWidth = (float)newValue / hp.maxHp * fullWidth;
            float clipAmount = fullWidth - visibleWidth;

            // Clamp clip amount (padding.z is right padding)
            clipAmount = Mathf.Clamp(clipAmount, 0, fullWidth);

            var padding = barMask.padding;
            padding.z = clipAmount;   // Increase right padding to mask more
            barMask.padding = padding;
        }
    }
}

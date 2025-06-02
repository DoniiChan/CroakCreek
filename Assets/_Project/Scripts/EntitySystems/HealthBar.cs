using UnityEngine;
using UnityEngine.UI;

namespace CroakCreek
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] HealthManager hp;
        [SerializeField] RectTransform barRect;
        [SerializeField] RectMask2D mask;

        private float maxRightMask;
        private float initialRightMask;

        private void Start()
        {
            maxRightMask = barRect.rect.width - mask.padding.x - mask.padding.z;
            initialRightMask = mask.padding.z;
        }

        public void SetValue(int newValue)
        {
            var targetWidth = newValue * maxRightMask / hp.maxHp;
            var newRightMask = maxRightMask + initialRightMask - targetWidth;
            var padding = mask.padding;
            padding.z = newRightMask;
            mask.padding = padding;
        }
    }
}

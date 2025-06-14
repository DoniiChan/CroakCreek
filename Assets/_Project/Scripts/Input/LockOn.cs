using UnityEngine;

namespace CroakCreek
{
    public class LockOn : MonoBehaviour
    {
        [SerializeField] private float range;
        [SerializeField] private RectTransform lockIndicator;

        private Transform target = null;
        private Enemy targetEnemy;
        private Vector3 offset = new Vector3(0, 2, 0);

        private void Start()
        {
            lockIndicator.gameObject.SetActive(false);
        }

        public void ToggleLock()
        {
            if (target != null)
            {
                SetTarget(null); // clear the current lock
            }
            else
            {
                LockNearestEnemy(); // acquire new target
            }
        }

        public void LockNearestEnemy()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, range);
            Enemy nearest = null;
            float closestDistanceSqr = Mathf.Infinity;

            foreach (Collider collider in colliders)
            {
                if (collider.TryGetComponent<Enemy>(out Enemy enemy))
                {
                    float distSqr = (enemy.transform.position - transform.position).sqrMagnitude;
                    if (distSqr < closestDistanceSqr)
                    {
                        closestDistanceSqr = distSqr;
                        nearest = enemy;
                    }
                }
            }

            SetTarget(nearest);
        }

        public void SetTarget(Enemy targetEnemy)
        {
            this.targetEnemy = targetEnemy;

            if (targetEnemy != null)
            {
                target = targetEnemy.transform;
                lockIndicator.gameObject.SetActive(true);
                lockIndicator.position = Camera.main.WorldToScreenPoint(target.position + offset);
                Debug.Log("Targeting: " + targetEnemy.name);
            }
            else
            {
                target = null;
                lockIndicator.gameObject.SetActive(false);
            }
        }

        private void Update()
        {
            if (target != null)
            {
                lockIndicator.position = Camera.main.WorldToScreenPoint(target.position + offset);
            }
        }
    }
}

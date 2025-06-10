using Unity.VisualScripting;
using UnityEngine;

namespace CroakCreek
{
    public class TargetingSystem : MonoBehaviour
    {
        [SerializeField] private float range;

        PlayerController pc;

        private LockOn target;

        private void Start()
        {
            target = GetComponent<LockOn>();
        }

        private void Update()
        {
            Collider[] colliderArray = Physics.OverlapSphere(transform.position, range);
            foreach (Collider collider in colliderArray)
                if (collider.TryGetComponent<Enemy>(out Enemy enemy))
                {
                    target.SetTarget(enemy);
                }
        }
    }
}

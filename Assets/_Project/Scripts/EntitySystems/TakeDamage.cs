using UnityEngine;
using System.Collections.Generic;

namespace CroakCreek
{
    public class TakeDamage : MonoBehaviour
    {
        [SerializeField] public int damage;
        private HashSet<Collider> damagedTargets = new();

        private void OnTriggerEnter(Collider other)
        {
            if (damagedTargets.Contains(other)) return;

            HealthManager healthManager = other.GetComponent<HealthManager>();
            if (healthManager != null)
            {
                healthManager.Damage(damage);
                damagedTargets.Add(other);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            // Allow re-damaging when they exit and re-enter
            damagedTargets.Remove(other);
        }
    }
}

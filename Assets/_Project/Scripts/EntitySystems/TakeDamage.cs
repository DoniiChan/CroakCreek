using UnityEngine;

namespace CroakCreek
{
    public class TakeDamage : MonoBehaviour
    {
        [SerializeField] public int damage;

        private void OnTriggerEnter(Collider other)
        {
            HealthManager healthManager = other.GetComponent<HealthManager>();

            if (healthManager != null)
            {
                healthManager.Damage(damage);
            }
        }
    }
}

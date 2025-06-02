using UnityEngine;

namespace CroakCreek
{
    public class TakeDamage : MonoBehaviour
    {
        [SerializeField] public int damage;
        [SerializeField] HealthManager healthManager;

        private void OnTriggerEnter(Collider other)
        {
            healthManager.Damage(damage);
        }
    }
}

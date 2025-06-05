using UnityEngine;

namespace CroakCreek
{
    public class TakeDamage : MonoBehaviour
    {
        [SerializeField] public int damage;

        private void OnTriggerEnter(Collider other)
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            HealthManager healthManager = other.GetComponent<HealthManager>();
            IHitable hitable = other.GetComponent<IHitable>();

            if (healthManager && rb != null)
            {
                healthManager.Damage(damage);

                if (hitable != null)
                {
                    hitable.Excecute(transform);
                }
            }
        }
    }
}

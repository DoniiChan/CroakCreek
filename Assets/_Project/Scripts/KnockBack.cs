using UnityEngine;

namespace CroakCreek
{
    public class KnockBack : MonoBehaviour, IHitable
    {
        private Rigidbody rb;
        [SerializeField] float knockBackMultiplier;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        public void Excecute(Transform excecutionSource)
        {
            KnockBackEntity(excecutionSource);
        }

        private void KnockBackEntity(Transform executionSource)
        {
            //Vector3 dir = (transform.position - executionSource.position).normalized;
            ////rb.AddForce((dir * knockBackMultiplier + Vector3.up).normalized * knockBackMultiplier, ForceMode.Impulse);
            //rb.AddForce((dir * knockBackMultiplier + Vector3.up).normalized * knockBackMultiplier, ForceMode.Force);
            rb.AddForce(((transform.position - executionSource.position).normalized + Vector3.up * 0.3f).normalized * knockBackMultiplier * 0.5f, ForceMode.Impulse);

        }
    }
}

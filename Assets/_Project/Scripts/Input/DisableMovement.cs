using UnityEngine;

namespace CroakCreek
{
    public class DisableMovement : MonoBehaviour
    {
        Rigidbody rb;
        GroundChecker gc;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            gc = GetComponent<GroundChecker>();
        }

        public void Freeze()
        {
            rb.constraints = RigidbodyConstraints.FreezePosition;
        }

        public void UnFreeze()
        {
            rb.constraints = RigidbodyConstraints.FreezeRotation;

            if (!gc.IsGrounded)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            }
        }
    }
}

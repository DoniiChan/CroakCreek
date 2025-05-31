using UnityEngine;

namespace CroakCreek
{
    public class GroundChecker : MonoBehaviour
    {
        [SerializeField] float groundDistance = 0.25f;
        [SerializeField] LayerMask groundLayers;

        public bool IsGrounded {  get; private set; }

        private void Update()
        {
            IsGrounded = Physics.SphereCast(transform.position, groundDistance, Vector3.down, out _, groundDistance, groundLayers);
        }
    }
}

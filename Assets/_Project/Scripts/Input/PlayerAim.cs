using UnityEngine;
using UnityEngine.InputSystem;

namespace CroakCreek
{
    public class PlayerAim : MonoBehaviour
    {
        [SerializeField] private Transform player; // Reference to player body or center
        [SerializeField] private float maxAimDistance = 100f;

        public Vector3 AimDirection { get; private set; }
        public Vector3 AimPoint { get; private set; }

        private Camera mainCamera;

        private void Awake()
        {
            mainCamera = Camera.main;
        }

        private void Update()
        {
            UpdateAimDirection();

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                DetectClick();
            }
        }

        private void UpdateAimDirection()
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Ray ray = mainCamera.ScreenPointToRay(mousePos);

            if (Physics.Raycast(ray, out RaycastHit hitInfo, maxAimDistance))
            {
                AimPoint = hitInfo.point;

                AimDirection = (AimPoint - player.position).normalized;

                Debug.DrawLine(player.position, AimPoint, Color.red);
            }
        }

        private void DetectClick()
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Ray ray = mainCamera.ScreenPointToRay(mousePos);

            if (Physics.Raycast(ray, out RaycastHit hitInfo, maxAimDistance))
            {
                Debug.Log($"Clicked on: {hitInfo.collider.gameObject.name}");
            }
            else
            {
                Debug.Log("Clicked on empty space.");
            }
        }
    }
}

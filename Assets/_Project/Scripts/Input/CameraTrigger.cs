using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    public OrbitalCameraManager cameraManager;
    public float desiredAngle = 90f; // You can set this in the inspector

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            cameraManager.SetCameraSide(desiredAngle);
        }
    }
}

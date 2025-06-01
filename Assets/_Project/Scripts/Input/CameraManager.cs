using UnityEngine;
using Unity.Cinemachine;

public class OrbitalCameraManager : MonoBehaviour
{
    public CinemachineCamera virtualCamera;
    public Transform followTarget;
    public float orbitRadius = 5f;
    public float cameraHeight = 2f;
    public float rotationSpeed = 5f;

    private float targetAngle;
    private float currentAngle;

    void Start()
    {
        if (virtualCamera == null || followTarget == null)
        {
            Debug.LogError("Missing camera or follow target reference.");
        }

        currentAngle = 0f;
        targetAngle = 0f;
    }

    void Update()
    {
        // Smoothly rotate towards target angle
        currentAngle = Mathf.LerpAngle(currentAngle, targetAngle, Time.deltaTime * rotationSpeed);

        // Convert angle to position
        Vector3 offset = Quaternion.Euler(0f, currentAngle, 0f) * Vector3.forward * orbitRadius;
        Vector3 desiredPosition = followTarget.position - offset;
        desiredPosition.y = followTarget.position.y + cameraHeight;

        virtualCamera.transform.position = desiredPosition;
        virtualCamera.transform.LookAt(followTarget.position);
    }

    public void SetCameraSide(float angle)
    {
        targetAngle = angle;
    }
}

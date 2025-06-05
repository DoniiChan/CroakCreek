using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class FrogWander : MonoBehaviour
{
    public float hopRange = 5f;
    public float hopForce = 5f;
    public float hopDelay = 2f;
    public Transform centrePoint;

    private bool isHopping = false;
    private Rigidbody rb;
    private NavMeshHit hit;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (centrePoint == null) centrePoint = transform;
        StartCoroutine(HopRoutine());
    }

    IEnumerator HopRoutine()
    {
        while (true)
        {
            if (!isHopping)
            {
                Vector3 point;
                if (RandomPoint(centrePoint.position, hopRange, out point))
                {
                    Vector3 jumpDir = (point - transform.position);
                    jumpDir.y = 0; // flatten to horizontal

                    Vector3 force = jumpDir.normalized * hopForce;
                    force.y = hopForce; // upward force for hop

                    rb.AddForce(force, ForceMode.VelocityChange);
                    isHopping = true;

                    yield return new WaitForSeconds(1.0f); // Wait for the hop to peak and land

                    isHopping = false;
                }
            }

            yield return new WaitForSeconds(hopDelay);
        }
    }

    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        for (int i = 0; i < 10; i++) // try a few times
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * range;
            if (NavMesh.SamplePosition(randomPoint, out hit, 2.0f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }

        result = Vector3.zero;
        return false;
    }
}

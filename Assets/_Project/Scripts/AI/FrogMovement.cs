using CroakCreek;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
public class FrogMovement : MonoBehaviour
{
    private Rigidbody rb;

    [Header("Hop Settings")]
    public float hopForce = 5f;
    public float hopCooldown = 2f;
    public float extraGravityMultiplier = 2f;

    [Header("Wandering (Optional)")]
    public bool enableWandering = false;
    public float wanderRange = 5f;
    public Transform wanderCenter;

    [Header("Follow Player")]
    public bool followPlayer = false;
    public Transform playerTarget;
    public float followRange = 10f;

    private bool isHopping = false;
    private float Zerof = 0f;
    private float lastHopTime = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (wanderCenter == null) wanderCenter = transform;
    }

    void Start()
    {
        if (enableWandering || followPlayer)
            StartCoroutine(BehaviorRoutine());
    }

    void Update()
    {
        if (isHopping && !IsGrounded())
        {
            rb.AddForce(Vector3.down * Physics.gravity.magnitude * (extraGravityMultiplier - 1f), ForceMode.Acceleration);
        }
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }

    public void HopTo(Vector3 targetPosition)
    {
        if (Time.time < lastHopTime + hopCooldown || isHopping)
            return;

        StartCoroutine(Hop(targetPosition));
    }

    private IEnumerator Hop(Vector3 targetPosition)
    {
        isHopping = true;

        Vector3 direction = targetPosition - transform.position;
        direction.y = 0;

        Vector3 force = direction.normalized * hopForce;
        force.y = hopForce * 1.2f;

        rb.AddForce(force, ForceMode.Impulse);

        yield return new WaitForSeconds(1f);

        isHopping = false;
        lastHopTime = Time.time;
    }

    private IEnumerator BehaviorRoutine()
    {
        while (enableWandering || followPlayer)
        {
            if (!isHopping)
            {
                if (followPlayer && playerTarget != null)
                {
                    float distance = Vector3.Distance(transform.position, playerTarget.position);
                    if (distance < followRange)
                    {
                        hopCooldown = Zerof;
                        HopTo(playerTarget.position);
                    }
                }
                else if (enableWandering)
                {
                    hopCooldown = 2;
                    if (GetRandomNavPoint(wanderCenter.position, wanderRange, out Vector3 point))
                        HopTo(point);
                }
            }

            yield return new WaitForSeconds(hopCooldown);
        }
    }

    private bool GetRandomNavPoint(Vector3 center, float range, out Vector3 result)
    {
        for (int i = 0; i < 10; i++)
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * range;
            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 2.0f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }

        result = Vector3.zero;
        return false;
    }
}

using CroakCreek;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NetScript : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private AnimationCurve throwVelocity;
    [SerializeField] private AnimationCurve returnVelocity;

    public float throwSpeed = 20f;
    public float maxRange = 5f;
    public float netCooldown = 0.5f;
    public float throwDuration = 0.5f;
    public float throwTime;

    public float returnSpeed = 20f;
    public float returnDuration = 0.5f;
    public float returnTimer;
    public Vector3 returnStartPosition;

    private Vector3 moveDirection;
    private Vector3 startPosition;
    private Transform playerTransform;

    private bool isMoving = false;
    private bool returning = false;
    private bool isCoolingDown = false;

    void Awake()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider>().enabled = false;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;

        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        transform.position = playerTransform.position;
    }

    private void Update()
    {
        if (isMoving || returning)
        {
            HandleThrow();
        }
        else if (!isMoving)
        {
            transform.position = playerTransform.position;
        }
    }

    public void Throw(Vector3 direction)
    {
        if (!isMoving)
        {
            EnableObject();
            throwSpeed = throwVelocity.Evaluate(0);
            throwTime = 0f;
            moveDirection = direction.normalized;
            startPosition = transform.position;
            isMoving = true;
            returning = false;
            returnStartPosition = transform.position; // Ensure return origin is set
        }
    }

    private void HandleThrow()
    {
        if (!returning)
        {
            throwTime += Time.deltaTime;
            throwSpeed = throwVelocity.Evaluate(throwTime / throwDuration); // Normalize over duration
            transform.position += moveDirection * throwSpeed * Time.deltaTime;

            if (Vector3.Distance(startPosition, transform.position) >= maxRange)
            {
                returning = true;
                returnTimer = 0f;
                returnStartPosition = transform.position;
            }
        }
        else
        {
            GetComponent<Collider>().enabled = false;
            returnTimer += Time.deltaTime;
            float t = Mathf.Clamp01(returnTimer / returnDuration);
            returnSpeed = returnVelocity.Evaluate(t);
            transform.position = Vector3.Lerp(returnStartPosition, playerTransform.position, returnSpeed);

            if (t >= 1f)
            {
                isMoving = false;
                returning = false;
                GetComponent<SpriteRenderer>().enabled = false;
                StartCooldown(netCooldown);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wild"))
        {
            returning = true;
            returnTimer = 0f;
            returnStartPosition = transform.position;
        }
    }

    public bool IsCoolingDown() => isCoolingDown;

    public void StartCooldown(float duration)
    {
        isCoolingDown = true;
        Invoke(nameof(EndCooldown), duration);
    }

    private void EndCooldown()
    {
        isCoolingDown = false;
    }

    void EnableObject()
    {
        GetComponent<SpriteRenderer>().enabled = true;
        GetComponent<Collider>().enabled = true;
    }
}
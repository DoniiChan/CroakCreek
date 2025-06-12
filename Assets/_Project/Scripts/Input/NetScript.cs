using CroakCreek;
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
    private float throwDuration = 0.5f;
    public float throwTime;

    private float returnSpeed = 20f;
    private float returnDuration = 0.5f;
    public float returnTimer;
    private Vector3 returnStartPosition;

    private Vector3 moveDirection;
    private Vector3 startPosition;
    private Transform playerTransform;

    private bool isMoving = false;
    private bool returning = false;
    private bool isCoolingDown = false;

    void Awake()
    {
        DisableObject();
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
            throwTime = 0f;
            throwSpeed = throwVelocity.Evaluate(throwTime);
            moveDirection = direction.normalized;
            startPosition = transform.position;
            isMoving = true;
            returning = false;
        }
    }

    private void HandleThrow()
    {

        if (!returning)
        {
            throwTime += Time.deltaTime;

            throwSpeed = throwVelocity.Evaluate(throwTime);

            transform.position += moveDirection * throwSpeed * Time.deltaTime;
            //transform.position = Vector3.Lerp(playerTransform.position, moveDirection, throwTime);

            if (Vector3.Distance(startPosition, transform.position) >= maxRange)
            {
                returning = true;
                returnTimer = 0f;
            }
        }
        else // if (returning)
        {
            returnTimer += Time.deltaTime;
            float returnTime = Mathf.Clamp01(returnTimer / returnDuration);

            //returnSpeed = returnVelocity.Evaluate(returnTime);

            transform.position = Vector3.Lerp(transform.position, playerTransform.position, returnTime);

            if (returnTime >= 1f)
            {
                isMoving = false;
                returning = false;
                DisableObject();
                StartCooldown(netCooldown);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (returning) return;

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

    void DisableObject()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider>().enabled = false;
    }
}
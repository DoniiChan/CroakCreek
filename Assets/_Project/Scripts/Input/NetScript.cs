using UnityEngine;

public class NetScript : MonoBehaviour
{
    public float speed = 20f;
    public float maxRange = 5f;

    private Vector3 moveDirection;
    private Vector3 startPosition;
    private Transform playerTransform;
    private bool returning = false;
    private bool isActive = false;

    void Awake()
    {
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
    }

    public void Spawn(Vector3 direction)
    {
        if (isActive) return; // Prevent starting mid-flight again

        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        moveDirection = direction.normalized;
        startPosition = transform.position;
        returning = false;
        isActive = true;
    }

    private void Update()
    {
        if (!isActive) return;

        if (!returning)
        {
            transform.position += moveDirection * speed * Time.deltaTime;

            if (Vector3.Distance(startPosition, transform.position) >= maxRange)
            {
                returning = true;
            }
        }
        else
        {
            Vector3 returnDirection = (playerTransform.position - transform.position).normalized;
            transform.position += returnDirection * speed * Time.deltaTime;

            if (Vector3.Distance(transform.position, playerTransform.position) < 0.5f)
            {
                isActive = false;
                // Freeze net in place (optional):
                // rb.velocity = Vector3.zero;
                gameObject.SetActive(false); // Optional if pooling
            }
        }
    }
}

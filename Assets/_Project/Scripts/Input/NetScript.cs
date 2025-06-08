using UnityEngine;

public class NetScript : MonoBehaviour
{
    public float speed = 10f;
    private Vector3 moveDirection;

    public void Spawn(Vector3 direction)
    {
        moveDirection = direction.normalized;
        transform.rotation = Quaternion.LookRotation(moveDirection);
        Destroy(gameObject, 5f); // optional lifetime
    }

    private void Update()
    {
        transform.position += moveDirection * speed * Time.deltaTime;
    }
}

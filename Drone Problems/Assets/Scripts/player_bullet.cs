using UnityEngine;

public class player_bullet : MonoBehaviour
{
    [Header("Movement")]
    public float lifeTime = 2f;

    private Vector3 moveDirection;
    private float moveSpeed;
    private bool isReady = false;

    public void SetOwner(Transform owner)
    {
        // Kept so other scripts calling SetOwner do not break
    }

    public void SetDirection(Vector3 direction, float speed, float bulletLifeTime)
    {
        moveDirection = direction.normalized;
        moveSpeed = speed;
        lifeTime = bulletLifeTime;
        isReady = true;

        if (moveDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(moveDirection);
        }

        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        if (!isReady)
        {
            return;
        }

        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }
}
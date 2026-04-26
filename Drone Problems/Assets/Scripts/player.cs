using UnityEngine;
using UnityEngine.SceneManagement;

public class player : MonoBehaviour
{
    [Header("Attributes")]
    public float Speed = 5f;
    public float rotationSpeed = 5f;

    public shooting shootingScript;
    public int max_bullets = 1000000;
    public int current_bullets = 0;

    [Header("Health")]
    public int maxHealth = 100;
    public int health = 100;

    public float bulletSpeed = 20f;
    public float fireInterval = 0.2f;

    private float nextShootTime = 0f;

    CharacterController cc;

    [Header("Gravity")]
    public float gravityAccel = -9.81f;
    public Transform groundCheck;
    public LayerMask groundLayer;
    Vector3 gravity_velocity;
    public float jumppower = 100f;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
    }

    void Start()
    {
        gravity_velocity = new Vector3(0, -2, 0);
        current_bullets = max_bullets;
        health = maxHealth;
    }

    void Update()
    {
        ApplyGravity();
    }

    public void ApplyGravity()
    {
        if (onGround() && gravity_velocity.y < 0)
        {
            gravity_velocity.y = -2f;
        }

        gravity_velocity.y += gravityAccel * Time.deltaTime;
        cc.Move(gravity_velocity * Time.deltaTime);
    }

    public bool onGround()
    {
        if (groundCheck == null)
        {
            return false;
        }

        Collider[] colliders = Physics.OverlapSphere(
            groundCheck.position,
            0.3f,
            groundLayer,
            QueryTriggerInteraction.Ignore
        );

        return colliders.Length > 0;
    }

    public void Move(Vector3 direction)
    {
        if (direction == Vector3.zero)
        {
            return;
        }

        direction = direction.normalized;
        cc.Move(direction * Speed * Time.deltaTime);
    }

    public void shoot()
    {
        if (shootingScript == null)
        {
            return;
        }

        if (Time.time < nextShootTime)
        {
            return;
        }

        if (current_bullets <= 0)
        {
            return;
        }

        shootingScript.Shoot();
        current_bullets--;
        nextShootTime = Time.time + fireInterval;
    }

    public void Jump()
    {
        if (onGround())
        {
            gravity_velocity = new Vector3(0, jumppower * 2, 0);
        }
        else
        {
            gravity_velocity = new Vector3(0, jumppower, 0);
        }
    }

    public void TakeDamage(int damageAmount)
    {
        health -= damageAmount;

        Debug.Log("Player took " + damageAmount + " damage. Health: " + health);

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Player died.");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
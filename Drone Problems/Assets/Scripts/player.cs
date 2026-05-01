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

    [Header("Coins")]
    public int coins = 0;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip coinPickupSound;
    public AudioClip bulletPickupSound;
    public AudioClip shootingSound;

    public float bulletSpeed = 20f;
    public float fireInterval = 0.2f;

    private float nextShootTime = 0f;

    CharacterController cc;

    [Header("Gravity")]
    public float gravityAccel = -9.81f;
    public Transform groundCheck;
    public LayerMask groundLayer;
    Vector3 gravity_velocity;
    public float jumppower = 10f;

    [Header("Jumping")]
    public int extraJumpsAllowed = 1;
    private int extraJumpsUsed = 0;

    void Awake()
    {
        cc = GetComponent<CharacterController>();

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    void Start()
    {
        gravity_velocity = new Vector3(0, -2, 0);
        current_bullets = max_bullets;
        health = maxHealth;
        coins = 0;
    }

    void Update()
    {
        ApplyGravity();

        if (onGround())
        {
            extraJumpsUsed = 0;
        }
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
            shootingScript = FindFirstObjectByType<shooting>();

            if (shootingScript == null)
            {
                Debug.LogWarning("No shooting script found. Assign Main Camera to Shooting Script on Player.");
                return;
            }
        }

        if (Time.time < nextShootTime)
        {
            return;
        }

        if (current_bullets <= 0)
        {
            Debug.Log("Out of bullets.");
            return;
        }

        shootingScript.Shoot();

        if (audioSource != null && shootingSound != null)
        {
            audioSource.PlayOneShot(shootingSound);
        }

        current_bullets--;
        nextShootTime = Time.time + fireInterval;
    }

    public void Jump()
    {
        if (onGround())
        {
            gravity_velocity.y = jumppower;
            extraJumpsUsed = 0;
        }
        else if (extraJumpsUsed < extraJumpsAllowed)
        {
            gravity_velocity.y = jumppower;
            extraJumpsUsed++;
        }
    }

    public void TakeDamage(int damageAmount)
    {
        health -= damageAmount;

        if (health < 0)
        {
            health = 0;
        }

        Debug.Log("Player took " + damageAmount + " damage. Health: " + health);

        if (health <= 0)
        {
            Die();
        }
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        Debug.Log("Coins: " + coins);

        if (audioSource != null && coinPickupSound != null)
        {
            audioSource.PlayOneShot(coinPickupSound);
        }
    }

    public void AddBullets(int amount)
    {
        if (audioSource != null && bulletPickupSound != null)
        {
            audioSource.PlayOneShot(bulletPickupSound);
        }

        current_bullets += amount;

        if (current_bullets > max_bullets)
        {
            current_bullets = max_bullets;
        }

        Debug.Log("Bullets: " + current_bullets);
    }

    void Die()
    {
    GameStats.finalCoins = coins;
    GameStats.finalBullets = current_bullets;
    GameStats.timeSurvived = Time.timeSinceLevelLoad;

    GameStats.SaveBestCoinsIfNeeded(coins);

    SceneManager.LoadScene("DeathScene");
    }
}
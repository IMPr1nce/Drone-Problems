using UnityEngine;
using UnityEngine.SceneManagement;
public class player : MonoBehaviour
{
    public float Speed = 5f;
    public float rotationSpeed = 5f; 
    public float gravityAccel = -9.81f;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public shooting shootingScript;
    public int max_bullets = 1000000;
    public int current_bullets = 0;

    public float bulletSpeed = 20f;
    public float fireInterval = 0.2f;

    private float nextShootTime = 0f;

    
    CharacterController cc;
    

    [Header("Gravity")]
    Vector3 gravity_velocity;
    public float jumppower = 100f;

    public void ApplyGravity()
    {
        if (onGround() && gravity_velocity.y < 0 )
        {

            gravity_velocity.y = -2f; // Small negative value to keep the player grounded
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

        Collider[] colliders = Physics.OverlapSphere(groundCheck.position, 0.3f, groundLayer, QueryTriggerInteraction.Ignore);
        return colliders.Length > 0;
    
        
    }

    void Awake(){
        cc = GetComponent<CharacterController>();
    
    }
    void Start()
    {
        gravity_velocity = new Vector3(0,-2,0);
        current_bullets = max_bullets;
    }
    void Update()
    {
        ApplyGravity();
    }

    public void Move(Vector3 direction)
    {
        if (direction == Vector3.zero)
            return;
        

        direction = direction.normalized;
        cc.Move(direction * Speed * Time.deltaTime);

        
        
    }
    
    public void shoot()
    {
        if (shootingScript != null)
        {
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
    }

    public void Jump()
    {   
        if (onGround()){
            gravity_velocity = new Vector3(0,jumppower*2,0);
        }
        else
        {
            gravity_velocity = new Vector3(0,jumppower,0);
        }
    }


    
    private void OnTriggerEnter(Collider other)
    {
    
    }


    
}

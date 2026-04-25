using UnityEngine;

public class shooting : MonoBehaviour
{
    public GameObject bulletPrefab; 
    public Transform firePoint;
    public Transform GameObject;
    public float bulletSpeed = 200f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Shoot()
    {
        // Instantiate a bullet at the position and rotation of the shooting object
        GameObject newBullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        newBullet.GetComponent<Rigidbody>().linearVelocity = firePoint.forward * bulletSpeed;
        Destroy(newBullet, 10); 
    }

    
}

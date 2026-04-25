using UnityEngine;

public class target_follow : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Transform target;
    float xRotation = 0f;
    float yRotation = 0f;
    float xSpeed = 100f;
    float ySpeed = 100f;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = target.position;
        // transform.rotation = target.rotation;
    }

    public void rotate(float xDelta, float yDelta)
    {
        xDelta *= xSpeed * Time.deltaTime;
        yDelta *= ySpeed * Time.deltaTime;

        yRotation += xDelta;
        xRotation -= yDelta; 
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);
        target.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);   

    }
    

}
using UnityEngine;

public class first_person_camera : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    float xRotation = 0f;
    float yRotation = 0f;
    float xSpeed = 100f;
    float ySpeed = 100f;
    public Transform cameraTransform;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void rotate(float xDelta, float yDelta)
    {
        xDelta *= xSpeed * Time.deltaTime;
        yDelta *= ySpeed * Time.deltaTime;

        yRotation += xDelta;
        xRotation -= yDelta; 
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);   

    }
}

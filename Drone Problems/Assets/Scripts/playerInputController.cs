using UnityEngine;
using UnityEngine.InputSystem;

public class playerInputController : MonoBehaviour
{
    public player playerMovement;
    public first_person_camera playerCamera;
    public target_follow playerShooting;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 tempMovement = Vector3.zero;

        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
        {
            tempMovement.z += 1;
        }
        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
        {
            tempMovement.z -= 1;
        }

        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
        {
            tempMovement.x -= 1;
        }
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
        {
            tempMovement.x += 1;
        }
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            playerMovement.Jump();
        }
        if (Mouse.current.leftButton.isPressed)
        {
            playerMovement.shoot();
        }
        tempMovement =  playerCamera.cameraTransform.TransformDirection(tempMovement);
        tempMovement.y = 0; // Keep movement on the horizontal plane
        playerMovement.Move(tempMovement);
        

        //Camera control
        playerCamera.rotate(Mouse.current.delta.x.value, Mouse.current.delta.y.value);
        playerShooting.rotate(Mouse.current.delta.x.value, Mouse.current.delta.y.value);

    }
}

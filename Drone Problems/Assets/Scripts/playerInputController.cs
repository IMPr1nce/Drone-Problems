using UnityEngine;
using UnityEngine.InputSystem;

public class playerInputController : MonoBehaviour
{
    public player playerMovement;
    public first_person_camera playerCamera;

    void Start()
    {
        if (playerMovement == null)
        {
            playerMovement = FindFirstObjectByType<player>();
        }

        if (playerCamera == null)
        {
            playerCamera = FindFirstObjectByType<first_person_camera>();
        }
    }

    void Update()
    {
        if (playerMovement == null || playerCamera == null)
        {
            return;
        }

        if (Keyboard.current == null || Mouse.current == null)
        {
            return;
        }

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

        tempMovement = playerCamera.cameraTransform.TransformDirection(tempMovement);
        tempMovement.y = 0;

        playerMovement.Move(tempMovement);

        playerCamera.rotate(
            Mouse.current.delta.x.value,
            Mouse.current.delta.y.value
        );
    }
}
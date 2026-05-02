using UnityEngine;
using UnityEngine.InputSystem;

public class PS5ControllerInput : MonoBehaviour
{
    [Header("References")]
    public player playerScript;

    [Header("Camera References")]
    public Transform cameraPivot;
    public Transform pitchCamera;
    public Transform playerBody;

    [Header("Controller Look Settings")]
    public float controllerHorizontalSensitivity = 120f;
    public float controllerVerticalSensitivity = 120f;
    public bool invertControllerYLook = false;
    public float stickDeadzone = 0.1f;

    [Header("Mouse / Trackpad Look Settings")]
    public float mouseHorizontalSensitivity = 0.12f;
    public float mouseVerticalSensitivity = 0.12f;
    public bool invertMouseYLook = false;

    [Header("Vertical Look Limits")]
    public float minLookAngle = -80f;
    public float maxLookAngle = 80f;

    private float xRotation = 0f;
    private Vector2 controllerLookInput = Vector2.zero;
    private Vector2 mouseLookInput = Vector2.zero;

    void Awake()
    {
        if (playerScript == null)
        {
            playerScript = GetComponent<player>();
        }

        if (playerBody == null)
        {
            playerBody = transform;
        }

        if (pitchCamera == null && Camera.main != null)
        {
            pitchCamera = Camera.main.transform;
        }

        if (pitchCamera != null)
        {
            xRotation = NormalizeAngle(pitchCamera.localEulerAngles.x);
        }
    }

    void Update()
    {
        ReadMouseLookInput();

        Gamepad gamepad = Gamepad.current;

        if (gamepad != null && playerScript != null)
        {
            HandleControllerMovement(gamepad);
            ReadControllerLookInput(gamepad);
            HandleControllerJump(gamepad);
            HandleControllerShoot(gamepad);
        }
        else
        {
            controllerLookInput = Vector2.zero;
        }
    }

    void LateUpdate()
    {
        HandleLook();
    }

    private void HandleControllerMovement(Gamepad gamepad)
    {
        Vector2 moveInput = gamepad.leftStick.ReadValue();

        Transform moveReference = cameraPivot != null ? cameraPivot : playerBody;

        Vector3 forward = moveReference.forward;
        Vector3 right = moveReference.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = forward * moveInput.y + right * moveInput.x;

        playerScript.Move(moveDirection);
    }

    private void ReadControllerLookInput(Gamepad gamepad)
    {
        controllerLookInput = gamepad.rightStick.ReadValue();

        if (Mathf.Abs(controllerLookInput.x) < stickDeadzone)
        {
            controllerLookInput.x = 0f;
        }

        if (Mathf.Abs(controllerLookInput.y) < stickDeadzone)
        {
            controllerLookInput.y = 0f;
        }
    }

    private void ReadMouseLookInput()
    {
        mouseLookInput = Vector2.zero;

        if (Mouse.current != null)
        {
            mouseLookInput = Mouse.current.delta.ReadValue();
        }
    }

    private void HandleLook()
    {
        float controllerLookX = controllerLookInput.x * controllerHorizontalSensitivity * Time.deltaTime;
        float controllerLookY = controllerLookInput.y * controllerVerticalSensitivity * Time.deltaTime;

        float mouseLookX = mouseLookInput.x * mouseHorizontalSensitivity;
        float mouseLookY = mouseLookInput.y * mouseVerticalSensitivity;

        float totalLookX = controllerLookX + mouseLookX;

        float totalLookY = 0f;

        if (invertControllerYLook)
        {
            totalLookY += controllerLookY;
        }
        else
        {
            totalLookY -= controllerLookY;
        }

        if (invertMouseYLook)
        {
            totalLookY += mouseLookY;
        }
        else
        {
            totalLookY -= mouseLookY;
        }

        // Horizontal 360 rotation
        if (playerBody != null)
        {
            playerBody.Rotate(Vector3.up * totalLookX, Space.World);
        }

        // If camera_pivot is separate from Player, rotate it too
        if (cameraPivot != null && playerBody != null && !cameraPivot.IsChildOf(playerBody))
        {
            cameraPivot.Rotate(Vector3.up * totalLookX, Space.World);
        }

        // Vertical aiming
        xRotation += totalLookY;
        xRotation = Mathf.Clamp(xRotation, minLookAngle, maxLookAngle);

        if (pitchCamera != null)
        {
            pitchCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }
    }

    private void HandleControllerJump(Gamepad gamepad)
    {
        if (gamepad.buttonSouth.wasPressedThisFrame)
        {
            playerScript.Jump();
        }
    }

    private void HandleControllerShoot(Gamepad gamepad)
    {
        if (gamepad.rightTrigger.isPressed)
        {
            playerScript.shoot();
        }
    }

    private float NormalizeAngle(float angle)
    {
        if (angle > 180f)
        {
            angle -= 360f;
        }

        return angle;
    }
}
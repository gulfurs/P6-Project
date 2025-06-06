using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float lookSensitivity = 2f;
    public Transform cameraTransform;

    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction lookAction;
    private Rigidbody rb;

    private float verticalRotation = 0f;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        lookAction = playerInput.actions["Look"];
        rb = GetComponent<Rigidbody>();

        rb.freezeRotation = true; // Prevents Rigidbody from rotating due to physics

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        LookPlayer();
    }

    void FixedUpdate() // Use FixedUpdate for Rigidbody movement
    {
        MovePlayer();
    }

    void MovePlayer()
    {
        Vector2 direction = moveAction.ReadValue<Vector2>();
        Vector3 moveVector = transform.right * direction.x + transform.forward * direction.y;
        
        rb.linearVelocity = new Vector3(moveVector.x * speed, rb.linearVelocity.y, moveVector.z * speed);
    }

    void LookPlayer()
    {
        Vector2 look = lookAction.ReadValue<Vector2>();

        float mouseX = look.x * lookSensitivity;
        float mouseY = look.y * lookSensitivity;

        transform.Rotate(Vector3.up * mouseX);
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }
}

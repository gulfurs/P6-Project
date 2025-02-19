using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [Header("Player")]
    public float moveSpeed = 4.0f;
    public float sprintSpeed = 6.0f;
    public float crouchSpeed = 2.0f;
    public float jumpHeight = 1.2f;
    public float gravity = -15.0f;

    [Header("Camera")]
    public Transform cameraTransform;
    public float cameraSensitivity = 1.0f;
    public float maxCameraAngle = 85f;

    [Header("Ground Check")]
    public float groundCheckDistance = 0.4f;
    public LayerMask groundLayer;
    public bool isGrounded;

    // Components & References
    private CharacterController controller;
    private StarterAssetsInputs input;

    // Movement & Camera
    private Vector3 velocity;
    private float cameraPitch = 0f;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        input = GetComponent<StarterAssetsInputs>();

         if (input == null)
    {
        Debug.LogError("StarterAssetsInputs is missing from " + gameObject.name);
    }
    }

    private void Update()
    {
        JumpAndGravity();
        Move();
        CameraRotation();
    }

    // Handle jumping and gravity
    private void JumpAndGravity()
    {
        isGrounded = Physics.CheckSphere(transform.position, groundCheckDistance, groundLayer);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Small force to keep grounded
        }

        if (input.jump && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    // Handle player movement
    private void Move()
    {
        float speed = input.sprint ? sprintSpeed : (input.crouch ? crouchSpeed : moveSpeed);

        Vector3 move = (transform.right * input.move.x + transform.forward * input.move.y).normalized;
        controller.Move(move * speed * Time.deltaTime);
    }

    // Handle camera rotation
    private void CameraRotation()
    {
        if (input.look.sqrMagnitude >= 0.01f)
        {
            float mouseX = input.look.x * cameraSensitivity * Time.deltaTime;
            float mouseY = input.look.y * cameraSensitivity * Time.deltaTime;

            // Rotate the camera up and down (pitch)
            cameraPitch -= mouseY;
            cameraPitch = Mathf.Clamp(cameraPitch, -maxCameraAngle, maxCameraAngle);
            cameraTransform.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);

            // Rotate the player left and right (yaw)
            transform.Rotate(Vector3.up * mouseX);
        }
    }
}

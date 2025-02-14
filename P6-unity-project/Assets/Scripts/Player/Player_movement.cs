using UnityEngine;
using UnityEngine.InputSystem;

public class Player_movement : MonoBehaviour
{
    public float speed = 5f;
    public float lookSenstivity = 2f;

    public Transform cameraTransform;


    PlayerInput playerInput;
    InputAction moveAction;
    InputAction lookAction;
    CharacterController characterController;

    private float verticalRotation = 0f;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        lookAction = playerInput.actions["Look"];
        characterController = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        MovePlayer();
        LookPlayer();
    }

    void MovePlayer()
    {

        Vector2 direction = moveAction.ReadValue<Vector2>();
        
        Vector3 moveVector = transform.right * direction.x + transform.forward * direction.y; 
        characterController.Move(moveVector * speed * Time.deltaTime);
       
        //Vector3 moveVector = new Vector3(direction.x, 0, direction.y) * speed * Time.deltaTime;
        //characterController.Move(moveVector);
    }

    void LookPlayer()
    {
        Vector2 look = lookAction.ReadValue<Vector2>();

        float mouseX = look.x * lookSenstivity;
        float mouseY = look.y * lookSenstivity;

        transform.Rotate(Vector3.up * mouseX);
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }
}

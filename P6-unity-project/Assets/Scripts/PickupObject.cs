using UnityEngine;

public class PickupObject : MonoBehaviour
{
    [Header("Pickup Settings")]
    public float moveSpeed = 10f; // Speed at which the object moves
    public Vector3 holdOffset = new Vector3(0, 0, 1.5f); // Offset in front of the camera

    private Transform cameraTransform; // Player's camera transform
    private Vector3 originalPosition; // Original position of the object
    private Quaternion originalRotation; // Original rotation of the object
    private bool isHolding = false; // Whether the object is currently in front of the camera

    void Start()
    {
        // Find the player's camera (assumes it's tagged as "MainCamera")
        cameraTransform = Camera.main.transform;

        // Save the original position and rotation of the object
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    void Update()
    {
        // Smoothly move the object to its target position
        if (isHolding)
        {
            // Move the object in front of the camera
            Vector3 targetPosition = cameraTransform.position + cameraTransform.forward * holdOffset.z + cameraTransform.up * holdOffset.y;
            Quaternion targetRotation = Quaternion.LookRotation(cameraTransform.forward, cameraTransform.up);

            transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, moveSpeed * Time.deltaTime);
        }
        else
        {
            // Return the object to its original position
            transform.position = Vector3.Lerp(transform.position, originalPosition, moveSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, originalRotation, moveSpeed * Time.deltaTime);
        }
    }

    // Toggle the object's hold state
    public void ToggleHold()
    {
        isHolding = !isHolding;
    }
}
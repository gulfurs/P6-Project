using UnityEngine;

public class FitVideoToCamera : MonoBehaviour
{
    private Camera mainCamera;
    public float distanceFromCamera = 5f; // Distance to keep video in front of the camera

    void Start()
    {
        mainCamera = Camera.main; // Automatically find the main camera
        AdjustVideoScreen();
    }

    void AdjustVideoScreen()
    {
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera not found!");
            return;
        }

        // Position the video **directly in front of the camera**
        transform.position = mainCamera.transform.position + mainCamera.transform.forward * distanceFromCamera;

        // Make the video face the camera
        transform.LookAt(mainCamera.transform);
        transform.Rotate(0, 180, 0); // Flip it so it displays correctly

        // Calculate proper scaling to match screen size
        float height = 2.0f * distanceFromCamera * Mathf.Tan(mainCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
        float width = height * mainCamera.aspect;

        transform.localScale = new Vector3(width, height, 1f); // Scale to fit camera view
    }
}

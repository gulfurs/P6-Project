using UnityEngine;

public class Billboard : MonoBehaviour
{
    void Update()
    {
        // Make the object face the camera every frame
        transform.LookAt(Camera.main.transform);
        // lock the rotation on the x and z axes if you only want it to rotate around y
        transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
    }
}

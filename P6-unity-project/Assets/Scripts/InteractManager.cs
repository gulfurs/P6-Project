using UnityEngine;

public class InteractManager : MonoBehaviour
{
    private StarterAssetsInputs _input;

    void Start()
    {
        _input = GetComponent<StarterAssetsInputs>();
    }

    void Update()
    {
        if (_input != null && _input.interact)
        {
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                InteractHandler interactObject = hit.collider.GetComponent<InteractHandler>();

                if (interactObject != null && interactObject.interactable)
                {
                    interactObject.InteractLogic();
                }
            }
        }
    }
}

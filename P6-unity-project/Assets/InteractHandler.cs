using UnityEngine;

public class InteractHandler : MonoBehaviour
{
    public bool interactable = true;

    public virtual void InteractLogic()
    {
        Debug.Log("Interacted with: " + gameObject.name);
        interactable = false;
    }
}

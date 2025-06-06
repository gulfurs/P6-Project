using UnityEngine;
using TMPro;

public class InteractManager : MonoBehaviour
{
    private StarterAssetsInputs _input;
    public TextMeshProUGUI tooltip;

    private InteractHandler currentHoverTarget;
    private bool canInteract = false;

    void Start()
    {
        _input = GetComponent<StarterAssetsInputs>();
        HideToolTip();
    }

    void Update()
    {
        if (!canInteract) {
            HideToolTip();
            return;
        }

        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            InteractHandler interactObject = hit.collider.GetComponent<InteractHandler>();

            if (interactObject != null && interactObject.interactable)
            {
                // Show tooltip if it's a new object
                if (interactObject != currentHoverTarget)
                {
                    currentHoverTarget = interactObject;
                    ShowToolTip(interactObject.tooltipText);
                }

                // Interact on key press
                if (_input != null && _input.interact)
                {
                   interactObject.InteractLogic();
                }

                return;
            }
        }

        if (currentHoverTarget != null)
        {
            currentHoverTarget = null;
            HideToolTip();
        }
    }

    public void ShowToolTip(string message)
    {
        tooltip.text = message;
        tooltip.gameObject.SetActive(true);
    }

    public void HideToolTip()
    {
        tooltip.gameObject.SetActive(false);
    }

    public void UnlockInteract(bool unlock)
    {
        canInteract = unlock;
    }
}

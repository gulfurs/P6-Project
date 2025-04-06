using UnityEngine;

public class InteractStep : TutorialStep
{
    public GameObject interactable;

    public override void UpdateStep(StarterAssetsInputs input)
    {
        InteractManager interactMan = FindObjectOfType<InteractManager>();
        interactMan.UnlockInteract(true);

        if (input.interact && IsLookingAt(interactable))
        {
            stepCompleted = true;
        }
    }

    public override void EndStep()
    {
        Debug.Log("Ended Tutorial Step: " + stepName);

    }

    private bool IsLookingAt(GameObject obj)
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        return Physics.Raycast(ray, out RaycastHit hit, 5f) && hit.collider.gameObject == obj;
    }
}

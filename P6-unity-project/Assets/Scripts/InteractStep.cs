using UnityEngine;

public class InteractStep : TutorialStep
{
    public GameObject interactable;
    public Objective PickUpOBJ;

    public override void UpdateStep(StarterAssetsInputs input)
    {
        InteractManager interactMan = FindObjectOfType<InteractManager>();
        interactMan.UnlockInteract(true);

        if (input.interact && IsLookingAt(interactable))
        {
            ObjectiveManager.Instance.UpdateObjectiveProgress(PickUpOBJ, 1);
            stepCompleted = true;
        }
    }

    /*public override void EndStep()
    {
        Debug.Log("AYOOOOOOOOOOOOOOOOOOOOOOOO");

    }*/

    private bool IsLookingAt(GameObject obj)
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        return Physics.Raycast(ray, out RaycastHit hit, 5f) && hit.collider.gameObject == obj;
    }
}

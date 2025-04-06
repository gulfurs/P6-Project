using UnityEngine;

public class InteractHandler : MonoBehaviour
{
    public bool interactable = true;
    [TextArea] public string tooltipText = "Press E to interact";
    public Objective ObjectiveProgress;
    public Objective ObjectiveStart;

    public virtual void InteractLogic()
    {
        if (ObjectiveProgress != null)
        {

            //npcObjective.currentProgress = npcObjective.goal; 
            ObjectiveManager.Instance.UpdateObjectiveProgress(ObjectiveProgress, 1);

            Debug.Log($"Objective '{ObjectiveProgress.objectiveName}' updated or completed.");
        }

        if (ObjectiveStart != null)
        {

            //npcObjective.currentProgress = npcObjective.goal; 
            ObjectiveManager.Instance.AddObjective(ObjectiveStart);

            Debug.Log($"Objective '{ObjectiveStart.objectiveName}' started");
        }

        Debug.Log("Interacted with: " + gameObject.name);
        interactable = false;
    }
}

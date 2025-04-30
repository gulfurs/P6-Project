using UnityEngine;

public class InteractHandler : MonoBehaviour
{
    [Copyable] public bool interactable = true;
    [TextArea] public string tooltipText = "Press E to interact";
    [Copyable] public Objective ObjectiveProgress;
    [Copyable] public Objective ObjectiveStart;

    public virtual void InteractLogic()
    {
        if (ObjectiveProgress != null)
        {

            //npcObjective.currentProgress = npcObjective.goal; 
            ObjectiveManager.Instance.UpdateObjectiveProgress(ObjectiveProgress, 1);

            Debug.Log($"Objective '{ObjectiveProgress.objectiveName}' updated or completed.");
            ObjectiveProgress = null;
        }

        if (ObjectiveStart != null)
        {

            //npcObjective.currentProgress = npcObjective.goal; 
            ObjectiveManager.Instance.AddObjective(ObjectiveStart, transform);

            Debug.Log($"Objective '{ObjectiveStart.objectiveName}' started");
        }

        Debug.Log("Interacted with: " + gameObject.name);
        interactable = false;
    }
}

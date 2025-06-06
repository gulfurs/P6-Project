using UnityEngine;

public class LastStep : TutorialStep
{
    public override void StartStep()
    {
        Debug.Log("Started Tutorial Step: " + stepName);

        // Enable interactions
        foreach (var handler in enableInteraction)
        {
            if (handler != null)
            {
                handler.interactable = true;
            }
        }

        // Disable interactions
        foreach (var handler in disableInteraction)
        {
            if (handler != null)
            {
                handler.interactable = false;
            }
        }

        // Enable GameObjects
        foreach (var go in enableGameObject)
        {
            if (go != null)
            {
                go.SetActive(true);
            }
        }

        // Disable GameObjects
        foreach (var go in disableGameObject)
        {
            if (go != null)
            {
                go.SetActive(false);
            }
        }


        if (initiateTimeline != null)
        {
            initiateTimeline.time = 0;
            initiateTimeline.Play();
        }

        if (initiateObjective != null)
        {
            ObjectiveManager.Instance.AddObjective(initiateObjective);
        }
    }

    public override void UpdateStep(StarterAssetsInputs input)
    {
        
    }

    public override void EndStep()
    {
        Debug.Log("Ended Tutorial Step: " + stepName);
        // HideBorders(); // If necessary
    }

    private void OnEnable()
    {
        // Subscribe to the event to handle XPReached
        ObjectiveManager.OnXPReached += HandleXPReached;
    }

    private void OnDisable()
    {
        // Unsubscribe from the event when this object is disabled
        ObjectiveManager.OnXPReached -= HandleXPReached;
    }

    // This method will be called when the event is triggered
    private void HandleXPReached()
    {
        // Set stepCompleted to true when the XP is reached
        stepCompleted = true;
        Debug.Log("XP reached, step completed!");
    }
}

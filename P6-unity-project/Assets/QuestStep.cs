using UnityEngine;

public class QuestStep : TutorialStep
{
    public Objective OBJ;

    void Awake()
    {
        ObjectiveManager.OnObjectiveCompleted += OnObjectiveCompleted;
    }

    void OnDestroy()
    {
        ObjectiveManager.OnObjectiveCompleted -= OnObjectiveCompleted;
    }

    private void OnObjectiveCompleted(Objective completedObjective)
    {
        if (completedObjective == OBJ)
        {
            stepCompleted = true;
        }
    }

    public override void EndStep()
    {
        Debug.Log("Ended Tutorial Step: " + stepName);
        //HideBorders();
    }
}

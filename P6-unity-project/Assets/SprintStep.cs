using UnityEngine;

public class SprintStep : TutorialStep
{
    public override void EndStep()
    {
        Debug.Log("Ended Tutorial Step: " + stepName);
        //HideBorders();
    }
}

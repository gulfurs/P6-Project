using UnityEngine;

public class MoveStep : TutorialStep
{
    private bool hasMoved = false;
    private bool hasJumped = false;
    public Objective MoveObj;
    public Objective JumpObj;

    public override void UpdateStep(StarterAssetsInputs input)
    {
        if (input.move.magnitude > 0.1f)
        {
            ObjectiveManager.Instance.UpdateObjectiveProgress(MoveObj, 1);
            hasMoved = true;
        }
        if (input.jump)
        {
            ObjectiveManager.Instance.UpdateObjectiveProgress(JumpObj, 1);
            hasJumped = true;
        }

        if (hasMoved && hasJumped)
            stepCompleted = true;
    }
}

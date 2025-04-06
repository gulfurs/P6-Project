using UnityEngine;

public class MoveStep : TutorialStep
{
    private bool hasMoved = false;
    private bool hasJumped = false;

    public override void UpdateStep(StarterAssetsInputs input)
    {
        if (input.move.magnitude > 0.1f)
            hasMoved = true;

        if (input.jump)
            hasJumped = true;

        if (hasMoved && hasJumped)
            stepCompleted = true;
    }
}

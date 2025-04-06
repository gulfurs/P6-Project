using UnityEngine;

public class JumpStep : TutorialStep
{
    public override void UpdateStep(StarterAssetsInputs input)
    {
        if (input.jump)
        {
            stepCompleted = true;
        }
    }
}

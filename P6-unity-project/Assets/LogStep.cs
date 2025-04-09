using UnityEngine;
using System.Collections;

public class LogStep : TutorialStep
{
    public override void StartStep()
    {
        StartCoroutine(WaitForDialogueAndContinue());
    }

    public override void UpdateStep(StarterAssetsInputs input)
    {
        base.UpdateStep(input);

        if (input.log)
        {   
            stepCompleted = true;
        }   
    }
}

using UnityEngine;
using System.Collections;

public class LogStep : TutorialStep
{
    public override void StartStep()
    {
         // This runs everything in your base StartStep (enabling/disabling interactions, etc.)
        
        // Then delay the dialogue portion:
        StartCoroutine(WaitForDialogueAndContinue());
    }

    public override void UpdateStep(StarterAssetsInputs input)
    {
        if (input.log)
        {   
            stepCompleted = true;
        }   
    }
}

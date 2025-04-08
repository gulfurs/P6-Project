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

    private IEnumerator WaitForDialogueAndContinue()
    {
        NPCInteract[] npcInteracts = FindObjectsOfType<NPCInteract>();

        // Wait until none of them are in dialogue
        while (AnyNPCInDialogue(npcInteracts))
        {
            yield return null; // wait for the next frame
        }
        base.StartStep();
        // Disable GameObjects
        // Now continue with borders and the first line
        ShowBorders();
        ShowNextLine();
    }

    private bool AnyNPCInDialogue(NPCInteract[] npcInteracts)
    {
        foreach (var npc in npcInteracts)
        {
            if (npc.inDialogue)
                return true;
        }
        return false;
    }
}

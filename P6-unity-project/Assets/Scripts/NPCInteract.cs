using System.Collections.Generic;
using UnityEngine;

public class NPCInteract : InteractHandler
{
    public Objective ObjectiveProgress;
    public Objective ObjectiveStart;
    public TypeWriter typeWriter;
    public List<string> npcDialogue;

    private int dialogueIndex = 0;
    private bool inDialogue = false;
    private GameManager gm;
    

    void Start()
    {
        dialogueIndex = 0;
        gm = FindObjectOfType<GameManager>();
        typeWriter = FindObjectOfType<TypeWriter>();
    }

    public override void InteractLogic()
    {
        if (ObjectiveProgress != null)
        {

            //npcObjective.currentProgress = npcObjective.goal; 
            ObjectiveManager.Instance.UpdateObjectiveProgress(ObjectiveProgress, 1);

            Debug.Log($"Objective '{ObjectiveProgress.objectiveName}' updated or completed.");
        }

        if (ObjectiveStart != null)
        {

            //npcObjective.currentProgress = npcObjective.goal; 
            ObjectiveManager.Instance.AddObjective(ObjectiveStart);

            Debug.Log($"Objective '{ObjectiveStart.objectiveName}' started");
        }

        if (!inDialogue)
        {
            gm.borders.SetTrigger("ToggleBorders");
            inDialogue = true;
            dialogueIndex = 0;
            ShowNextLine();
        }
        else
        {
            HandleNextDialogue();
        }

        base.InteractLogic();
    }

    void Update()
    {
        if (inDialogue && Input.GetMouseButtonDown(0))
        {
            Debug.Log("UPDATING");
            HandleNextDialogue();
        }
    }

    void HandleNextDialogue()
    {
        if (typeWriter == null || npcDialogue.Count == 0) return;

        // Remove formatting and characters like asterisks and dashes
        string rawText = RemoveFormattingAndSpecialChars(typeWriter.textMesh.text);
        string rawDialogue = RemoveFormattingAndSpecialChars(npcDialogue[dialogueIndex]);

        if (rawText != rawDialogue)
        {
            typeWriter.SkipTyping();
        }
        else
        {
            dialogueIndex++;

            if (dialogueIndex < npcDialogue.Count)
            {
                ShowNextLine();
            }
            else
            {
                EndDialogue();
            }
        }
    }

    // Function to strip rich text tags, asterisks, and dashes
    string RemoveFormattingAndSpecialChars(string text)
    {
        // Remove rich text tags and asterisks/dashes
        string noFormatting = System.Text.RegularExpressions.Regex.Replace(text, "<.*?>", "");
        string cleanText = noFormatting.Replace("*", "").Replace("-", " ");
        return cleanText;
    }



    void ShowNextLine()
    {
        typeWriter = FindObjectOfType<TypeWriter>();
        typeWriter.StartTyping(npcDialogue[dialogueIndex]);
    }

    void EndDialogue()
    {
        inDialogue = false;
        gm.borders.SetTrigger("ToggleBorders");
    }
}

using System.Collections.Generic;
using UnityEngine;

public class NPCInteract : InteractHandler
{
    public Objective npcObjective;
    public TypeWriter typeWriter;
    public List<string> npcDialogue;

    private int dialogueIndex = 0;
    private bool inDialogue = false;
    private GameManager gm;

    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        typeWriter = FindObjectOfType<TypeWriter>();
    }

    public override void InteractLogic()
    {
        if (npcObjective != null)
        {

            //npcObjective.currentProgress = npcObjective.goal; 
            ObjectiveManager.Instance.UpdateObjectiveProgress(npcObjective, 1);

            Debug.Log($"Objective '{npcObjective.objectiveName}' updated or completed.");
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
            HandleNextDialogue();
        }
    }

    void HandleNextDialogue()
    {
        if (typeWriter == null || npcDialogue.Count == 0) return;

        if (typeWriter.textMesh.text != npcDialogue[dialogueIndex])
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

    void ShowNextLine()
    {
        typeWriter.StartTyping(npcDialogue[dialogueIndex]);
    }

    void EndDialogue()
    {
        inDialogue = false;
        gm.borders.SetTrigger("ToggleBorders");
    }
}

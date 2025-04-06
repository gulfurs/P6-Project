using System.Collections.Generic;
using UnityEngine;

public class NPCInteract : InteractHandler
{
    public TypeWriter typeWriter;
    public List<string> npcDialogue;
    private int dialogueIndex = 0;
    private bool inDialogue = false;

    void Start()
    {
        dialogueIndex = 0;
        typeWriter = FindObjectOfType<TypeWriter>();
    }

    public override void InteractLogic()
    {
        if (!inDialogue)
        {
            GameManager.Instance.borders.Play("EnterBorder", 0, 0f);
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
        GameManager.Instance.borders.Play("ExitBorder", 0, 0f);
    }
}

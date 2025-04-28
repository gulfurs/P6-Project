using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using System.Linq;
using Unity.Cinemachine;

public class NPCInteract : InteractHandler
{
    public TypeWriter typeWriter;
    [Copyable] public List<string> npcDialogue;
    [Copyable] public List<NPCDialogue> npcDialogues;
    private int dialogueIndex = 0;
    public bool inDialogue = false;

    [Copyable] public PlayableDirector _timeline;
    private InteractManager interactMan;
    private FirstPersonController firstPersonController;
    public Objective mainOBJ;
    public Objective RemoveOBJ;
    public GameObject NPCInterface;

    private GameObject currentUI;

    private CinemachineCamera activeCamera;

    [System.Serializable]
    public class NPCDialogue
    {
        public string dialogue;
        public CinemachineCamera camera;
    }

    void Start()
    {
        dialogueIndex = 0;
        typeWriter = FindObjectOfType<TypeWriter>();
        interactMan = FindObjectOfType<InteractManager>();
        firstPersonController = FindObjectOfType<FirstPersonController>();
    }

    public override void InteractLogic()
    {
        if (!inDialogue)
        {
            GameManager.Instance.borders.Play("EnterBorder", 0, 0f);
            inDialogue = true;
            interactMan.UnlockInteract(false);
            firstPersonController.UnlockMove(false);
            dialogueIndex = 0;
            ShowNextLine();

            Actor actor = GetComponent<Actor>();
            if (actor != null && actor.objectiveList.Contains(RemoveOBJ) && RemoveOBJ != null)
            {
                actor.objectiveList.Remove(RemoveOBJ);
            }


            /*if (_timeline != null)
            {
                _timeline.time = 0;
                _timeline.Play();  // Start the timeline
            }*/
        }
        else
        {
            HandleNextDialogue();
        }

        base.InteractLogic();
        interactable = true;
    }

    void Update()
    {
        if (inDialogue && Input.GetMouseButtonDown(0)) 
        {
            LogManager logman = FindFirstObjectByType<LogManager>();
            logman.ToggleLogMenu(false);
            HandleNextDialogue();
        }
    }

    void HandleNextDialogue()
    {
        if (typeWriter == null || npcDialogue.Count == 0) return;

        string rawText = RemoveFormattingAndSpecialChars(typeWriter.textMesh.text);
        string rawDialogue = RemoveFormattingAndSpecialChars(npcDialogue[dialogueIndex]);

        if (rawText != rawDialogue)
        {
            typeWriter.SkipTyping(); // Skip to the next line of dialogue and the corresponding timeline signal
        }
        else
        {
            dialogueIndex++;
            if (dialogueIndex < npcDialogue.Count)
            {
                ShowNextLine();  // Show the next line and wait for the next timeline signal
            }
            else
            {
                EndDialogue();
            }
        }
    }

    // Strip rich text and formatting like asterisks or dashes
    string RemoveFormattingAndSpecialChars(string text)
    {
        string noFormatting = System.Text.RegularExpressions.Regex.Replace(text, "<.*?>", "");
        string cleanText = noFormatting.Replace("*", "").Replace("-", " ");
        return cleanText;
    }

    void ShowNextLine()
    {
        typeWriter = FindObjectOfType<TypeWriter>();
        typeWriter.StartTyping(npcDialogue[dialogueIndex]);
        /*if (_timeline != null)
        {
            _timeline.Play();
        }*/
    }

    public void OnNextSignal()
    {
        /*if (_timeline != null)
        {
            _timeline.Pause();
        }*/
    }

    public void OnEndSignal()
    {
        EndDialogue();
    }


    void EndDialogue()
    {
        inDialogue = false;
        interactMan.UnlockInteract(true);
        firstPersonController.UnlockMove(true);
        GameManager.Instance.borders.Play("ExitBorder", 0, 0f);

        if (currentUI == null && NPCInterface != null)
        {
            Debug.Log("RUN THIS");
            currentUI = Instantiate(NPCInterface);
            currentUI.GetComponent<NPCInterface>().obj = mainOBJ;
            currentUI.GetComponent<NPCInterface>().npc = this;
        }

        // Optionally stop the timeline when the dialogue ends
        /*if (_timeline != null)
        {
            _timeline.Stop();
        }*/
    }
}

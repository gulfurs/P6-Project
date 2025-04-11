using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class TutorialManager : MonoBehaviour
{
    public StarterAssetsInputs playerInput;
    public List<TutorialStep> tutorialSteps = new();
    private int currentIndex = 0;

    private TutorialStep currentStep;
    private bool isHandlingStep = false;

    void Start()
    {
        if (tutorialSteps.Count > 0)
        {
            currentStep = tutorialSteps[currentIndex];
            currentStep.StartStep();
        }
    }

    void Update()
    {
        if (currentStep == null || playerInput == null) return;

        currentStep.UpdateStep(playerInput);

        if (currentStep.stepCompleted)
        {
            StartCoroutine(HandleStepCompletion());
        }
    }

    private IEnumerator HandleStepCompletion()
    {
        if (isHandlingStep) yield break;  // Prevent double triggering
        isHandlingStep = true;

        currentStep.EndStep();

        yield return new WaitForSeconds(1.5f); // Delay before next step

        currentIndex++;

        if (currentIndex < tutorialSteps.Count)
        {
            currentStep = tutorialSteps[currentIndex];
            currentStep.StartStep();
        }
        else
        {
            Debug.Log("Tutorial finished.");
            currentStep = null;
        }

        isHandlingStep = false;
    }
}

public abstract class TutorialStep : MonoBehaviour
{
        private Animator borders;
        public string stepName;
        public string[] tutorialLines;
        private int tutorialIndex = 0;
        private TypeWriter typeWriter;
        public bool stepCompleted { get; protected set; }

        public PlayableDirector initiateTimeline;
        public Objective initiateObjective;
        public List<InteractHandler> enableInteraction;
        public List<InteractHandler> disableInteraction;

        public List<GameObject> enableGameObject;
        public List<GameObject> disableGameObject;

        private float lineTimer = 0f;
        public float timeBetweenLines = 3f;
        public bool hasDialogue = true;

        void Start()
        {
        typeWriter = FindObjectOfType<TypeWriter>();
        borders = GameManager.Instance.borders;
        }

        public virtual void StartStep()
        {
        typeWriter = FindObjectOfType<TypeWriter>();
        borders = GameManager.Instance.borders;
        EnableAndDisable();

        NPCInteract[] npcInteracts = FindObjectsOfType<NPCInteract>();
        foreach (var npcInteract in npcInteracts)
        {
            if (npcInteract.inDialogue)  // If any NPC is currently in dialogue
            {
                return;  // Exit early, don't update the tutorial step
            }
        }
            if (hasDialogue)
            {
            ShowBorders();
            ShowNextLine();
            }
        }

        public virtual void EndStep()
        {
            Debug.Log("Ended Tutorial Step: " + stepName);
            HideBorders();
        }

    public void EnableAndDisable()
    {
        Debug.Log("Started Tutorial Step: " + stepName);

        // Enable interactions
        foreach (var handler in enableInteraction)
        {
            if (handler != null)
            {
                handler.interactable = true;
            }
        }

        // Disable interactions
        foreach (var handler in disableInteraction)
        {
            if (handler != null)
            {
                handler.interactable = false;
            }
        }

        // Enable GameObjects
        foreach (var go in enableGameObject)
        {
            if (go != null)
            {
                go.SetActive(true);
            }
        }

        // Disable GameObjects
        foreach (var go in disableGameObject)
        {
            if (go != null)
            {
                go.SetActive(false);
            }
        }


        if (initiateTimeline != null)
        {
            initiateTimeline.time = 0;
            initiateTimeline.Play();
        }

        if (initiateObjective != null)
        {
            ObjectiveManager.Instance.AddObjective(initiateObjective);
        }
    }

    public virtual void UpdateStep(StarterAssetsInputs input)
    {
        // Get all instances of NPCInteract
        NPCInteract[] npcInteracts = FindObjectsOfType<NPCInteract>();

        // If any NPCInteract is in dialogue, skip the rest of the step update logic
        foreach (var npcInteract in npcInteracts)
        {
            if (npcInteract.inDialogue)  // If any NPC is currently in dialogue
            {
                return;  // Exit early, don't update the tutorial step
            }
        }

        // If typeWriter is null, return early to prevent errors
        if (typeWriter == null) return;

        // Only start the timer after typing is done
        if (!typeWriter.isTyping)
        {
            lineTimer += Time.deltaTime;

            // When it's time to show the next line and we haven't reached the end of the tutorial lines
            if (lineTimer >= timeBetweenLines && tutorialIndex < tutorialLines.Length)
            {
                ShowNextLine();
                lineTimer = 0f;  // Reset timer
            }
        }

        // End step when all lines are shown and no typing is in progress
        if (tutorialIndex >= tutorialLines.Length && !typeWriter.isTyping)
        {
            if (lineTimer >= timeBetweenLines)
            {
                //stepCompleted = true;  // Mark the step as completed
            }
        }
    }

    protected void ShowBorders()
        {
                if (borders != null)
            {
                borders.Play("EnterBorder", 0, 0f); // Play from start
            }
        }

        protected void HideBorders()
        {
            
        if (borders != null)
            {
            borders.Play("ExitBorder", 0, 0f); // Play from start
            }
        }

        public virtual void ShowNextLine()
        {
        if (tutorialIndex >= tutorialLines.Length) return;

        if (typeWriter != null)
        {
            typeWriter.StartTyping(tutorialLines[tutorialIndex]);
        }

        tutorialIndex++;
        }

    public IEnumerator WaitForDialogueAndContinue()
    {
        NPCInteract[] npcInteracts = FindObjectsOfType<NPCInteract>();

        for (int i = 0; i < 3; i++) yield return null;
        // Wait until none of them are in dialogue
        while (AnyNPCInDialogue(npcInteracts))
        {
            yield return null; // wait for the next frame
        }
        EnableAndDisable();

            if (hasDialogue)
        {
            ShowBorders();
            ShowNextLine();
        }
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

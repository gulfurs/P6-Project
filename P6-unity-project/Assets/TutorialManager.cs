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

        private float lineTimer = 0f;
        public float timeBetweenLines = 3f;

        void Start()
        {
        typeWriter = FindObjectOfType<TypeWriter>();
        borders = GameManager.Instance.borders;
        }

        public virtual void StartStep()
        {
            Debug.Log("Started Tutorial Step: " + stepName);
            ShowBorders();
            ShowNextLine();

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

        public virtual void EndStep()
        {
            Debug.Log("Ended Tutorial Step: " + stepName);
            HideBorders();
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
                stepCompleted = true;  // Mark the step as completed
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
}

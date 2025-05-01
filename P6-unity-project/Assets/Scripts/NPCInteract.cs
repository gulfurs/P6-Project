using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using System.Linq;
using Unity.Cinemachine;

public class NPCInteract : InteractHandler
{
    public TypeWriter typeWriter;
    [Copyable] public List<NPCDialogue> npcDialogues;
    private CinemachineCamera activeCamera = null;
    private int dialogueIndex = 0;
    public bool inDialogue = false;

    [Copyable] public PlayableDirector _timeline;
    private InteractManager interactMan;
    private FirstPersonController firstPersonController;
    public Objective mainOBJ;
    public Objective RemoveOBJ;
    public GameObject NPCInterface;

    private GameObject currentUI;
    private Camera weaponCamera;
    
    [Header("Audio")]
    [Tooltip("Sound that plays when clicking on this NPC")]
    public AudioClip clickSound;
    private AudioSource audioSource;

    [System.Serializable]
    public class NPCDialogue
    {
        public string dialogue;
        public CinemachineCamera camera;
        public bool instantSwitch = true;
    }

    void Start()
    {
        StartShenanigans();
    }

    void StartShenanigans()
    {
        dialogueIndex = 0;
        typeWriter = FindObjectOfType<TypeWriter>();
        interactMan = FindObjectOfType<InteractManager>();
        firstPersonController = FindObjectOfType<FirstPersonController>();
        
        // Initialize audio source if needed
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
    }

    public override void InteractLogic()
    {
        // Play the click sound if available
        if (clickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(clickSound);
        }

        if (!inDialogue)
        {
            StartShenanigans();
            GameManager.Instance.borders.Play("EnterBorder", 0, 0f);
            inDialogue = true;
            interactMan.UnlockInteract(false);
            firstPersonController.UnlockMove(false);
            dialogueIndex = 0;
            ShowNextLine();

            weaponCamera = GameObject.Find("WEAPONCAMERA").GetComponent<Camera>();
            if (weaponCamera != null) weaponCamera.enabled = false;

            Actor actor = GetComponent<Actor>();
            if (actor != null && actor.objectiveList.Contains(RemoveOBJ) && RemoveOBJ != null)
            {
                actor.objectiveList.Remove(RemoveOBJ);
            }
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
        if (typeWriter == null || npcDialogues.Count == 0) return;

        string rawText = RemoveFormattingAndSpecialChars(typeWriter.textMesh.text);
        string rawDialogue = RemoveFormattingAndSpecialChars(npcDialogues[dialogueIndex].dialogue);

        if (rawText != rawDialogue)
        {
            typeWriter.SkipTyping(); // Skip to the next line of dialogue and the corresponding timeline signal
        }
        else
        {
            dialogueIndex++;
            if (dialogueIndex < npcDialogues.Count)
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

        if (npcDialogues[dialogueIndex] != null)
        {
            typeWriter.StartTyping(npcDialogues[dialogueIndex].dialogue);

            CinemachineBrain brain = Camera.main.GetComponent<CinemachineBrain>();

            // Handle camera switching
            if (activeCamera != null)
            {
                activeCamera.Priority = 9;  // Reset previous camera
            }

            if (npcDialogues[dialogueIndex].camera != null)
            {
                activeCamera = npcDialogues[dialogueIndex].camera;

                if (npcDialogues[dialogueIndex].instantSwitch)
                {
                    brain.DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Styles.Cut, 0f);
                }
                else
                {
                    brain.DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Styles.EaseInOut, 2f);
                }

                activeCamera.Priority = 11;
            }
            else
            {
                activeCamera = null;  // No camera for this dialogue
            }
        }
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
        // End all other NPC dialogues except self
        foreach (var npc in FindObjectsOfType<NPCInteract>())
        {
            if (npc != this && npc.inDialogue)
            {
                npc.ForceEndDialogue();
            }
        }


        if (inDialogue)
        {
            inDialogue = false;
            interactMan.UnlockInteract(true);
            firstPersonController.UnlockMove(true);
            GameManager.Instance.borders.Play("ExitBorder", 0, 0f);
            StartCoroutine(EnableWeaponCameraDelayed());

            if (activeCamera != null)
            {
                activeCamera.Priority = 9;  // Reset camera when dialogue ends
                activeCamera = null;
            }

            if (currentUI == null && NPCInterface != null)
            {
                Debug.Log("RUN THIS");
                currentUI = Instantiate(NPCInterface);
                currentUI.GetComponent<NPCInterface>().obj = mainOBJ;
                currentUI.GetComponent<NPCInterface>().npc = this;
            }
        }
    }

    public void ForceEndDialogue()
    {
        inDialogue = false;
        interactMan.UnlockInteract(true);
        firstPersonController.UnlockMove(true);
        GameManager.Instance.borders.Play("ExitBorder", 0, 0f);
        StartCoroutine(EnableWeaponCameraDelayed());

        if (activeCamera != null)
        {
            activeCamera.Priority = 9;
            activeCamera = null;
        }
    }


    private System.Collections.IEnumerator EnableWeaponCameraDelayed()
    {
        yield return new WaitForSeconds(2f);
        if (weaponCamera != null) weaponCamera.enabled = true;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LogManager : MonoBehaviour
{
    [System.Serializable]
    public class LogEntry
    {
        public string wordOfInterest;
        public string userDefinition;
    }

    public List<LogEntry> logEntries = new List<LogEntry>();
    public GameObject logPrefab;
    public Transform contentPanel;
    public GameObject logMenu;
    private bool isLogOpen = false;
    private InteractManager interactMan;

    private StarterAssetsInputs input;
    public ScrollRect scrollRect;
    private CrabInterface crabInterface;
    public Animator scrollArea;
    
    [Header("Audio")]
    public AudioClip openLogSound;
    public AudioClip closeLogSound;
    public AudioClip confirmSound;
    public AudioClip clearSound;
    private AudioSource audioSource;

    [Copyable] public bool canOpenLog = false;

    void Start()
    {
        UpdateLog();
        interactMan = GetComponent<InteractManager>();
        input = GetComponent<StarterAssetsInputs>();
        logMenu.SetActive(false);
        
        // Initialize audio source
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }

    void Update()
    {
        if (!canOpenLog)
        {
            return;
        }

        if (input.log)
        {
            ToggleLogMenu(!isLogOpen);
            input.log = false;
        }
    }

    public void SetCrabInterface(CrabInterface newCrabInterface)
    {
        crabInterface = newCrabInterface;
    }

    public void ToggleLogMenu(bool open)
    {
        isLogOpen = open;
        logMenu.SetActive(isLogOpen);

        // Play appropriate sound effect
        if (audioSource != null)
        {
            if (isLogOpen && openLogSound != null)
            {
                audioSource.PlayOneShot(openLogSound);
            }
            else if (!isLogOpen && closeLogSound != null)
            {
                audioSource.PlayOneShot(closeLogSound);
            }
        }

        if (isLogOpen)
        {
            Time.timeScale = 0f; // Pause game
            input.SetCursorState(false); // Unlock cursor
            interactMan.UnlockInteract(false);
            scrollArea.Play("EnterScrollArea");
            if (scrollRect != null)
            {
                scrollRect.verticalNormalizedPosition = 1f;
            }
        }
        else
        {
            if (crabInterface != null)
            Destroy(crabInterface.gameObject);

            Time.timeScale = 1f; // Resume game
            interactMan.UnlockInteract(true);
            input.SetCursorState(true); // Lock cursor
        }
    }

    public void AddWord(string newWord)
    {
        interactMan = GetComponent<InteractManager>();
        input = GetComponent<StarterAssetsInputs>();
        // Remove punctuation marks and make the first letter uppercase
        newWord = newWord.TrimEnd('!', '.', ',', '?', ';', ':').ToLower();
        newWord = Char.ToUpper(newWord[0]) + newWord.Substring(1);

        // Avoid duplicates
        if (logEntries.Exists(entry => entry.wordOfInterest == newWord))
            return;

        logEntries.Add(new LogEntry { wordOfInterest = newWord, userDefinition = "" });
        UpdateLog();
    }


    public void UpdateLog()
    {
        // Clear all existing UI entries
        foreach (Transform child in contentPanel)
        {
            Destroy(child.gameObject);
        }

        // Rebuild UI from logEntries list, sorted alphabetically
        foreach (var entry in logEntries.OrderBy(e => e.wordOfInterest))
        {
            GameObject logInstance = Instantiate(logPrefab, contentPanel);
            Button button = logInstance.GetComponent<Button>();
            LogUI logUI = logInstance.GetComponent<LogUI>();
            logUI.Setup(entry.wordOfInterest, entry.userDefinition, this);
            button.onClick.AddListener(() => OnWordClicked(entry));
        }
    }

    private void OnWordClicked(LogEntry entry)
    {
        if (crabInterface != null)
        {
            crabInterface.AddWordToBoard(entry.wordOfInterest);
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.PlaySoundForWord(entry.wordOfInterest);
        }
    }

    public void UpdateUserDefinition(string word, string newDefinition)
    {
        LogEntry entry = logEntries.Find(e => e.wordOfInterest == word);
        if (entry != null)
        {
            entry.userDefinition = newDefinition;
            
            // Play confirmation sound
            if (audioSource != null && confirmSound != null)
            {
                audioSource.PlayOneShot(confirmSound);
            }
        }
    }

    public void StartLogManager()
    {

    }

    public void UnlockLog(bool unlock)
    {
        canOpenLog = unlock;
    }
}

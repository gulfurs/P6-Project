using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogManager : MonoBehaviour
{
    [System.Serializable]
    public class LogEntry
    {
        public string wordOfInterest;
        public string userDefinition;
        public AudioClip soundword;
    }

    public List<LogEntry> logEntries = new List<LogEntry>(); 
    public GameObject logPrefab; 
    public Transform contentPanel;
    public GameObject logMenu;
    private bool isLogOpen = false;

    private StarterAssetsInputs input;
    public ScrollRect scrollRect;
    private AudioSource audioSource;
    private CrabInterface crabInterface;
    private List<string> pendingWords = new List<string>();

    void Start()
    {
        UpdateLog();
        input = GetComponent<StarterAssetsInputs>();
        logMenu.SetActive(false);
    }

    void Update()
    {
        if (input.log)
        {
            ToggleLogMenu(!isLogOpen);
            input.log = false;
        }
    }

    public void SetCrabInterface(CrabInterface newCrabInterface)
    {
        crabInterface = newCrabInterface;

        // Send any pending words to the board
        foreach (string word in pendingWords)
        {
            crabInterface.AddWordToBoard(word);
        }
        pendingWords.Clear();
    }
        
    public void ToggleLogMenu(bool open)
    {
        isLogOpen = open;
        logMenu.SetActive(isLogOpen);

        if (isLogOpen)
        {
            Time.timeScale = 0f; // Pause game
            input.SetCursorState(false); // Unlock cursor

            if (scrollRect != null)
            {
                scrollRect.horizontalNormalizedPosition = 0f;
            }
        }
        else
        {
            Time.timeScale = 1f; // Resume game
            input.SetCursorState(true); // Lock cursor
        }
    }


    public void AddWord(string newWord)
    {
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

        // Rebuild UI from logEntries list
        foreach (var entry in logEntries)
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
        else
        {
            pendingWords.Add(entry.wordOfInterest); // Store word for later
        }

        if (entry.soundword != null && audioSource != null)
        {
            audioSource.PlayOneShot(entry.soundword);
        }
    }

    public void UpdateUserDefinition(string word, string newDefinition)
    {
        LogEntry entry = logEntries.Find(e => e.wordOfInterest == word);
        if (entry != null)
        {
            entry.userDefinition = newDefinition;
        }
    }
}

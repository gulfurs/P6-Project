using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using TMPro;

public class TestEvaluation : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject questionPanel;
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private Button[] ratingButtons; // 5 buttons for the rating scale

    [Header("TextMeshPro Trigger Settings")]
    [SerializeField] private TextMeshProUGUI textToMonitor; // The TMPro we want to monitor
    [SerializeField] private string[] triggerValues = { "50%", "100%" }; // Values that trigger evaluations
    [SerializeField] private string[] triggerEventNames = { "Reached_50", "Reached_100" }; // Event names for each trigger
    private string previousTextValue = "";

    [Header("Second Question")]
    [SerializeField] private GameObject textInputPanel;
    [SerializeField] private TextMeshProUGUI secondQuestionText;
    [SerializeField] private TMP_InputField textInputField;
    [SerializeField] private Button submitButton;

    [Header("Evaluation Settings")]
    [SerializeField] private string participantID = "default"; // For data identification

    [Header("Developer Settings")]
    [SerializeField] private bool enableDebugKeys = true;
    [SerializeField] private KeyCode debugTriggerKey = KeyCode.F5;
    [SerializeField] private string[] debugEventNames = { "Debug_Trigger_1", "Debug_Trigger_2", "Debug_Trigger_3" };
    private int debugEventCounter = 0;

    private List<EvaluationData> evaluationResults = new List<EvaluationData>();
    private bool isPaused = false;
    private int evaluationCounter = 0;
    private string currentEventName = "";
    private float gameStartTime;
    
    // Store pre-pause states
    private CursorLockMode previousLockState;
    private bool previousCursorVisibility;

    [System.Serializable]
    private class EvaluationData
    {
        public int evaluationNumber;
        public string eventName;
        public int rating;
        public string textResponse; 
        public float gameTime;
        public string timestamp;
    }

    void Start()
    {
        // Make sure the question panel is hidden at the start
        if (questionPanel != null)
            questionPanel.SetActive(false);
        if (textInputPanel != null)
            textInputPanel.SetActive(false);

        // Set up the rating buttons
        SetupRatingButtons();

        if (submitButton != null){ 
            submitButton.onClick.AddListener(OnTextSubmitted);
        }
        
        // Record when the game started
        gameStartTime = Time.time;
    }

    void Update()
    {
        // Developer debug trigger
        if (!isPaused && enableDebugKeys && Input.GetKeyDown(debugTriggerKey))
        {
            string eventName = debugEventCounter < debugEventNames.Length 
                ? debugEventNames[debugEventCounter] 
                : "Debug_Trigger_" + (debugEventCounter + 1);
                
            TriggerEvaluation(eventName);
            debugEventCounter++;
            Debug.Log($"Debug evaluation triggered: {eventName}");
        }
        CheckTextTriggers();
    }

    private void SetupRatingButtons()
    {
        if (ratingButtons == null || ratingButtons.Length == 0)
            return;

        for (int i = 0; i < ratingButtons.Length; i++)
        {
            int rating = i + 1; // Rating from 1 to 5
            ratingButtons[i].onClick.AddListener(() => OnRatingSelected(rating));
        }
    }
    private void CheckTextTriggers()
    {
        if (isPaused || textToMonitor == null) return;
        
        string currentText = textToMonitor.text;
        
        // Only process if the text has changed
        if (currentText != previousTextValue)
        {
            previousTextValue = currentText;
            
            // Check if current text matches any trigger values
            for (int i = 0; i < triggerValues.Length; i++)
            {
                if (currentText == triggerValues[i])
                {
                    string eventName = i < triggerEventNames.Length ? 
                        triggerEventNames[i] : $"TextTrigger_{triggerValues[i]}";
                    
                    TriggerEvaluation(eventName);
                    Debug.Log($"Evaluation triggered by text value: {currentText}");
                    break; // Only trigger one evaluation at a time
                }
            }
        }
    }

    /// <summary>
    /// Call this method from other scripts when you want to trigger an evaluation
    /// </summary>
    /// <param name="eventName">Name/identifier of the event triggering the evaluation</param>
    public void TriggerEvaluation(string eventName)
    {
        if (isPaused) return; // Don't trigger if we're already evaluating

        evaluationCounter++;
        currentEventName = eventName;
        ShowEvaluationQuestion();
    }

    private void ShowEvaluationQuestion()
    {
        ConfigureConflictingUI(false);

        // Store current cursor state
        previousLockState = Cursor.lockState;
        previousCursorVisibility = Cursor.visible;
        
        // Show cursor and unlock it for UI interaction
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        // Completely freeze the game
        Time.timeScale = 0f;
        AudioListener.pause = true; // Pauses most audio
        isPaused = true;

        // Show the question panel
        if (questionPanel != null)
        {
            questionPanel.SetActive(true);
        }
    }

    public void OnRatingSelected(int rating)
    {
        // Record the evaluation data
        float elapsedGameTime = Time.time - gameStartTime;
        
        EvaluationData data = new EvaluationData
        {
            evaluationNumber = evaluationCounter,
            eventName = currentEventName,
            rating = rating,
            gameTime = elapsedGameTime,
            timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            textResponse = "" 
        };
        
        // Add to results and save data
        evaluationResults.Add(data);
        SaveEvaluationData();
        
        // Hide the question panel
        if (questionPanel != null)
            questionPanel.SetActive(false);
        
        // Resume the game immediately (instead of showing text input)
        ConfigureConflictingUI(true);
        
        // Restore previous cursor state
        Cursor.lockState = previousLockState;
        Cursor.visible = previousCursorVisibility;
        
        // Unfreeze the game
        Time.timeScale = 1f;
        AudioListener.pause = false;
        isPaused = false;
    }

    public void OnTextSubmitted(){

        ConfigureConflictingUI(true);

        string textResponse = textInputField != null ? textInputField.text : "";

        if(evaluationResults.Count > 0){
            int lastIndex = evaluationResults.Count - 1;
            EvaluationData lastData = evaluationResults[lastIndex];
            lastData.textResponse = textResponse; 
            evaluationResults[lastIndex] = lastData; 
        }

        SaveEvaluationData();

        // Hide the text input panel
        if (textInputPanel != null)
            textInputPanel.SetActive(false);

        // Restore previous cursor state
        Cursor.lockState = previousLockState;
        Cursor.visible = previousCursorVisibility;
        
        // Unfreeze the game
        Time.timeScale = 1f;
        AudioListener.pause = false;
        isPaused = false;
    }

    // Add this as a new class variable at the top with other private variables
    private string currentSessionFilename = "";

    private void SaveEvaluationData()
    {
        // Get the application data path which points to the project folder
        //string basePath = Application.dataPath;
        // Go up one level from Assets folder to project root
        //string projectRoot = Directory.GetParent(basePath).FullName;
        string directory = Path.Combine(Application.persistentDataPath, "EvaluationData");
        
        if (!Directory.Exists(directory))
        {
            Debug.Log("Directory does not exist, creating: " + directory);
            Directory.CreateDirectory(directory);
        }
        
        // Only generate a filename once per play session
        if (string.IsNullOrEmpty(currentSessionFilename))
        {
            string baseFilename = participantID;
            string filename = Path.Combine(directory, baseFilename + "_evaluation_data.json");
            
            // Check if file exists - if so, append incrementing number
            int suffix = 1;
            while (File.Exists(filename))
            {
                string newParticipantID = baseFilename + "_" + suffix;
                filename = Path.Combine(directory, newParticipantID + "_evaluation_data.json");
                suffix++;
            }
            
            currentSessionFilename = filename;
        }
        
        string jsonData = JsonUtility.ToJson(new SerializableList<EvaluationData>(evaluationResults), true);
        File.WriteAllText(currentSessionFilename, jsonData);
        
        Debug.Log("Evaluation data saved to: " + currentSessionFilename);
    }

    // Helper class to serialize lists
    [Serializable]
    private class SerializableList<T>
    {
        public List<T> items;
        
        public SerializableList(List<T> items)
        {
            this.items = items;
        }
    }

    void ConfigureConflictingUI(bool unlock) {
        LogManager logman = FindObjectOfType<LogManager>();

        if (logman != null)
            logman.ToggleLogMenu(false);

        logman.UnlockLog(unlock);
    }

    // Call this on application quit to ensure data is saved
    private void OnApplicationQuit()
    {
        SaveEvaluationData();
    }
}
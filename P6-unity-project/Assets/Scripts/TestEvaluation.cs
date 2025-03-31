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
        
        // Save data after each evaluation for safety
        SaveEvaluationData();
        
        // Resume the game
        if (questionPanel != null)
            questionPanel.SetActive(false);
        
        if (textInputPanel != null){

            if(textInputField != null){
                textInputField.text = ""; 
            }
            textInputPanel.SetActive(true);
        }
        evaluationResults.Add(data);

    }

    public void OnTextSubmitted(){

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

    private void SaveEvaluationData()
    {
        // Get the application data path which points to the project folder
        string basePath = Application.dataPath;
        // Go up one level from Assets folder to project root
        string projectRoot = Directory.GetParent(basePath).FullName;
        string directory = Path.Combine(projectRoot, "EvaluationData");
        
        if (!Directory.Exists(directory))
        {
            Debug.Log("Directory does not exist, creating: " + directory);
            Directory.CreateDirectory(directory);
        }
            
        string filename = Path.Combine(directory, participantID + "_evaluation_data.json");
                        
        string jsonData = JsonUtility.ToJson(new SerializableList<EvaluationData>(evaluationResults), true);
        File.WriteAllText(filename, jsonData);
        
        Debug.Log("Evaluation data saved to: " + filename);
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
    
    // Call this on application quit to ensure data is saved
    private void OnApplicationQuit()
    {
        SaveEvaluationData();
    }
}
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Cursor = UnityEngine.Cursor;

public class WordWire : MonoBehaviour
{
    [System.Serializable]
    public class WordPair
    {
        public string leftWord;
        public string rightWord;
    }

    [Header("Word Configuration")]
    [SerializeField] private List<WordPair> wordPairs = new List<WordPair>();
    
    [Header("References")]
    [SerializeField] private Canvas puzzleCanvas;
    [SerializeField] private Transform leftWordsContainer;
    [SerializeField] private Transform rightWordsContainer;
    [SerializeField] private LineRenderer wirePrefab;
    [SerializeField] private Color correctWireColor = Color.green;
    [SerializeField] private Color activeWireColor = Color.yellow;

    [Header("Puzzle Activation")]
    [SerializeField] private float interactionDistance = 3f;
    
    private bool inPuzzleMode = false;
    private Camera mainCamera;
    
    private TextMeshProUGUI[] leftWordTexts;
    private TextMeshProUGUI[] rightWordTexts;
    private List<Connection> connections = new List<Connection>();
    
    private TextMeshProUGUI selectedWord;
    private LineRenderer currentWire;
    
    // Store cursor and player states
    private CursorLockMode previousCursorLockState;
    private bool previousCursorVisibleState;
    private bool wasPlayerMovementEnabled = true;
    
    // Reference to player movement script, adjust this to match your player controller
    [SerializeField] private MonoBehaviour playerMovementScript;
    
    [System.Serializable]
    private class Connection
    {
        public TextMeshProUGUI leftText;
        public TextMeshProUGUI rightText;
        public LineRenderer wire;
        public bool isCorrect;
    }

    void Start()
    {
        mainCamera = Camera.main;
        InitializeWordTexts();
        PopulateWords();
        
        // Hide puzzle canvas initially
        if (puzzleCanvas != null)
            puzzleCanvas.gameObject.SetActive(false);
    }

    void Update()
    {
        // Simple puzzle activation distance check
        if (!inPuzzleMode)
        {
            float distance = Vector3.Distance(mainCamera.transform.position, transform.position);
            if (distance <= interactionDistance && Input.GetKeyDown(KeyCode.E))
            {
                EnterPuzzleMode();
            }
        }
        
        // Handle exiting puzzle mode
        if (inPuzzleMode && Input.GetKeyDown(KeyCode.Escape))
        {
            ExitPuzzleMode();
        }

        // Handle wire drawing
        if (inPuzzleMode && currentWire != null)
        {
            Vector3 mousePosition = Input.mousePosition;
            Ray ray = mainCamera.ScreenPointToRay(mousePosition);
            RaycastHit hit;
            
            // Use raycast to get point on puzzle plane
            if (Physics.Raycast(ray, out hit, 10f, LayerMask.GetMask("UI")))
            {
                currentWire.SetPosition(1, hit.point);
            }
            else
            {
                // Fallback if raycast doesn't hit
                Vector3 worldPos = mainCamera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 2f));
                currentWire.SetPosition(1, worldPos);
            }
            
            if (Input.GetMouseButtonUp(0))
            {
                HandleConnectionAttempt();
            }
        }
    }
    
    private void EnterPuzzleMode()
    {
        if (inPuzzleMode) return;
        
        // Store cursor state
        previousCursorLockState = Cursor.lockState;
        previousCursorVisibleState = Cursor.visible;
        
        // Unlock cursor and make it visible
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        // Disable player movement if we have a reference
        if (playerMovementScript != null)
        {
            wasPlayerMovementEnabled = playerMovementScript.enabled;
            playerMovementScript.enabled = false;
        }
        
        // Show puzzle UI
        if (puzzleCanvas != null)
            puzzleCanvas.gameObject.SetActive(true);
        
        inPuzzleMode = true;
    }

    private void ExitPuzzleMode()
    {
        if (!inPuzzleMode) return;
        
        // Restore cursor state
        Cursor.lockState = previousCursorLockState;
        Cursor.visible = previousCursorVisibleState;
        
        // Re-enable player movement if it was enabled before
        if (playerMovementScript != null && wasPlayerMovementEnabled)
        {
            playerMovementScript.enabled = true;
        }
        
        // Hide puzzle UI
        if (puzzleCanvas != null)
            puzzleCanvas.gameObject.SetActive(false);
        
        // Clear connections
        ClearAllConnections();
        ResetCurrentWire();
        
        inPuzzleMode = false;
    }
    
    private void ClearAllConnections()
    {
        foreach (Connection connection in connections)
        {
            if (connection.wire != null)
                Destroy(connection.wire.gameObject);
        }
        connections.Clear();
    }
    
    private void InitializeWordTexts()
    {
        // Make sure we have the containers
        if (leftWordsContainer == null || rightWordsContainer == null)
        {
            Debug.LogError("Word containers not assigned to WordWire script!");
            return;
        }
        
        // Get all TextMeshProUGUI components from containers
        leftWordTexts = leftWordsContainer.GetComponentsInChildren<TextMeshProUGUI>(true);
        rightWordTexts = rightWordsContainer.GetComponentsInChildren<TextMeshProUGUI>(true);
        
        // Log warning if no texts found
        if (leftWordTexts.Length == 0)
            Debug.LogWarning("No TextMeshProUGUI elements found in leftWordsContainer!");
        if (rightWordTexts.Length == 0)
            Debug.LogWarning("No TextMeshProUGUI elements found in rightWordsContainer!");
            
        // Add click listeners
        foreach (var text in leftWordTexts)
        {
            AddClickListener(text, true);
            Debug.Log("Added left word: " + text.text);
        }
        
        foreach (var text in rightWordTexts)
        {
            AddClickListener(text, false);
            Debug.Log("Added right word: " + text.text);
        }
    }
    
    private void AddClickListener(TextMeshProUGUI text, bool isLeft)
    {
        EventTrigger trigger = text.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = text.gameObject.AddComponent<EventTrigger>();
            
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerDown;
        
        entry.callback.AddListener((data) => {
            OnWordClicked(text, isLeft);
        });
        
        trigger.triggers.Add(entry);
    }
    
    private void OnWordClicked(TextMeshProUGUI clickedText, bool isLeft)
    {
        Debug.Log("Clicked on: " + clickedText.text);
        
        if (selectedWord != null)
        {
            // Can't connect words from the same column
            if ((isLeft && selectedWord.transform.IsChildOf(leftWordsContainer)) || 
                (!isLeft && selectedWord.transform.IsChildOf(rightWordsContainer)))
            {
                ResetCurrentWire();
                return;
            }
            
            // Determine left and right sides of connection
            TextMeshProUGUI leftText = isLeft ? clickedText : selectedWord;
            TextMeshProUGUI rightText = isLeft ? selectedWord : clickedText;
            
            // Remove any existing connections for either word
            RemoveExistingConnections(leftText);
            RemoveExistingConnections(rightText);
            
            // Complete the connection
            CompleteConnection(leftText, rightText);
        }
        else
        {
            // First selection - start wire from this word
            selectedWord = clickedText;
            StartNewWire(clickedText.transform.position);
        }
    }
    
    private void StartNewWire(Vector3 startPos)
    {
        currentWire = Instantiate(wirePrefab, transform);
        currentWire.startColor = currentWire.endColor = activeWireColor;
        
        Vector3 pos = startPos;
        pos.z = startPos.z - 0.1f;  // Slightly in front of text
        currentWire.SetPosition(0, pos);
        currentWire.SetPosition(1, pos);
    }
    
    private void HandleConnectionAttempt()
    {
        Destroy(currentWire.gameObject);
        currentWire = null;
        selectedWord = null;
    }
    
    private void CompleteConnection(TextMeshProUGUI leftText, TextMeshProUGUI rightText)
    {
        // Set wire positions
        Vector3 leftPos = leftText.transform.position;
        Vector3 rightPos = rightText.transform.position;
        leftPos.z = rightPos.z = leftPos.z - 0.1f;  // Slightly in front of text
        
        currentWire.SetPosition(0, leftPos);
        currentWire.SetPosition(1, rightPos);
        
        // Check if connection is correct
        bool isCorrect = false;
        foreach (var pair in wordPairs)
        {
            if (leftText.text == pair.leftWord && rightText.text == pair.rightWord)
            {
                isCorrect = true;
                break;
            }
        }
        
        // Update wire color
        currentWire.startColor = currentWire.endColor = isCorrect ? correctWireColor : activeWireColor;
        
        // Save connection
        connections.Add(new Connection {
            leftText = leftText,
            rightText = rightText,
            wire = currentWire,
            isCorrect = isCorrect
        });
        
        // Reset selection
        selectedWord = null;
        currentWire = null;
        
        // Check for puzzle completion
        CheckPuzzleComplete();
    }
    
    private void RemoveExistingConnections(TextMeshProUGUI text)
    {
        List<Connection> connectionsToRemove = new List<Connection>();
        
        foreach (Connection connection in connections)
        {
            if (connection.leftText == text || connection.rightText == text)
            {
                Destroy(connection.wire.gameObject);
                connectionsToRemove.Add(connection);
            }
        }
        
        foreach (Connection connection in connectionsToRemove)
        {
            connections.Remove(connection);
        }
    }
    
    private void ResetCurrentWire()
    {
        if (currentWire != null)
        {
            Destroy(currentWire.gameObject);
            currentWire = null;
        }
        selectedWord = null;
    }
    
    private void PopulateWords()
    {
        // Safety check
        if (leftWordTexts == null || rightWordTexts == null || leftWordTexts.Length == 0 || rightWordTexts.Length == 0)
        {
            Debug.LogError("Word text elements not found. Check containers!");
            return;
        }
        
        int pairCount = Mathf.Min(wordPairs.Count, Mathf.Min(leftWordTexts.Length, rightWordTexts.Length));
        
        if (pairCount == 0)
        {
            Debug.LogError("No word pairs available to populate!");
            return;
        }
        
        // Create indices for shuffling
        List<int> indices = new List<int>();
        for (int i = 0; i < pairCount; i++)
            indices.Add(i);
        
        ShuffleList(indices);
        
        // Assign left words
        for (int i = 0; i < pairCount; i++)
        {
            leftWordTexts[i].text = wordPairs[indices[i]].leftWord;
            Debug.Log("Set left word " + i + " to: " + leftWordTexts[i].text);
        }
        
        // Shuffle again for right words
        ShuffleList(indices);
        
        // Assign right words
        for (int i = 0; i < pairCount; i++)
        {
            rightWordTexts[i].text = wordPairs[indices[i]].rightWord;
            Debug.Log("Set right word " + i + " to: " + rightWordTexts[i].text);
        }
    }
    
    private void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
    
    private void CheckPuzzleComplete()
    {
        // Count correct connections
        int correctConnections = 0;
        foreach (Connection connection in connections)
        {
            if (connection.isCorrect)
                correctConnections++;
        }
        
        if (correctConnections == wordPairs.Count)
        {
            Debug.Log("Puzzle Complete!");
            // Exit puzzle mode after a short delay
            Invoke("ExitPuzzleMode", 1.5f);
        }
    }
    
    // For debugging
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}
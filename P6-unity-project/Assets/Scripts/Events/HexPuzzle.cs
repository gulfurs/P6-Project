using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public class SymbolTextPair
{
    public GameObject symbolPrefab;
    public string associatedText;
}

public class HexPuzzle : MonoBehaviour
{
    [Header("Puzzle Setup")]
    public Transform symbolSpawnPoint;
    public TMP_Text[] wordTexts; // Six text fields around
    public List<SymbolTextPair> symbolTextPairs; // List of possible symbol-text pairs
    
    [Header("Interaction Settings")]
    [SerializeField] private KeyCode activateKey = KeyCode.E;
    [SerializeField] private float interactionDistance = 5f;
    [SerializeField] private float dropRadius = 2.0f; // Increased radius for easier dropping
    
    [Header("Feedback Settings")]
    [SerializeField] private Color highlightColor = new Color(0.8f, 0.8f, 0.2f);
    [SerializeField] private Color correctColor = Color.green;
    [SerializeField] private Color incorrectColor = Color.red;
    [SerializeField] private float feedbackDuration = 0.5f;
    [SerializeField] private AudioClip correctSound;
    [SerializeField] private AudioClip incorrectSound;
    
    // Private variables
    private GameObject currentSymbolObject; // Currently active symbol
    private TMP_Text correctTextField; // Current correct answer field
    private bool isDragging = false;
    private bool isActivated = false;
    private Camera mainCamera;
    private Vector3 originalPosition;
    private bool puzzleSolved = false;
    private float fixedZPosition;
    private AudioSource audioSource;
    private float originalSymbolScale;
    private TMP_Text highlightedText;
    
    private void Awake()
    {
        mainCamera = Camera.main;
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
    }
    
    private void Start()
    {
        SetupRandomPuzzle();
    }
    
    public void SetupRandomPuzzle()
    {
        // Clear any existing symbol first
        if (currentSymbolObject != null)
        {
            Destroy(currentSymbolObject);
        }
        
        // Select a random symbol-text pair
        int randomPairIndex = Random.Range(0, symbolTextPairs.Count);
        SymbolTextPair selectedPair = symbolTextPairs[randomPairIndex];
        
        // Reset text colors
        foreach (TMP_Text text in wordTexts)
        {
            text.color = Color.white;
        }
        
        // Assign random text values to the 6 text fields
        List<string> possibleTexts = new List<string>();
        foreach (var pair in symbolTextPairs)
        {
            possibleTexts.Add(pair.associatedText);
        }
        
        // Shuffle the possibleTexts for more randomness
        for (int i = 0; i < possibleTexts.Count; i++)
        {
            string temp = possibleTexts[i];
            int randomIndex = Random.Range(i, possibleTexts.Count);
            possibleTexts[i] = possibleTexts[randomIndex];
            possibleTexts[randomIndex] = temp;
        }
        
        // Ensure the correct text is among the options
        bool correctTextAssigned = false;
        int correctTextIndex = Random.Range(0, wordTexts.Length);
        
        for (int i = 0; i < wordTexts.Length; i++)
        {
            if (i == correctTextIndex)
            {
                wordTexts[i].text = selectedPair.associatedText;
                correctTextField = wordTexts[i];
                correctTextAssigned = true;
            }
            else
            {
                // Get a random text that's not the correct one
                string randomText;
                do
                {
                    int randomTextIndex = Random.Range(0, possibleTexts.Count);
                    randomText = possibleTexts[randomTextIndex];
                } while (randomText == selectedPair.associatedText);
                
                wordTexts[i].text = randomText;
            }
        }
        
        // In case we didn't assign the correct text (failsafe)
        if (!correctTextAssigned && wordTexts.Length > 0)
        {
            wordTexts[0].text = selectedPair.associatedText;
            correctTextField = wordTexts[0];
        }
        
        // Spawn the correct symbol
        currentSymbolObject = Instantiate(selectedPair.symbolPrefab, symbolSpawnPoint.position, Quaternion.identity, transform);
        originalPosition = currentSymbolObject.transform.position;
        fixedZPosition = originalPosition.z;
        
        // Store original scale for size adjustments
        originalSymbolScale = currentSymbolObject.transform.localScale.x;
        
        // Reset puzzle state
        puzzleSolved = false;
        isActivated = false;
    }
    
    private void Update()
    {
        if (puzzleSolved || currentSymbolObject == null) return;
        
        // Check for activation with E key when looking at the puzzle
        if (Input.GetKeyDown(activateKey) && !isActivated)
        {
            // Simple distance check instead of raycast
            float distanceToPuzzle = Vector3.Distance(mainCamera.transform.position, transform.position);
            
            if (distanceToPuzzle <= interactionDistance)
            {
                ActivatePuzzle();
            }
        }
        
        // If puzzle is activated, handle interaction
        if (isActivated)
        {
            // Start dragging on mouse button down
            if (Input.GetMouseButtonDown(0) && !isDragging)
            {
                StartDragging();
            }
            
            // Handle dragging - symbol follows cursor
            if (isDragging)
            {
                MoveSymbolToMousePosition();
                
                // End dragging on mouse button up
                if (Input.GetMouseButtonUp(0))
                {
                    StopDragging();
                }
            }
            
            // Allow exiting the puzzle with ESC
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                DeactivatePuzzle();
            }
        }
    }
    
    private void ActivatePuzzle()
    {
        isActivated = true;
        
        // Switch to cursor mode
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        // Optional: center camera on puzzle
        Vector3 lookPosition = transform.position;
        mainCamera.transform.LookAt(lookPosition);
    }
    
    private void DeactivatePuzzle()
    {
        isActivated = false;
        
        // Reset cursor mode for player control
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        // Return symbol to original position
        if (currentSymbolObject != null)
        {
            StartCoroutine(AnimateMove(currentSymbolObject.transform, originalPosition, 0.3f));
        }
    }
    
    private void MoveSymbolToMousePosition()
    {
        // Create a plane at the fixed Z position
        Plane dragPlane = new Plane(Vector3.forward, new Vector3(0, 0, fixedZPosition));
        
        // Cast a ray from mouse position
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        float distance;
        
        // Get the point where the ray intersects the plane
        if (dragPlane.Raycast(ray, out distance))
        {
            Vector3 newPosition = ray.GetPoint(distance);
            // Keep the z position fixed
            newPosition.z = fixedZPosition;
            currentSymbolObject.transform.position = newPosition;
            
            // Highlight text field if hovering over it
            CheckHoverFeedback();
        }
    }
    
    private void StartDragging()
    {
        isDragging = true;
        
        // Simple visual feedback - slightly larger
        currentSymbolObject.transform.localScale = new Vector3(
            originalSymbolScale * 1.2f,
            originalSymbolScale * 1.2f,
            originalSymbolScale * 1.2f
        );
    }
    
    private void StopDragging()
    {
        isDragging = false;
        
        // Reset size
        currentSymbolObject.transform.localScale = new Vector3(
            originalSymbolScale,
            originalSymbolScale,
            originalSymbolScale
        );
        
        // Reset any highlighted text
        if (highlightedText != null)
        {
            highlightedText.color = Color.white;
            highlightedText = null;
        }
        
        CheckDroppedLocation();
    }
    
    private void CheckHoverFeedback()
    {
        // Reset previous highlight if any
        if (highlightedText != null)
        {
            highlightedText.color = Color.white;
            highlightedText = null;
        }
        
        // Find the closest text field within a certain radius
        float closestDistance = dropRadius;
        TMP_Text closestText = null;
        
        foreach (TMP_Text text in wordTexts)
        {
            // Calculate distance in 2D (ignore Z)
            Vector2 symbolPos2D = new Vector2(currentSymbolObject.transform.position.x, currentSymbolObject.transform.position.y);
            Vector2 textPos2D = new Vector2(text.transform.position.x, text.transform.position.y);
            float distance = Vector2.Distance(symbolPos2D, textPos2D);
            
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestText = text;
            }
        }
        
        // Highlight the closest text
        if (closestText != null)
        {
            closestText.color = highlightColor;
            highlightedText = closestText;
        }
    }

    private void CheckDroppedLocation()
    {
        // Find the closest text field within a certain radius
        float closestDistance = dropRadius;
        TMP_Text closestText = null;
        
        foreach (TMP_Text text in wordTexts)
        {
            // Calculate distance in 2D (ignore Z)
            Vector2 symbolPos2D = new Vector2(currentSymbolObject.transform.position.x, currentSymbolObject.transform.position.y);
            Vector2 textPos2D = new Vector2(text.transform.position.x, text.transform.position.y);
            float distance = Vector2.Distance(symbolPos2D, textPos2D);
            
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestText = text;
            }
        }
        
        if (closestText != null)
        {
            // Snap to the text position
            Vector3 targetPos = closestText.transform.position;
            targetPos.z = fixedZPosition;
            StartCoroutine(AnimateMove(currentSymbolObject.transform, targetPos, 0.2f));
            
            CheckAnswer(closestText);
        }
        else
        {
            // No text field close enough, return to original position
            StartCoroutine(AnimateMove(currentSymbolObject.transform, originalPosition, 0.3f));
        }
    }
    
    private IEnumerator AnimateMove(Transform objectToMove, Vector3 targetPosition, float duration)
    {
        Vector3 startPosition = objectToMove.position;
        float elapsed = 0;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float normalizedTime = Mathf.Clamp01(elapsed / duration);
            
            // Use smoothstep for more natural movement
            float t = normalizedTime * normalizedTime * (3f - 2f * normalizedTime);
            objectToMove.position = Vector3.Lerp(startPosition, targetPosition, t);
            
            yield return null;
        }
        
        objectToMove.position = targetPosition;
    }

    public void CheckAnswer(TMP_Text droppedText)
    {
        if (droppedText == correctTextField)
        {
            // Visual feedback for correct answer
            StartCoroutine(ShowAnswerFeedback(droppedText, true));
            puzzleSolved = true;
            
            // Play correct sound
            if (correctSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(correctSound, 1.0f);
            }
            
            // Optional: Set up a new puzzle after a delay
            Invoke("SetupRandomPuzzle", 2.0f);
            
            // Deactivate puzzle after a delay
            Invoke("DeactivatePuzzle", 2.1f);
        }
        else
        {
            // Visual feedback for incorrect answer
            StartCoroutine(ShowAnswerFeedback(droppedText, false));
            
            // Play incorrect sound
            if (incorrectSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(incorrectSound, 0.7f);
            }
            
            // Return to original position with animation
            Invoke("ResetSymbolPosition", feedbackDuration);
        }
    }
    
    private IEnumerator ShowAnswerFeedback(TMP_Text textField, bool isCorrect)
    {
        Color feedbackColor = isCorrect ? correctColor : incorrectColor;
        Color originalColor = textField.color;
        
        // Simple color change
        textField.color = feedbackColor;
        
        yield return new WaitForSeconds(feedbackDuration);
        
        // Set final color
        textField.color = isCorrect ? correctColor : Color.white;
    }

    private void ResetSymbolPosition()
    {
        if (currentSymbolObject != null)
        {
            StartCoroutine(AnimateMove(currentSymbolObject.transform, originalPosition, 0.3f));
        }
    }
}
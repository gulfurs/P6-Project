using UnityEngine;
using TMPro;
using System.Collections.Generic;

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
    
    [Header("Raycast Settings")]
    public LayerMask interactableLayer;
    public float maxRaycastDistance = 20f;
    
    // Private variables
    private GameObject currentSymbolObject; // Currently active symbol
    private TMP_Text correctTextField; // Current correct answer field
    private bool isDragging = false;
    private bool isLockedOn = false;
    private Camera mainCamera;
    private Vector3 originalPosition;
    private bool puzzleSolved = false;
    private float fixedZPosition;
    
    private void Start()
    {
        mainCamera = Camera.main;
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
        
        // Reset puzzle state
        puzzleSolved = false;
    }
    
    private void Update()
    {
        if (puzzleSolved || currentSymbolObject == null) return;

        // Handle raycasting for interaction
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxRaycastDistance, interactableLayer))
            {
                // Check if we hit the hexagon puzzle itself
                if (hit.collider.gameObject == gameObject)
                {
                    LockOnToPuzzle();
                }
                
                // Check if we hit the symbol
                if (hit.collider.gameObject == currentSymbolObject)
                {
                    isDragging = true;
                }
            }
        }

        // Handle dragging
        if (isDragging)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            
            // Create a plane at the fixed Z position
            Plane dragPlane = new Plane(Vector3.forward, new Vector3(0, 0, fixedZPosition));
            float distance;
            
            // Get the point where the ray intersects the plane
            if (dragPlane.Raycast(ray, out distance))
            {
                Vector3 newPosition = ray.GetPoint(distance);
                // Keep the z position fixed
                newPosition.z = fixedZPosition;
                currentSymbolObject.transform.position = newPosition;
            }
            
            // End dragging on mouse button up
            if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
                CheckDroppedLocation();
            }
        }
    }

    private void CheckDroppedLocation()
    {
        // Cast a ray from the symbol position to detect nearby text fields
        Collider[] hitColliders = Physics.OverlapSphere(currentSymbolObject.transform.position, 0.5f);
        
        bool foundTarget = false;
        foreach (var hitCollider in hitColliders)
        {
            // Check if the collider has a TMP_Text component or is a parent of one
            TMP_Text textComponent = hitCollider.GetComponent<TMP_Text>();
            if (textComponent == null)
            {
                textComponent = hitCollider.GetComponentInChildren<TMP_Text>();
            }
            
            if (textComponent != null && System.Array.IndexOf(wordTexts, textComponent) != -1)
            {
                CheckAnswer(textComponent);
                foundTarget = true;
                break;
            }
        }
        
        if (!foundTarget)
        {
            ResetSymbolPosition();
        }
    }

    public void CheckAnswer(TMP_Text droppedText)
    {
        if (droppedText == correctTextField)
        {
            Debug.Log("Correct!");
            puzzleSolved = true;
            
            // Optional: Set up a new puzzle after a delay
            Invoke("SetupRandomPuzzle", 2.0f);
        }
        else
        {
            Debug.Log("Wrong answer! Try again.");
            ResetSymbolPosition();
        }
    }

    private void ResetSymbolPosition()
    {
        currentSymbolObject.transform.position = originalPosition;
    }

    private void LockOnToPuzzle()
    {
        if (!isLockedOn)
        {
            Vector3 targetPosition = new Vector3(transform.position.x, transform.position.y, mainCamera.transform.position.z);
            mainCamera.transform.position = targetPosition;
            isLockedOn = true;
        }
    }
}
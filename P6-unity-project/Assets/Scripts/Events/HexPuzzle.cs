using UnityEngine;
using TMPro;

public class HexPuzzle : MonoBehaviour
{
    public TMP_Text[] wordTexts; // Six text fields around
    public GameObject symbolObject; // Center symbol (draggable)
    public TMP_Text correctTextField; // Correct answer field
    private string correctWord;
    private bool isDragging = false;
    private bool isLockedOn = false;
    private Camera mainCamera;
    private Vector3 originalPosition;
    private bool puzzleSolved = false;

    private float fixedZPosition;

    [Header("Raycast Settings")]
    public LayerMask interactableLayer;
    public float maxRaycastDistance = 20f;

    private void Start()
    {
        mainCamera = Camera.main;
        AssignPuzzle();
        originalPosition = symbolObject.transform.position;
        fixedZPosition = originalPosition.z;
    }

    private void Update()
    {
        if (puzzleSolved) return;

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
                if (hit.collider.gameObject == symbolObject)
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
            
            // Create a plane that is perpendicular to the camera's forward direction
            Plane dragPlane = new Plane(-mainCamera.transform.forward, symbolObject.transform.position);
            float distance;
            
            // Get the point where the ray intersects the plane
            if (dragPlane.Raycast(ray, out distance))
            {
                Vector3 newPosition = ray.GetPoint(distance);
                newPosition.z = fixedZPosition; // Keep the symbol at the same Z position
                symbolObject.transform.position = newPosition;
            }
            
            // End dragging on mouse button up
            if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
                CheckDroppedLocation();
            }
        }
    }

    public void AssignPuzzle()
    {
        int randomIndex = Random.Range(0, wordTexts.Length);
        correctWord = wordTexts[randomIndex].text;
        correctTextField = wordTexts[randomIndex];
    }

    private void CheckDroppedLocation()
    {
        // Cast a ray from the symbol position to detect nearby text fields
        Collider[] hitColliders = Physics.OverlapSphere(symbolObject.transform.position, 0.5f);
        
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
            // You can add visual feedback or other effects here
        }
        else
        {
            Debug.Log("Wrong answer! Try again.");
            ResetSymbolPosition();
        }
    }

    private void ResetSymbolPosition()
    {
        symbolObject.transform.position = originalPosition;
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
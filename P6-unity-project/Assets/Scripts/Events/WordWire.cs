using UnityEngine;
using TMPro;

public class WordWire : MonoBehaviour
{
    // Reference to the player's movement script that should be disabled.
    public MonoBehaviour PlayerMovement;
    // The player's main camera (if you need to adjust view settings).
    public Camera mainCamera;
    // A prefab that contains a LineRenderer component used for drawing the connecting line.
    public GameObject linePrefab;

    // Internal state for word selection.
    private WireText selectedWord = null;

    // For storing previous cursor and player movement states.
    private CursorLockMode previousCursorLockState;
    private bool previousCursorVisibleState;
    private bool wasPlayerMovementEnabled;

    // This method is called when the wall is clicked to "lock on" to the puzzle.
    void OnMouseDown()
    {
        // Optionally you can add a range check here if needed.
        ShowPuzzle();
    }

    // Called to lock on: disable player movement and unlock the cursor.
    public void ShowPuzzle()
    {
        // Store previous cursor state.
        previousCursorLockState = Cursor.lockState;
        previousCursorVisibleState = Cursor.visible;

        // Unlock cursor so the player can click on words.
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Disable player movement.
        if (PlayerMovement != null)
        {
            wasPlayerMovementEnabled = PlayerMovement.enabled;
            PlayerMovement.enabled = false;
        }

        // Optionally, adjust the camera or other UI settings so the wall fills the view.
        Debug.Log("Puzzle activated – player locked on to the wall puzzle.");
    }

    // Call this method to exit the puzzle and restore player movement.
    public void ClosePuzzle()
    {
        Cursor.lockState = previousCursorLockState;
        Cursor.visible = previousCursorVisibleState;

        if (PlayerMovement != null)
        {
            PlayerMovement.enabled = wasPlayerMovementEnabled;
        }
        
        Debug.Log("Puzzle closed – player movement restored.");
    }

    // This is called from PuzzleWord when a word is clicked.
    public void OnWordClicked(WireText clickedWord)
    {
        if (selectedWord == null)
        {
            // First word is selected.
            selectedWord = clickedWord;
            Debug.Log("Selected word: " + clickedWord.GetComponent<TextMeshPro>().text);
        }
        else
        {
            // A second word has been clicked.
            // Ensure the player isn't clicking the same word twice.
            if (selectedWord != clickedWord)
            {
                // Check if the words match: same pairID and on opposite sides.
                if (selectedWord.pairID == clickedWord.pairID && selectedWord.isLeft != clickedWord.isLeft)
                {
                    Debug.Log("✅ Correct match: " + selectedWord.GetComponent<TextMeshPro>().text + " matches " + clickedWord.GetComponent<TextMeshPro>().text);
                    DrawLineBetween(selectedWord.transform.position, clickedWord.transform.position, Color.green);
                }
                else
                {
                    Debug.Log("❌ Incorrect match: " + selectedWord.GetComponent<TextMeshPro>().text + " does not match " + clickedWord.GetComponent<TextMeshPro>().text);
                    DrawLineBetween(selectedWord.transform.position, clickedWord.transform.position, Color.red);
                }
            }
            // Reset the selection after an attempt.
            selectedWord = null;
        }
    }

    // Helper method to create a line between two positions.
    void DrawLineBetween(Vector3 start, Vector3 end, Color lineColor)
    {
        if (linePrefab != null)
        {
            GameObject lineObj = Instantiate(linePrefab);
            LineRenderer lr = lineObj.GetComponent<LineRenderer>();
            if (lr != null)
            {
                lr.positionCount = 2;
                lr.SetPosition(0, start);
                lr.SetPosition(1, end);
                lr.startColor = lineColor;
                lr.endColor = lineColor;
            }
        }
    }
}

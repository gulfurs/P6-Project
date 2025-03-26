using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MemoryManager : MonoBehaviour
{

    [Header("Game Interaction")]
    public Transform cameraPosition;    // The position the camera should move to when viewing the game
    public GameObject playerMovement;   // Reference to the player movement component
    private bool isGameActive = false;  // Is the memory game currently being played?
    private Vector3 originalCameraPos; // Store original camera position
    private Quaternion originalCameraRot; 



    [Header("Grid Settings")]
    public GameObject tilePrefab;  // Assign the memoryTile prefab here
    public Transform gridParent;   // Parent transform for organizing tiles
    public int rows = 4;           // Number of rows
    public int columns = 4;        // Number of columns
    public float tileSpacing = 2f; // Distance between tiles

    [Header("Words for Matching")]
    public List<string> wordList; // List of words (must be at least (rows*columns)/2)

    private List<memoryTile> tiles = new List<memoryTile>();
    private memoryTile firstSelectedTile;
    private memoryTile secondSelectedTile;

    private int currentRow = 0;
    private int currentColumn = 0;

    void Start()
    {

        originalCameraPos = Camera.main.transform.position;
        originalCameraRot = Camera.main.transform.rotation;

        GenerateGrid();
        HighlightCurrentTile();
    }

    void Update()
    {
        if (isGameActive)
        {
            HandleInput();
        }
    }

        public void ActivateMemoryGame()
    {
        isGameActive = !isGameActive;
        
        if (isGameActive)
        {
            // Disable player movement
            if (playerMovement != null) 
                playerMovement.SetActive(false);
            
            // Move camera to game view
            StartCoroutine(MoveCameraToGameView());
            
            // Initialize game state
            currentRow = 0;
            currentColumn = 0;
            HighlightCurrentTile();
        }
        else
        {
            // Re-enable player movement
            if (playerMovement != null) 
                playerMovement.SetActive(true);
            
            // Return camera to original position
            StartCoroutine(ResetCameraPosition());
        }
    }
    
    // Smoothly move camera to game position
    IEnumerator MoveCameraToGameView()
    {
        float duration = 1.0f;
        float elapsed = 0;
        
        Transform cameraTransform = Camera.main.transform;
        Vector3 startPos = cameraTransform.position;
        Quaternion startRot = cameraTransform.rotation;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            
            cameraTransform.position = Vector3.Lerp(startPos, cameraPosition.position, t);
            cameraTransform.rotation = Quaternion.Slerp(startRot, cameraPosition.rotation, t);
            
            yield return null;
        }
    
    // Ensure final position is exact
    cameraTransform.position = cameraPosition.position;
    cameraTransform.rotation = cameraPosition.rotation;
}
    
    // Reset camera to original position
    IEnumerator ResetCameraPosition()
    {
        float duration = 1.0f;
        float elapsed = 0;
        
        Transform cameraTransform = Camera.main.transform;
        Vector3 startPos = cameraTransform.position;
        Quaternion startRot = cameraTransform.rotation;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            
            cameraTransform.position = Vector3.Lerp(startPos, originalCameraPos, t);
            cameraTransform.rotation = Quaternion.Slerp(startRot, originalCameraRot, t);
            
            yield return null;
        }
        
        // Ensure final position is exact
        cameraTransform.position = originalCameraPos;
        cameraTransform.rotation = originalCameraRot;
    }

    void GenerateGrid()
    {
        if (tilePrefab == null || wordList.Count < (rows * columns) / 2)
        {
            Debug.LogError("Tile Prefab missing OR not enough words to generate pairs.");
            return;
        }

        // Calculate grid dimensions and center position
        float gridWidth = (columns - 1) * tileSpacing;
        float gridHeight = (rows - 1) * tileSpacing;
        Vector3 startPos = gridParent.position;
        
        // Adjust startPos to center the grid on the table
        startPos.x -= gridWidth / 2;
        startPos.z -= gridHeight / 2;

        // Generate word pairs and shuffle them
        List<string> words = new List<string>();
        foreach (string word in wordList)
        {
            words.Add(word);
            words.Add(word); // Add each word twice to make pairs
        }
        ShuffleList(words);

        // Clear any existing tiles
        foreach (Transform child in gridParent)
        {
            if (child != gridParent) // Don't destroy the parent
                Destroy(child.gameObject);
        }
        tiles.Clear();

        // Generate tiles in a grid layout
        int wordIndex = 0;
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                // Calculate position relative to the grid parent's position
                Vector3 position = new Vector3(
                    startPos.x + c * tileSpacing,
                    startPos.y + 0.6f, // Slight offset to prevent z-fighting
                    startPos.z + r * tileSpacing
                );
                
                GameObject newTileObj = Instantiate(tilePrefab, position, Quaternion.Euler(90, 0, 0), gridParent);
                
                // Set a unique name for the tile
                newTileObj.name = "Tile_" + r + "_" + c;
                
                memoryTile newTile = newTileObj.GetComponent<memoryTile>();
                if (wordIndex < words.Count)
                {
                    newTile.word = words[wordIndex]; // Assign word
                    newTile.wordTMP.text = "";       // Hide the word initially
                }
                else
                {
                    Debug.LogWarning("Not enough words for all tiles!");
                }
                
                tiles.Add(newTile);
                wordIndex++;
            }
        }
    }

    void ShuffleList(List<string> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }


    void HandleInput()
    {
        bool moved = false;

        if (Input.GetKeyDown(KeyCode.UpArrow)) { currentRow = (currentRow - 1 + rows) % rows; moved = true; }
        if (Input.GetKeyDown(KeyCode.DownArrow)) { currentRow = (currentRow + 1) % rows; moved = true; }
        if (Input.GetKeyDown(KeyCode.LeftArrow)) { currentColumn = (currentColumn - 1 + columns) % columns; moved = true; }
        if (Input.GetKeyDown(KeyCode.RightArrow)) { currentColumn = (currentColumn + 1) % columns; moved = true; }

        if (moved) { HighlightCurrentTile(); }

        if (Input.GetKeyDown(KeyCode.Space)) { OnTileSelected(); }
    }

    int GetTileIndex(int row, int col) => row * columns + col;

    void HighlightCurrentTile()
    {
        foreach (var tile in tiles)
        {
            Renderer rend = tile.GetComponent<Renderer>();
            if (rend != null) { rend.material.color = Color.white; }
        }

        int index = GetTileIndex(currentRow, currentColumn);
        if (index >= 0 && index < tiles.Count)
        {
            Renderer rend = tiles[index].GetComponent<Renderer>();
            if (rend != null) { rend.material.color = Color.yellow; }
        }
    }

    void OnTileSelected()
    {
        int index = GetTileIndex(currentRow, currentColumn);
        if (index < 0 || index >= tiles.Count) return;

        memoryTile selectedTile = tiles[index];
        if (selectedTile.isFlipped || selectedTile.isMatched) return;

        selectedTile.ShowWord();

        if (firstSelectedTile == null) { firstSelectedTile = selectedTile; }
        else if (secondSelectedTile == null)
        {
            secondSelectedTile = selectedTile;
            StartCoroutine(CheckForMatch());
        }
    }

    IEnumerator CheckForMatch()
    {
        yield return new WaitForSeconds(1f);

        if (firstSelectedTile.word == secondSelectedTile.word)
        {
            Debug.Log("✅ Match Found: " + firstSelectedTile.word);
            firstSelectedTile.SetMatched();
            secondSelectedTile.SetMatched();
        }
        else
        {
            Debug.Log("❌ No Match: " + firstSelectedTile.word + " vs " + secondSelectedTile.word);
            firstSelectedTile.HideWord();
            secondSelectedTile.HideWord();
        }

        firstSelectedTile = null;
        secondSelectedTile = null;
    }
}

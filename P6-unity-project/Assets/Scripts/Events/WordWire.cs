using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WordWire : MonoBehaviour
{
    [Header("Puzzle Setup")]
    public TextMeshPro[] leftWords;  // English words (3D TextMeshPro)
    public TextMeshPro[] rightWords; // Target Language words (3D TextMeshPro)
    public List<string> englishWords = new List<string>() { "Dog", "House", "Car", "Sun" };
    public List<string> targetLanguageWords = new List<string>() { "Hund", "Haus", "Auto", "Sonne" };
    
    [Header("Visual Feedback")]
    public Color defaultColor = Color.white;
    public Color selectedColor = Color.yellow;
    public Color correctMatchColor = Color.green;
    public Color wrongMatchColor = Color.red;
    public float colorFeedbackDuration = 1.0f;
    
    [Header("Player References")]
    public MonoBehaviour playerMovementScript; // Reference to the player movement script
    public Camera playerCamera;
    public float interactionDistance = 5f;
    public LayerMask wordLayerMask;
    
    // Private variables
    private Dictionary<string, string> wordPairs = new Dictionary<string, string>();
    private TextMeshPro selectedWord = null;
    private bool isProcessingMatch = false;
    private Dictionary<TextMeshPro, Color> originalColors = new Dictionary<TextMeshPro, Color>();
    private int remainingPairs;
    private bool puzzleSolved = false;

    private void Start()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;
            
        SetupWordPairs();
        ShuffleWords();
        StoreOriginalColors();
        remainingPairs = wordPairs.Count;
    }
    
    private void StoreOriginalColors()
    {
        // Store original colors of all text objects
        foreach (var word in leftWords)
            originalColors[word] = word.color;
            
        foreach (var word in rightWords)
            originalColors[word] = word.color;
    }

    private void SetupWordPairs()
    {
        // Create word pairs from the two lists
        int pairCount = Mathf.Min(englishWords.Count, targetLanguageWords.Count);
        for (int i = 0; i < pairCount; i++)
        {
            wordPairs[englishWords[i]] = targetLanguageWords[i];
        }
    }
    
    private void ShuffleWords()
    {
        // Shuffle the words on the wall
        List<string> shuffledEnglish = new List<string>(englishWords);
        List<string> shuffledTarget = new List<string>(targetLanguageWords);
        
        // Fisher-Yates shuffle
        for (int i = 0; i < shuffledEnglish.Count; i++)
        {
            int randomIndex = Random.Range(i, shuffledEnglish.Count);
            string temp = shuffledEnglish[i];
            shuffledEnglish[i] = shuffledEnglish[randomIndex];
            shuffledEnglish[randomIndex] = temp;
        }
        
        for (int i = 0; i < shuffledTarget.Count; i++)
        {
            int randomIndex = Random.Range(i, shuffledTarget.Count);
            string temp = shuffledTarget[i];
            shuffledTarget[i] = shuffledTarget[randomIndex];
            shuffledTarget[randomIndex] = temp;
        }
        
        // Apply shuffled words to TextMeshPro objects
        for (int i = 0; i < leftWords.Length && i < shuffledEnglish.Count; i++)
            leftWords[i].text = shuffledEnglish[i];
            
        for (int i = 0; i < rightWords.Length && i < shuffledTarget.Count; i++)
            rightWords[i].text = shuffledTarget[i];
    }

    private void Update()
    {
        if (puzzleSolved || isProcessingMatch)
            return;
            
        // Detect player interaction with words
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit, interactionDistance, wordLayerMask))
            {
                TextMeshPro hitText = hit.collider.GetComponent<TextMeshPro>();
                if (hitText != null)
                {
                    // Only process active (non-matched) words
                    if (hitText.color != correctMatchColor)
                    {
                        OnWordClicked(hitText);
                    }
                }
            }
        }
    }

    private void OnWordClicked(TextMeshPro clickedText)
    {
        // First selection or different type selection
        bool isLeftSide = System.Array.IndexOf(leftWords, clickedText) != -1;
        bool isRightSide = System.Array.IndexOf(rightWords, clickedText) != -1;
        
        // If nothing is selected yet
        if (selectedWord == null)
        {
            selectedWord = clickedText;
            selectedWord.color = selectedColor;
            return;
        }
        
        // If clicked on the same word, deselect it
        if (selectedWord == clickedText)
        {
            ResetSelection();
            return;
        }
        
        bool selectedIsLeft = System.Array.IndexOf(leftWords, selectedWord) != -1;
        
        // Ensure we're selecting one from each column
        if ((selectedIsLeft && isLeftSide) || (!selectedIsLeft && isRightSide))
        {
            // Deselect current and select new if both from same column
            selectedWord.color = originalColors[selectedWord];
            selectedWord = clickedText;
            selectedWord.color = selectedColor;
            return;
        }
        
        // We have one word from each column, check if they match
        CheckWordMatch(selectedWord, clickedText);
    }

    private void CheckWordMatch(TextMeshPro leftSide, TextMeshPro rightSide)
    {
        // Ensure leftSide is actually from left column and rightSide from right
        if (System.Array.IndexOf(leftWords, leftSide) == -1)
        {
            var temp = leftSide;
            leftSide = rightSide;
            rightSide = temp;
        }
        
        isProcessingMatch = true;
        
        if (wordPairs.ContainsKey(leftSide.text) && wordPairs[leftSide.text] == rightSide.text)
        {
            // Match is correct
            Debug.Log("✅ Correct match: " + leftSide.text + " = " + rightSide.text);
            
            // Show visual feedback
            leftSide.color = correctMatchColor;
            rightSide.color = correctMatchColor;
            
            remainingPairs--;
            
            if (remainingPairs <= 0)
            {
                puzzleSolved = true;
                Debug.Log("Puzzle solved!");
                // You could trigger a completion event here
            }
            
            selectedWord = null;
            isProcessingMatch = false;
        }
        else
        {
            // Match is incorrect
            Debug.Log("❌ Incorrect match: " + leftSide.text + " ≠ " + rightSide.text);
            
            // Show visual feedback
            leftSide.color = wrongMatchColor;
            rightSide.color = wrongMatchColor;
            
            // Reset after a delay
            StartCoroutine(ResetAfterDelay(leftSide, rightSide));
        }
    }

    private IEnumerator ResetAfterDelay(TextMeshPro leftWord, TextMeshPro rightWord)
    {
        yield return new WaitForSeconds(colorFeedbackDuration);
        
        leftWord.color = originalColors[leftWord];
        rightWord.color = originalColors[rightWord];
        
        selectedWord = null;
        isProcessingMatch = false;
    }
    
    private void ResetSelection()
    {
        if (selectedWord != null)
        {
            selectedWord.color = originalColors[selectedWord];
            selectedWord = null;
        }
    }

    // Public method to restart the puzzle
    public void RestartPuzzle()
    {
        StopAllCoroutines();
        ResetSelection();
        
        // Reset all words to original colors
        foreach (var word in leftWords)
            word.color = originalColors[word];
            
        foreach (var word in rightWords)
            word.color = originalColors[word];
            
        ShuffleWords();
        remainingPairs = wordPairs.Count;
        puzzleSolved = false;
        isProcessingMatch = false;
    }
    
    // Public property to check if puzzle is solved
    public bool IsPuzzleSolved()
    {
        return puzzleSolved;
    }
}
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class DiglotWeaveJournal : MonoBehaviour
{
    [System.Serializable]
    public class SentenceKeywordPair
    {
        [TextArea(2, 5)]
        public string sentence;
        public string keyword;
    }

    [Tooltip("Define sentences and their associated keywords")]
    public List<SentenceKeywordPair> sentenceKeywordPairs = new List<SentenceKeywordPair>();

    public TextMeshPro textMeshPro;
    public Color keywordColor = new Color(0.2f, 0.6f, 1f);
    private PickupObject pickupObject; // Reference to the PickupObject script
    private bool hasBeenPickedUp = false;

    void Start()
    {
        pickupObject = GetComponent<PickupObject>();

        // Set the initial text on the paper
        UpdateJournalText(false);
    }

    void Update()
    {
        // Check for mouse click
        if (Input.GetMouseButtonDown(0)) // Left mouse button
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Perform the raycast
            if (Physics.Raycast(ray, out hit))
            {
                // Check if the raycast hit this journal
                if (hit.collider.transform == transform)
                {
                    // Toggle the hold state of the journal
                    pickupObject.ToggleHold();
                    
                    // If first time picking up, update text with bold keywords
                    if (!hasBeenPickedUp)
                    {
                        hasBeenPickedUp = true;
                        UpdateJournalText(true);
                    }
                }
            }
        }
    }

    // Update the journal text
    private void UpdateJournalText(bool highlightKeywords)
    {
        if (textMeshPro == null) return;

        string fullText = "";
        
        foreach (var pair in sentenceKeywordPairs)
        {
            string sentenceText = pair.sentence;
            
            
            if (highlightKeywords && !string.IsNullOrEmpty(pair.keyword))
            {
                string colorHex = ColorUtility.ToHtmlStringRGB(keywordColor);
                // Use regex to find the keyword while preserving case
                sentenceText = Regex.Replace(
                    sentenceText, 
                    $"({Regex.Escape(pair.keyword)})", 
                    $"<color=#{colorHex}>$1</color>",  
                    RegexOptions.IgnoreCase
                );
            }
            
            fullText += sentenceText + " ";
        }
        
        textMeshPro.text = fullText.Trim();
    }

    // Get all keywords from this journal
    public List<string> GetKeywords()
    {
        List<string> allKeywords = new List<string>();
        
        foreach (var pair in sentenceKeywordPairs)
        {
            if (!string.IsNullOrEmpty(pair.keyword))
            {
                allKeywords.Add(pair.keyword);
            }
        }
        
        return allKeywords;
    }
}
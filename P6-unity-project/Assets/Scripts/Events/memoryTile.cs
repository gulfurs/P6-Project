using UnityEngine;
using TMPro;

public class memoryTile : MonoBehaviour
{
    [Header("Tile Settings")]
    public string word;                     // The word displayed on the tile
    public TextMeshPro wordTMP;             // TextMeshPro component to show the word

    [Header("Tile Faces")]
    public GameObject tileFront;            // Front face (word is visible)
    public GameObject tileBack;             // Back face (hidden state)

    [HideInInspector]
    public bool isFlipped = false;          // Is the tile currently flipped?
    [HideInInspector]
    public bool isMatched = false;          // Has this tile been matched already?

    void Start()
    {
        HideWord();
    }

    // Reveal the word on the tile.
    public void ShowWord()
    {
        isFlipped = true;
        tileFront.SetActive(true);
        tileBack.SetActive(false);
        if (wordTMP != null)
        {
            wordTMP.text = word;
        }
    }

    // Hide the word (flip back over).
    public void HideWord()
    {
        isFlipped = false;
        tileFront.SetActive(false);
        tileBack.SetActive(true);
    }

    // Mark the tile as matched so it stays revealed.
    public void SetMatched()
    {
        isMatched = true;
    }
}

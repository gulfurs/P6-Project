using UnityEngine;
using TMPro;

public class WireText : MonoBehaviour
{
    // Unique identifier for the pair.
    public int pairID;
    // True if this word is on the left side, false if on the right.
    public bool isLeft;

    // Reference to the manager (will be auto-found in Start)
    private WordWire manager;

    void Start()
    {
        manager = FindObjectOfType<WordWire>();
    }

    // When the word is clicked, pass this to the manager.
    void OnMouseDown()
    {
        if (manager != null)
        {
            manager.OnWordClicked(this);
        }
    }
}

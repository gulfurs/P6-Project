using UnityEngine;
using TMPro;

public class DiglotWeaveJournal : MonoBehaviour
{
    [TextArea(3, 10)]
    public string journalEntry = "This is a journal entry.";

    public TextMeshPro textMeshPro;
    private PickupObject pickupObject; // Reference to the PickupObject script

    void Start()
    {
        pickupObject = GetComponent<PickupObject>();

        // Set the initial text on the paper
        if (textMeshPro != null)
        {
            textMeshPro.text = journalEntry;
        }
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
                }
            }
        }
    }
}
 
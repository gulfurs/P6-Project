using UnityEngine;

public class Container : MonoBehaviour
{
    public Objective obj; // The objective to be updated

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PickUp")) // Check if the colliding object has the "Finish" tag
        {
            Destroy(other.gameObject); // Destroy the object (e.g., an item or the player)

            // Update the objective progress by 1 (or adjust depending on how much you want to increment)
            ObjectiveManager.Instance.UpdateObjectiveProgress(obj, 1);
        }
    }
}

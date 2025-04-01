using UnityEngine;

public class Container : MonoBehaviour
{
    public Objective obj; // The objective to be updated
    public string actorType; // The type to check against Actor.HasType
    public bool destroyOnEnter = true;
    public bool exitLogic = true;


    private void OnTriggerEnter(Collider other)
    {
        Actor actor = other.GetComponent<Actor>();
        if (actor != null && actor.HasType(actorType)) // Check if the actor has the specified type
        {
            if (destroyOnEnter)
            {
                Destroy(other.gameObject);
            }

            ObjectiveManager.Instance.UpdateObjectiveProgress(obj, 1);
        }
    } 

    private void OnTriggerExit(Collider other)
    {
        if (!exitLogic) return;

        Actor actor = other.GetComponent<Actor>();
        if (actor != null && actor.HasType(actorType)) // Check if the actor has the specified type
        {
            ObjectiveManager.Instance.UpdateObjectiveProgress(obj, -1);
        }
    }
}

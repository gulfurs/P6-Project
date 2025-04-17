using UnityEngine;

public class ObjectiveResetter : MonoBehaviour
{
    [Header("Set this in Inspector")]
    public Objective testObjective;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            if (testObjective != null && ActorManager.Instance != null)
            {
                Debug.Log("Resetting actors for objective: " + testObjective.name);
                ActorManager.Instance.ResetAllForObjective(testObjective);
            }
            else
            {
                Debug.LogWarning("Objective or ActorManager.Instance is null!");
            }
        }
    }
}

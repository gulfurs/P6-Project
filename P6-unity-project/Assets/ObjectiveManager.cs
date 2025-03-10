using UnityEngine;
using System;
using System.Collections.Generic;

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager Instance; // Singleton for easy access

    private Dictionary<string, int> activeObjectives = new Dictionary<string, int>(); // Tracks progress 
    private Dictionary<string, int> objectiveGoals = new Dictionary<string, int>();  // Stores goal values 
    private List<string> completedObjectives = new List<string>(); // Stores completed objectives

    public event Action<string> OnObjectiveCompleted; // Event for when an objective is completed
    public event Action<string, int, int> OnObjectiveProgressUpdated; // Event for tracking progress

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        AddObjective("Put objects in container", 3);
    }

    public void AddObjective(string objective, int goal = 1)
    {
        if (!activeObjectives.ContainsKey(objective))
        {
            activeObjectives[objective] = 0; // Start at 0 progress
            objectiveGoals[objective] = goal; // Set the required goal
            Debug.Log($"New Objective Added: {objective} (0/{goal})");
        }
    }

    public void UpdateObjectiveProgress(string objective, int amount)
    {
        if (activeObjectives.ContainsKey(objective))
        {
            activeObjectives[objective] += amount;
            int currentProgress = activeObjectives[objective];
            int goal = objectiveGoals[objective];

            Debug.Log($"{objective}: {currentProgress}/{goal}");

            // Fire progress update event
            OnObjectiveProgressUpdated?.Invoke(objective, currentProgress, goal);

            // Check if objective is complete
            if (currentProgress >= goal)
            {
                CompleteObjective(objective);
            }
        }
    }

    private void CompleteObjective(string objective)
    {
        if (activeObjectives.ContainsKey(objective))
        {
            activeObjectives.Remove(objective);
            objectiveGoals.Remove(objective);
            completedObjectives.Add(objective);

            Debug.Log($"Objective Completed: {objective}");
            OnObjectiveCompleted?.Invoke(objective);
        }
    }

    public bool IsObjectiveComplete(string objective)
    {
        return completedObjectives.Contains(objective);
    }

    public int GetObjectiveProgress(string objective)
    {
        return activeObjectives.ContainsKey(objective) ? activeObjectives[objective] : 0;
    }

    public int GetObjectiveGoal(string objective)
    {
        return objectiveGoals.ContainsKey(objective) ? objectiveGoals[objective] : 0;
    }
}

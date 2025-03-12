using UnityEngine;
using System;
using System.Collections.Generic;
using TMPro;

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager Instance;

    private Dictionary<string, int> activeObjectives = new Dictionary<string, int>();
    private Dictionary<string, int> objectiveGoals = new Dictionary<string, int>();
    private List<string> completedObjectives = new List<string>();

    public TextMeshProUGUI objectiveText; // Assign this in the inspector

    public event Action<string> OnObjectiveCompleted;
    public event Action<string, int, int> OnObjectiveProgressUpdated;

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
        AddObjective("Kill the charade", 3);
    }

    public void AddObjective(string objective, int goal = 1)
    {
        if (!activeObjectives.ContainsKey(objective))
        {
            activeObjectives[objective] = 0;
            objectiveGoals[objective] = goal;
            UpdateObjectiveUI();
        }
    }

    public void UpdateObjectiveProgress(string objective, int amount)
    {
        if (activeObjectives.ContainsKey(objective))
        {
            activeObjectives[objective] += amount;
            int currentProgress = activeObjectives[objective];
            int goal = objectiveGoals[objective];

            OnObjectiveProgressUpdated?.Invoke(objective, currentProgress, goal);
            UpdateObjectiveUI();

            if (currentProgress >= goal)
            {
                CompleteObjective(objective);
            }
        }
    }

    private void UpdateObjectiveUI()
    {
        string uiText = "";

        foreach (var objective in activeObjectives)
        {
            string objName = objective.Key;
            int current = objective.Value;
            int goal = objectiveGoals[objName];

            uiText += $"{objName}: {current}/{goal}\n";
        }

        foreach (var completed in completedObjectives)
        {
            uiText += $"{completed}: ✅ Completed!\n";
        }

        objectiveText.text = uiText; // Update the UI with all objectives
    }

    private void CompleteObjective(string objective)
    {
        if (activeObjectives.ContainsKey(objective))
        {
            activeObjectives.Remove(objective);
            objectiveGoals.Remove(objective);
            completedObjectives.Add(objective);

            OnObjectiveCompleted?.Invoke(objective);
            UpdateObjectiveUI();
        }
    }
}

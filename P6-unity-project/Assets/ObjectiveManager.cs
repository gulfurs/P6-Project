using UnityEngine;
using System;
using System.Collections.Generic;
using TMPro;

[CreateAssetMenu(fileName = "NewObjective", menuName = "Objectives/Objective")]
public class Objective : ScriptableObject
{
    public string objectiveName;
    public string description;
    public int goal;  // The goal to achieve for the objective
    public List<Objective> nextObjectives;  // New objectives unlocked after completion
}

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager Instance;

    [System.Serializable]
    public class ActiveObjective
    {
        public Objective objective;  // Reference to the ScriptableObject
        public int currentProgress;  // Current progress towards the goal
        public bool isUnlocked;  // Whether the objective is unlocked or not
    }

    public List<ActiveObjective> activeObjectives = new List<ActiveObjective>();  // List of active objectives
    public Objective startObjective;
    public TextMeshProUGUI objectiveText;

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
        // Optionally, add all objectives on start
        AddObjective(startObjective);
    }

    public void AddObjective(Objective newObjective)
    {
        // Check if objective is already unlocked and not added yet
        if (!IsObjectiveAdded(newObjective))
        {
            // Create an ActiveObjective instance
            ActiveObjective activeObjective = new ActiveObjective
            {
                objective = newObjective,
                currentProgress = 0,  // Start with 0 progress
                isUnlocked = true  // Automatically unlocked when added
            };

            activeObjectives.Add(activeObjective);
            Debug.Log("New Objective Added: " + newObjective.objectiveName);
            UpdateObjectiveUI();
        }
    }

    public void UpdateObjectiveProgress(Objective objective, int amount)
    {
        ActiveObjective activeObj = GetActiveObjective(objective);
        if (activeObj != null)
        {
            activeObj.currentProgress += amount;
            UpdateObjectiveUI();

            if (activeObj.currentProgress >= objective.goal)
            {
                CompleteObjective(activeObj);
            }
        }
    }

    private void CompleteObjective(ActiveObjective activeObjective)
    {
        activeObjectives.Remove(activeObjective);
        Debug.Log(activeObjective.objective.objectiveName + " Completed!");

        // Unlock the next objectives
        UnlockNextObjectives(activeObjective.objective);

        UpdateObjectiveUI();
    }

    private void UnlockNextObjectives(Objective completedObjective)
    {
        if (completedObjective.nextObjectives.Count > 0)
        {
            foreach (Objective nextObjective in completedObjective.nextObjectives)
            {
                if (!IsObjectiveAdded(nextObjective))
                {
                    AddObjective(nextObjective);  // Add the next objective
                    Debug.Log("New Objective Unlocked: " + nextObjective.objectiveName);
                }
            }
        }
    }

    private void UpdateObjectiveUI()
    {
        string uiText = "";

        foreach (var activeObjective in activeObjectives)
        {
            uiText += $"{activeObjective.objective.objectiveName}: {activeObjective.currentProgress}/{activeObjective.objective.goal}\n";
        }

        objectiveText.text = uiText;
    }

    private bool IsObjectiveAdded(Objective objective)
    {
        return activeObjectives.Exists(obj => obj.objective == objective);
    }

    private ActiveObjective GetActiveObjective(Objective objective)
    {
        return activeObjectives.Find(obj => obj.objective == objective);
    }
}

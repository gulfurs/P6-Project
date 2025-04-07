using UnityEngine;
using System.Collections.Generic;
using TMPro;

[CreateAssetMenu(fileName = "NewObjective", menuName = "Objectives/Objective")]
public class Objective : ScriptableObject
{
    public string objectiveName;
    public string description;
    public int goal;
    public List<Objective> nextObjectives;
    public int xp;  // XP reward for completing this objective
}

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager Instance;

    [System.Serializable]
    public class ActiveObjective
    {
        public Objective objective;
        public int currentProgress;
        public bool isUnlocked;
    }

    public List<ActiveObjective> activeObjectives = new List<ActiveObjective>();
    public List<Objective> startObjectives;
    public TextMeshProUGUI objectiveText;
    public TextMeshProUGUI xpText;

    public delegate void ObjectiveCompletedEvent(Objective objective);
    public static event ObjectiveCompletedEvent OnObjectiveCompleted;

    public delegate void XPReachedEvent();
    public static event XPReachedEvent OnXPReached;
    private int playerXP = 0;  // Track total XP

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
        foreach (Objective startObjective in startObjectives)
        {
            AddObjective(startObjective);
        }
        UpdateXPUI();
    }

    public void AddObjective(Objective newObjective)
    {
        if (!IsObjectiveAdded(newObjective))
        {
            ActiveObjective activeObjective = new ActiveObjective
            {
                objective = newObjective,
                currentProgress = 0,
                isUnlocked = true
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

    public bool IsObjectiveCompleted(Objective objective)
    {
        return activeObjectives.Exists(o => o.objective == objective) == false &&
               IsObjectiveAdded(objective) == true;
    }

    private void CompleteObjective(ActiveObjective activeObjective)
    {
        activeObjectives.Remove(activeObjective);

        Debug.Log(activeObjective.objective.objectiveName + " Completed!");

        // Fire the event to notify listeners
        if (OnObjectiveCompleted != null)
        {
            Debug.Log($"Firing OnObjectiveCompleted for {activeObjective.objective.objectiveName}");
            OnObjectiveCompleted.Invoke(activeObjective.objective);
        }
        else
        {
            Debug.LogWarning("OnObjectiveCompleted event has no listeners!");
        }

        // Reward XP
        RewardXP(activeObjective.objective.xp);

        // Unlock the next objectives
        UnlockNextObjectives(activeObjective.objective);

        UpdateObjectiveUI();
    }


    private void RewardXP(int xpAmount)
    {
        playerXP += xpAmount;
        UpdateXPUI();

        // Check if XP is 100 or greater and trigger the event
        if (playerXP >= 100)
        {
            OnXPReached?.Invoke();  // Trigger the XP event
        }
    }


    private void UnlockNextObjectives(Objective completedObjective)
    {
        if (completedObjective.nextObjectives.Count > 0)
        {
            foreach (Objective nextObjective in completedObjective.nextObjectives)
            {
                if (!IsObjectiveAdded(nextObjective))
                {
                    AddObjective(nextObjective);
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

    private void UpdateXPUI()
    {
        xpText.text = $"{playerXP}%";
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

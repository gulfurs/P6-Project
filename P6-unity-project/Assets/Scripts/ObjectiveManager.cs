using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;

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
    public List<Objective> completedObjectives = new List<Objective>();
    public List<Objective> startObjectives;
    public TextMeshProUGUI objectiveText;
    public TextMeshProUGUI xpText;

    public delegate void ObjectiveCompletedEvent(Objective objective);
    public static event ObjectiveCompletedEvent OnObjectiveCompleted;

    public delegate void XPReachedEvent();
    public static event XPReachedEvent OnXPReached;
    private int playerXP = 0;  // Track total XP

    private Vector3 lastPlayerPosition;
    private float updateThreshold = 2f; // meters

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
            if (startObjective == null)
            {
                Debug.LogWarning("Null objective found in startObjectives list.");
                continue;
            }
            AddObjective(startObjective);
        }
        UpdateXPUI();
    }

    void Update()
    {
        if (Vector3.Distance(Player.Instance.transform.position, lastPlayerPosition) > updateThreshold)
        {
            UpdateObjectiveUI();
            lastPlayerPosition = Player.Instance.transform.position;
        }
    }

    public void AddObjective(Objective newObjective, Transform questGiver = null)
    {
        if (!IsObjectiveAdded(newObjective))
        {
            if (questGiver != null)
            {
                newObjective.objectiveTransform = questGiver;
            }

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
        completedObjectives.Add(activeObjective.objective);
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


    public void RewardXP(int xpAmount)
    {
        StartCoroutine(WaitForDialogueToEndThenReward(xpAmount));
    }

    private IEnumerator WaitForDialogueToEndThenReward(int xpAmount)
    {
        // Wait until all NPCs are no longer in dialogue
        while (AnyNPCInDialogue())
        {
            yield return null; // wait for next frame
        }

        // Dialogue is over, reward XP
        playerXP += xpAmount;
        UpdateXPUI();

        if (playerXP >= 100)
        {
            OnXPReached?.Invoke();
        }
    }

    private bool AnyNPCInDialogue()
    {
        NPCInteract[] npcInteracts = FindObjectsOfType<NPCInteract>();
        foreach (var npcInteract in npcInteracts)
        {
            if (npcInteract.inDialogue)
            {
                return true;
            }
        }
        return false;
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

    private List<ActiveObjective> GetNearestObjectives(int count = 2)
    {
        return new List<ActiveObjective>(activeObjectives)
            .OrderBy(obj =>
                obj.objective.objectiveTransform != null
                    ? Vector3.Distance(Player.Instance.transform.position, obj.objective.objectiveTransform.position)
                    : Mathf.Infinity
            )
            .Take(count)
            .ToList();
    }

    private void UpdateObjectiveUI()
    {
        string uiText = "";
        var nearestObjectives = GetNearestObjectives(2);

        foreach (var activeObjective in nearestObjectives)
        {
            uiText += $"{activeObjective.objective.objectiveName}: {activeObjective.currentProgress}/{activeObjective.objective.goal}\n";
        }   

        objectiveText.text = uiText;
    }

    public List<ActiveObjective> GetAllObjectives()
    {
        return activeObjectives;
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

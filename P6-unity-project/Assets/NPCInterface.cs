using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NPCInterface : MonoBehaviour
{
    public Objective obj;
    public NPCInteract npc;
    public TextMeshProUGUI TextForObjective;
    public TextMeshProUGUI TextForDifficulty;
    public TextMeshProUGUI TextForReward;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void OnEnable()
    {
        
    }

    void OnDisable()
    {
        //TimeScaling(1, true);
    }

    void Start()
    {
        UpdateUI();
        TimeScaling(0, false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void TimeScaling(float t, bool st) {
        Time.timeScale = t;
        StarterAssetsInputs input = FindFirstObjectByType<StarterAssetsInputs>();
        input.SetCursorState(st);
        InteractManager interactMan = FindFirstObjectByType<InteractManager>();
        interactMan.UnlockInteract(st);
        LogManager logMan = FindFirstObjectByType<LogManager>();
        logMan.UnlockLog(st);
    }

    void UpdateUI()
    {
        if (obj != null)
        {
            TextForObjective.text = "Objective: " + obj.objectiveName;
            TextForReward.text = "+" + obj.xp.ToString() + "%";
        }
    }

    public void ConfirmObjective()
    {
        ObjectiveManager.Instance.AddObjective(obj, npc.transform);
        npc.NPCInterface = null;
        if (npc._timeline != null)
        {
            npc._timeline.time = 0;
            npc._timeline.Play();
        }
        TimeScaling(1, true);
        Destroy(gameObject);
    }

    public void DenyObjective() 
    {
        TimeScaling(1, true);
        Destroy(gameObject);
    }
}

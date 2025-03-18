using UnityEngine;

public class NPCInteract : InteractHandler
{
    public Objective npcObjective; // Public field to assign the objective

    public override void InteractLogic()
    {
        GameManager gm = FindObjectOfType<GameManager>();
        gm.borders.SetTrigger("ToggleBorders");

        if (npcObjective != null)
        {
           
            //npcObjective.currentProgress = npcObjective.goal; 
            ObjectiveManager.Instance.UpdateObjectiveProgress(npcObjective, 1);

            Debug.Log($"Objective '{npcObjective.objectiveName}' updated or completed.");
        }

        base.InteractLogic();
    }
}

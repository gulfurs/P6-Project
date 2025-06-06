using UnityEngine;

public class DropStep : TutorialStep
{
    public EquipmentController dropItem;
    public Objective DropOBJ;

    public override void UpdateStep(StarterAssetsInputs input)
    {
        EquipmentManager equipMan = FindObjectOfType<EquipmentManager>();

        if (equipMan != null && dropItem != null)
        {
            // Assuming `equippedItems` is a list or array
            if (!equipMan.equippedItems.Contains(dropItem))
            {
                ObjectiveManager.Instance.UpdateObjectiveProgress(DropOBJ, 1);
                stepCompleted = true;
            }
        }
    }
}

using UnityEngine;

public class InteractStep : TutorialStep
{
    public EquipmentController pickUpItem;
    public Objective PickUpOBJ;

    public override void UpdateStep(StarterAssetsInputs input)
    {
        EquipmentManager equipMan = FindObjectOfType<EquipmentManager>();
        InteractManager interactMan = FindObjectOfType<InteractManager>();
        interactMan.UnlockInteract(true);

        if (equipMan.equippedItems.Contains(pickUpItem))
        {
            ObjectiveManager.Instance.UpdateObjectiveProgress(PickUpOBJ, 1);
            stepCompleted = true;
        }
    }
}

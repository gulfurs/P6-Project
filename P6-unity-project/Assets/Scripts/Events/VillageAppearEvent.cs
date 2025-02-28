using UnityEngine;

public class VillageAppearEvent : SpaceTimeEvent
{

    public VillageCreation villageCreation;
    // public float fadeDuration = 3f;

    protected override void ExecuteEvent()
    {
        spawnVillage();
    }
    private void spawnVillage()
    {
        villageCreation.GenerateVillage();
    }
}

using UnityEngine;

public class MeetVillagerEvent : SpaceTimeEvent
{
    void Awake()
    {
        startDelay = 30f;
        eventDuration = 15f;
    }

    protected override void ExecuteEvent()
    {
        Debug.Log($"[MeetVillagerEvent] You meet a villager speaking Danish at {eventLocation}.");
    }
}

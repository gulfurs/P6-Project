using UnityEngine;

public class WakeUpEvent : SpaceTimeEvent
{
    protected override void ExecuteEvent()
    {
        Debug.Log($"[WakeUpEvent] Player wakes up at {eventLocation}. Feels confused.");
    }
}

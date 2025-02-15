using UnityEngine;
using System.Collections;


// Class to manage the sequence of SpaceTimeEvents
// Controls Timeline order and gap between events 
public class SpaceTimeManager : MonoBehaviour
{
    public Transform player; // Assign in Unity Inspector
    public SpaceTimeEvent[] events; // Ordered events
    public float gapBetweenEvents = 5f; // Default gap between events

    private int currentEventIndex = 0;

    void Start()
    {
        if (events.Length > 0)
            StartCoroutine(TriggerNextEvent());
    }

    IEnumerator TriggerNextEvent()
    {
        while (currentEventIndex < events.Length)
        {
            SpaceTimeEvent nextEvent = events[currentEventIndex];
            
            // Wait for the event's specific start delay
            yield return new WaitForSeconds(nextEvent.startDelay);
            
            // Start the event at the player's current position
            nextEvent.StartEvent(player);

            // Wait for the event duration
            yield return new WaitForSeconds(nextEvent.eventDuration);

            // Wait for the additional gap time before the next event
            yield return new WaitForSeconds(gapBetweenEvents);

            currentEventIndex++;
        }

        Debug.Log("All events completed.");
    }
}

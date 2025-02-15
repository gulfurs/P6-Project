using UnityEngine;

// Base class: Controls timing, activation, and player position tracking
public abstract class SpaceTimeEvent : MonoBehaviour
{
    public float startDelay = 0f;   // Delay before event starts
    public float eventDuration = 10f; // How long the event lasts
    protected Vector3 eventLocation; // Captured at event trigger

    private bool isRunning = false;

    public void StartEvent(Transform player)
    {
        eventLocation = player.position;  // Capture player's location
        isRunning = true;
        Debug.Log($"[EVENT TRIGGERED] {gameObject.name} at {eventLocation}");
        Invoke(nameof(EndEvent), eventDuration); // Schedule end
        ExecuteEvent();
    }

    private void EndEvent()
    {
        isRunning = false;
        Debug.Log($"[EVENT ENDED] {gameObject.name}");
        OnEventEnd();
    }

    protected abstract void ExecuteEvent(); // Child class defines what happens
    protected virtual void OnEventEnd() {}  // Optional cleanup
}

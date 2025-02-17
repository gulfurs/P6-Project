using UnityEngine;

public class SimpleMonster : MonoBehaviour
{
    public Transform player; // Reference to the player
    public float stalkingSpeed = 2f; // Speed while stalking
    public float pounceDistance = 5f; // Distance to trigger pounce
    public float pounceSpeed = 10f; // Speed during pounce
    public float recoveryTime = 2f; // Time to recover after pounce
    public float retreatDistance = 5f; // Distance to retreat during recovery

    private enum State { Stalking, Pouncing, Recovering }
    private State currentState;

    private Vector3 pounceTarget;
    private float recoveryTimer;

    void Start()
    {
        currentState = State.Stalking;
    }

    void Update()
    {
        switch (currentState)
        {
            case State.Stalking:
                StalkingBehavior();
                break;

            case State.Pouncing:
                PouncingBehavior();
                break;

            case State.Recovering:
                RecoveringBehavior();
                break;
        }
    }

    void StalkingBehavior()
    {
        // Move toward the player
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * stalkingSpeed * Time.deltaTime;

        // Rotate to face the player
        transform.rotation = Quaternion.LookRotation(direction);

        // Check if close enough to pounce
        if (Vector3.Distance(transform.position, player.position) <= pounceDistance)
        {
            currentState = State.Pouncing;
            pounceTarget = player.position;

            // Trigger pounce animation (commented out for now)
            // animator.SetTrigger("Pounce");
        }
    }

    void PouncingBehavior()
    {
        // Move quickly toward the pounce target
        Vector3 direction = (pounceTarget - transform.position).normalized;
        transform.position += direction * pounceSpeed * Time.deltaTime;

        // Check if reached the pounce target
        if (Vector3.Distance(transform.position, pounceTarget) <= 0.1f)
        {
            currentState = State.Recovering;
            recoveryTimer = recoveryTime;

            // Trigger recovery animation (commented out for now)
            // animator.SetTrigger("Recover");
        }
    }

    void RecoveringBehavior()
    {
        // Retreat backward
        Vector3 retreatDirection = (transform.position - player.position).normalized;
        transform.position += retreatDirection * stalkingSpeed * Time.deltaTime;

        // Count down recovery timer
        recoveryTimer -= Time.deltaTime;

        // Check if recovery is complete
        if (recoveryTimer <= 0)
        {
            currentState = State.Stalking;

            // Trigger stalking animation (commented out for now)
            // animator.SetTrigger("Stalk");
        }
    }
}
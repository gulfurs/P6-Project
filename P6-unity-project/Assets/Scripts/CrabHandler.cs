using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CrabHandler : MonoBehaviour
{
    public Transform target;
    private NavMeshAgent agent;

    public float standStillRadius = 10.0f; // Beyond this, the crab doesn't move
    public float fleeRadius = 6.0f; // Inside this, the crab flees normally
    public float superSpeedRadius = 3.0f; // Inside this, the crab flees at high speed
    public float raycastLength = 2.0f; // Length of the raycast to detect the ground

    private Quaternion targetRotation;
    public bool shouldFlee = true; // Crab always tries to flee
    public float normalSpeed = 3.5f; // Default speed
    public float superSpeed = 8.0f; // Speed when escaping aggressively

    //private List<string> confirmedWords = new List<string>();

    void Start()
    {
        if (target == null)
            target = GameObject.FindGameObjectWithTag("Player").transform;

        agent = GetComponent<NavMeshAgent>();
        agent.speed = normalSpeed;
    }

    void Update()
    {
        //ApplyBoardTextEffects();

        float distanceToPlayer = Vector3.Distance(target.position, transform.position);
        Vector3 directionToPlayer = (target.position - agent.transform.position).normalized;
        /*
        // Handle movement behavior (flee or follow)
        if (distanceToPlayer > standStillRadius)
        {
            agent.ResetPath(); // Stop moving if too far from the player
            return;
        }
        else
        {
            agent.isStopped = false;
        } */

        if (shouldFlee)
        {
            HandleFleeBehavior(distanceToPlayer, directionToPlayer); // Flee behavior
        }
        else
        {
            HandleFollowBehavior(distanceToPlayer); // Follow behavior
        }

        // Handle ground alignment and rotation
        HandleGroundAlignmentAndRotation(directionToPlayer);
    }

    // Method to handle fleeing behavior
    void HandleFleeBehavior(float distanceToPlayer, Vector3 directionToPlayer)
    {
        // Adjust flee speed based on proximity
        if (distanceToPlayer < superSpeedRadius)
        {
            agent.speed = superSpeed; // Sprint away
        }
        else
        {
            agent.speed = normalSpeed; // Normal flee speed
        }

        // Flee target position (move away from the player)
        Vector3 targetPosition = agent.transform.position - directionToPlayer * fleeRadius;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(targetPosition, out hit, fleeRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    // Method to handle following behavior
    void HandleFollowBehavior(float distanceToPlayer)
    {
        // Follow target position (move towards the player)
        Vector3 followTargetPosition = target.position;

        // Ensure follow position is valid on the NavMesh
        NavMeshHit hit;
        if (NavMesh.SamplePosition(followTargetPosition, out hit, fleeRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }

        // Stop the crab if it is too close to the player
        if (distanceToPlayer <= superSpeedRadius)
        {
            agent.isStopped = true; // Stop moving
        }
        else
        {
            agent.isStopped = false; // Keep moving otherwise
        }

        agent.speed = normalSpeed; // Maintain normal speed while following
    }

    // Method to handle ground alignment and rotation
    void HandleGroundAlignmentAndRotation(Vector3 directionToPlayer)
    {
        // Raycast to align to the ground
        RaycastHit groundHit;
        if (Physics.Raycast(transform.position, Vector3.down, out groundHit, raycastLength))
        {
            Vector3 groundNormal = groundHit.normal;
            targetRotation = Quaternion.FromToRotation(transform.up, groundNormal) * transform.rotation;
            transform.rotation = targetRotation;
        }

        // Make sure the enemy faces away from the player (or faces the player when not fleeing)
        Vector3 directionToFace;
        if (shouldFlee)
        {
            directionToFace = -directionToPlayer; // Facing away when fleeing
        }
        else
        {
            directionToFace = directionToPlayer; // Facing the player when following
        }

        directionToFace.y = 0;  // Ignore y-axis to prevent tilting
        Quaternion targetPlayerRotation = Quaternion.LookRotation(directionToFace);

        // Smoothly rotate towards the target direction (whether fleeing or following)
        transform.rotation = Quaternion.Slerp(transform.rotation, targetPlayerRotation, Time.deltaTime * 5f);
    }

    public void ApplyBoardTextEffects(List<string> confirmedWords)
    {
        GameManager gameManager = FindObjectOfType<GameManager>();

        foreach (string word in confirmedWords)
        {
            GameManager.WordEffect effect = gameManager.GetEffectForWord(word);

            if (effect != null)
            {
                shouldFlee = effect.fleeBehavior;

                if (effect._target != null)
                {
                    target = effect._target.transform;
                }
            }
        }
    }
}

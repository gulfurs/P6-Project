using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
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

    void Start()
    {
        if (target == null)
            target = GameObject.FindGameObjectWithTag("Player").transform;

        agent = GetComponent<NavMeshAgent>();
        agent.speed = normalSpeed;
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(target.position, transform.position);
        Vector3 directionToPlayer = (target.position - agent.transform.position).normalized;

        if (distanceToPlayer > standStillRadius)
        {
            // Player is too far; enemy stands still
            agent.isStopped = true;
            return;
        }
        else
        {
            agent.isStopped = false;
        }

        // Adjust fleeing behavior
        if (distanceToPlayer < superSpeedRadius)
        {
            agent.speed = superSpeed; // Sprint away
        }
        else
        {
            agent.speed = normalSpeed; // Normal flee speed
        }

        Vector3 targetPosition = agent.transform.position - directionToPlayer * fleeRadius;

        // Ensure the target position is still on the NavMesh
        NavMeshHit hit;
        if (NavMesh.SamplePosition(targetPosition, out hit, fleeRadius, NavMesh.AllAreas))
        {
            agent.destination = hit.position;
        }

        // Raycast from the bottom of the agent to detect the ground
        RaycastHit groundHit;
        if (Physics.Raycast(transform.position, Vector3.down, out groundHit, raycastLength))
        {
            // Align the character to the ground
            Vector3 groundNormal = groundHit.normal;
            targetRotation = Quaternion.FromToRotation(transform.up, groundNormal) * transform.rotation;
            transform.rotation = targetRotation;
        }

        // Ensure enemy faces away from the player
        Vector3 directionToFace = -directionToPlayer;
        directionToFace.y = 0; // Ignore the y-axis to prevent tilting
        Quaternion targetPlayerRotation = Quaternion.LookRotation(directionToFace);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetPlayerRotation, Time.deltaTime * 5f);
    }
}

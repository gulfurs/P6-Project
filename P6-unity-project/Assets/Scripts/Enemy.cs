using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public Transform player;
    private NavMeshAgent agent;
    public float closeEnoughRadius = 2.0f;
    public float raycastLength = 2.0f;  // Length of the raycast to detect the ground
    private Quaternion targetRotation;

    void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        Vector3 directionToPlayer = (player.position - agent.transform.position).normalized;
        Vector3 targetPosition = player.position - directionToPlayer * closeEnoughRadius;

        // Ensure the target position is still on the NavMesh
        NavMeshHit hit;
        if (NavMesh.SamplePosition(targetPosition, out hit, closeEnoughRadius, NavMesh.AllAreas))
        {
            agent.destination = hit.position;
        }

        // Raycast from the bottom of the agent to detect the ground
        RaycastHit groundHit;
        if (Physics.Raycast(transform.position, Vector3.down, out groundHit, raycastLength))
        {
            // Align the character to the ground by setting the rotation
            Vector3 groundNormal = groundHit.normal;
            targetRotation = Quaternion.FromToRotation(transform.up, groundNormal) * transform.rotation;

            // Apply the rotation to the agent's transform
            transform.rotation = targetRotation;
        }
    }
}

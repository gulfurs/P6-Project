using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public Transform player;
    private NavMeshAgent agent;
    public float closeEnoughRadius = 2.0f;
    // Start is called before the first frame update
    void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
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
    }
}
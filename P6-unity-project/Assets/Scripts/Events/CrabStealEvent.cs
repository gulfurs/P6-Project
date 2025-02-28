using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class CrabStealEvent : SpaceTimeEvent
{

    public GameObject crabPrefab; 
    public Transform lostItem;
    
    private GameObject spawnedCrab;
    private NavMeshAgent crabAgent;
    //private Animator crabAnimator;
    private bool isStealing = false;

    protected override void ExecuteEvent()
    {
        spawnCrab();
    }

    private void spawnCrab()
    {

        Vector3 offset = new Vector3(Random.Range(-20, 20), 0, Random.Range(-20, 20));
        Vector3 spawnPosition = eventLocation + offset;


        if (NavMesh.SamplePosition(spawnPosition, out NavMeshHit hit, 10f, NavMesh.AllAreas))
        {
            spawnedCrab = Instantiate(crabPrefab, hit.position, Quaternion.identity);
            crabAgent = spawnedCrab.GetComponent<NavMeshAgent>();
            crabAgent.Warp(hit.position);
            Debug.Log("C");
        } else {
            Debug.Log("D");
        }


        //spawnedCrab = Instantiate(crabPrefab, eventLocation, Quaternion.identity);
        //crabAgent = spawnedCrab.GetComponent<NavMeshAgent>();
        //crabAnimator = spawnedCrab.GetComponent<Animator>();
        
        MoveToLostItem();
    }

    private void MoveToLostItem()
    {
        if (crabAgent != null && lostItem != null) {
            crabAgent.SetDestination(lostItem.position);
            //crabAnimator.SetBool("isMoving", true);
        }

    }

    private void Update(){
        if (spawnedCrab != null && !isStealing && crabAgent.remainingDistance < 0.5f){
              isStealing = true;  
              crabAgent.isStopped = true;
              //animator.SetTrigger("PickUp");
              Invoke(nameof(FleeWithItem), 1.5f);
        }
    }

    private void FleeWithItem()
    {
        if (lostItem != null)
        {
            lostItem.SetParent(spawnedCrab.transform);
            lostItem.localPosition = Vector3.up * 0.5f;
        }
        Vector3 fleeDirection = (spawnedCrab.transform.position - eventLocation).normalized;
        fleeDirection = Quaternion.Euler(0, Random.Range(-45, 45), 0) * fleeDirection;
        Vector3 fleeTarget = spawnedCrab.transform.position + fleeDirection * 10f;

        crabAgent.speed *= 5;
        crabAgent.SetDestination(fleeTarget);
        crabAgent.isStopped = false;
        //crabAnimator.SetBool("isMoving", true);

        Invoke(nameof(OnEventEnd), 10f);
    }

    protected override void OnEventEnd()
    {
        Destroy(spawnedCrab, 6f);
    }
}

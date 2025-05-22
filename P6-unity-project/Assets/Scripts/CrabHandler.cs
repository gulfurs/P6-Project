using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;

public class CrabHandler : MonoBehaviour
{
    [Copyable] public Transform target;
    private NavMeshAgent agent;

    [Copyable] public float standStillRadius = 10.0f;
    [Copyable] public float fleeRadius = 6.0f;
    [Copyable] public float superSpeedRadius = 3.0f;
    [Copyable] public float raycastLength = 2.0f;

    private Quaternion targetRotation;
    [Copyable] public bool shouldFlee = true;
    [Copyable] public float normalSpeed = 3.5f;
    [Copyable] public float superSpeed = 8.0f;

    [Copyable] public bool isCarryingObject = false;
    private GameObject carriedObject;
    private GameManager gm;
    [Copyable] public CrabBehavior crabBehavior;
    [Copyable] public string targetingType;

    [SerializeField] private bool startWithAgentDisabled = false;
    private Animator animator;
    private Rig carryRig;

    void Start()
    {
        if (target == null)
            target = GameObject.FindGameObjectWithTag("Player").transform;

        agent = GetComponent<NavMeshAgent>();
        agent.speed = normalSpeed;
        gm = FindObjectOfType<GameManager>();

        if (startWithAgentDisabled)
        {
            DisableAgentStuff();
        }

        if (carriedObject == null)
        {
            foreach (Transform child in transform)
            {
                if (child.CompareTag("PickUp"))
                {
                    carriedObject = child.gameObject;
                    break; 
                }
            }
        }

        animator = GetComponentInChildren<Animator>();
        carryRig = GetComponentInChildren<Rig>();
    }


    void Update()
    {
        isCarryingObject = HasChildWithTag("PickUp");

        if (!string.IsNullOrEmpty(targetingType))
        {
            target = FindNearestTarget(targetingType);
        }

        CarryObject();

        if (target != null)
        {
            float distanceToPlayer = Vector3.Distance(target.position, transform.position);
            Vector3 directionToPlayer = (target.position - agent.transform.position).normalized;

            switch (crabBehavior)
            {
                case CrabBehavior.Flee:
                    HandleFleeBehavior(distanceToPlayer, directionToPlayer);
                    break;

                case CrabBehavior.Follow:
                    HandleFollowBehavior(distanceToPlayer);
                    break;

                case CrabBehavior.PickingUp:
                    HandlePickingUpBehavior(distanceToPlayer, directionToPlayer);
                    break;

                case CrabBehavior.GoTo:
                    HandleGotoBehavior(distanceToPlayer, directionToPlayer);
                    break;

                case CrabBehavior.DropItem:
                    DropObject();
                    break;

                case CrabBehavior.StandStill:
                    HandleStandStillBehavior();
                    break;

            }


        }
        else {
            target = transform;
        }

        if (animator != null && agent != null)
        {
            float currentSpeed = agent.velocity.magnitude;
            animator.SetFloat("CrabSpeed", currentSpeed);
        }

    }

    void HandleFleeBehavior(float distanceToPlayer, Vector3 directionToPlayer)
    {

        if (distanceToPlayer > standStillRadius)
        {
            agent.ResetPath();
            return;
        }
        else
        {
            agent.isStopped = false;
        }
        
        if (distanceToPlayer < superSpeedRadius)
        {
            agent.speed = superSpeed;
        }
        else
        {
            agent.speed = normalSpeed;
        }

        Vector3 targetPosition = agent.transform.position - directionToPlayer * fleeRadius;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(targetPosition, out hit, fleeRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    void HandleFollowBehavior(float distanceToPlayer)
    {
        Vector3 followTargetPosition = target.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(followTargetPosition, out hit, fleeRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }

        if (distanceToPlayer <= superSpeedRadius)
        {
            agent.isStopped = true;
        }
        else
        {
            agent.isStopped = false;
        }

        agent.speed = normalSpeed;
    }

    private float pickupRange = 2f;

    void HandlePickingUpBehavior(float distanceToObject, Vector3 directionToObject)
    {
        if (target == null) return;

        agent.isStopped = false;

        if (distanceToObject > pickupRange)
        {
            Debug.Log("HELP ME IM IN GREAT PAIN");
            agent.SetDestination(target.position);
            agent.speed = normalSpeed;
            return;
        }

        if (!isCarryingObject && target.CompareTag("PickUp"))
        {
            Debug.Log("Attempting pickup");
            PickUpObject(target.gameObject);
        }
    }

    void HandleGotoBehavior(float distanceToObject, Vector3 directionToObject)
    {
        agent.isStopped = false;

        if (distanceToObject > 0.5f)
        {
            Debug.Log("HELP ME IM IN GREAT PAIN");
            agent.SetDestination(target.position);
            agent.speed = normalSpeed;
        }
    }

    void HandleStandStillBehavior()
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
    }

    void HandleGroundAlignmentAndRotation(Vector3 directionToPlayer)
    {
        RaycastHit groundHit;
        if (Physics.Raycast(transform.position, Vector3.down, out groundHit, raycastLength))
        {
            Vector3 groundNormal = groundHit.normal;
            targetRotation = Quaternion.FromToRotation(transform.up, groundNormal) * transform.rotation;
            transform.rotation = targetRotation;
        }

        Vector3 directionToFace;
        if (shouldFlee)
        {
            directionToFace = -directionToPlayer;
        }
        else
        {
            directionToFace = directionToPlayer;
        }

        directionToFace.y = 0;
        Quaternion targetPlayerRotation = Quaternion.LookRotation(directionToFace);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetPlayerRotation, Time.deltaTime * 5f);
    }

    public void ApplyBoardTextEffects(List<string> confirmedWords)
    {
        targetingType = "";

        foreach (string word in confirmedWords)
        {
            GameManager.WordEffect effect = gm.GetEffectForWord(word);

            if (effect != null)
            {
                if (effect.affectFlee)
                {
                    if (effect.crabBehavior == CrabBehavior.Flee)
                    {
                        crabBehavior = CrabBehavior.Flee;
                    }
                    else if (effect.crabBehavior == CrabBehavior.Follow)
                    {
                        crabBehavior = CrabBehavior.Follow;
                    }
                    else if (effect.crabBehavior == CrabBehavior.PickingUp)
                    {
                        crabBehavior = CrabBehavior.PickingUp;
                    }
                    else if (effect.crabBehavior == CrabBehavior.DropItem)
                    {
                        crabBehavior = CrabBehavior.DropItem;
                    }
                    else if (effect.crabBehavior == CrabBehavior.StandStill)
                    {
                        crabBehavior = CrabBehavior.StandStill;
                    }
                    else if (effect.crabBehavior == CrabBehavior.GoTo)
                    {
                        crabBehavior = CrabBehavior.GoTo;
                    }
                }

                if (effect.affectTarget)
                {
                    if (effect._target != null)
                    {
                        target = effect._target.transform;
                    }
                    else if (!string.IsNullOrEmpty(effect.targetType))
                    {
                        targetingType = effect.targetType;
                        target = FindNearestTarget(effect.targetType);
                    }
                }
            }
        }
    }



    private Transform FindNearestTarget(string targetType)
    {
        Actor[] actors = FindObjectsOfType<Actor>();
        Transform nearestTarget = null;
        float shortestDistance = Mathf.Infinity;

        foreach (Actor actor in actors)
        {
            Transform actorTransform = actor.transform;

            if (actor.HasType(targetType) && !IsSelfOrAncestor(actorTransform))
            {
                float distance = Vector3.Distance(transform.position, actorTransform.position);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    nearestTarget = actorTransform;
                }
            }
        }

        return nearestTarget;
    }

    private bool IsSelfOrAncestor(Transform other)
    {
        Transform current = transform;
        while (current != null)
        {
            if (other == current)
                return true;
            current = current.parent;
        }
        return false;
    }

    void PickUpObject(GameObject obj)
    {
        carriedObject = obj;
        obj.transform.SetParent(transform);
        carriedObject.transform.position = transform.position + new Vector3(0, 0.8f, 0);
        carriedObject.transform.localRotation = Quaternion.identity;

        Collider objCollider = obj.GetComponent<Collider>();
        if (objCollider != null) objCollider.enabled = false;

        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

            NavMeshAgent agent = obj.GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.isStopped = true;
                agent.updatePosition = false;
                agent.updateRotation = false;
            }
    }


    void DropObject()
    {
        if (carriedObject != null)
        {
            carriedObject.transform.SetParent(null);
            Vector3 dropOffset = transform.forward * 1.5f + Vector3.down * 0.2f;
            carriedObject.transform.position = transform.position + dropOffset;

            Collider col = carriedObject.GetComponent<Collider>();
            if (col != null)
            {
                col.enabled = true;
            }

            Rigidbody rb = carriedObject.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = carriedObject.AddComponent<Rigidbody>();
            }

            EquipInteract equip = carriedObject.GetComponent<EquipInteract>();
            if (equip != null) {
                equip.interactable = true;
            }

            rb.isKinematic = false;
            rb.linearVelocity = Vector3.down * 2f;

            NavMeshAgent agent = carriedObject.GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.isStopped = false;
                agent.updatePosition = true;
                agent.updateRotation = true;
            }

            carriedObject = null;

            crabBehavior = CrabBehavior.Follow;
        }
    }

    void DisableAgentStuff()
    {
        if (agent != null)
        {
            agent.isStopped = true;
            agent.updatePosition = false;
            agent.updateRotation = false;
        }
    }

    void CarryObject()
    {
        if (carryRig != null)
        {
            carryRig.weight = isCarryingObject ? 1f : 0f;
        }
    }

    bool HasChildWithTag(string tag)
    {
        foreach (Transform child in transform)
        {
            if (child.CompareTag(tag))
            {
                return true;
            }
        }
        return false;
    }

}

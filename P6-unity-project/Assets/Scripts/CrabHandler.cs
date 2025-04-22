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

    [Copyable] public float standStillRadius = 10.0f; // Beyond this, the crab doesn't move
    [Copyable] public float fleeRadius = 6.0f; // Inside this, the crab flees normally
    [Copyable] public float superSpeedRadius = 3.0f; // Inside this, the crab flees at high speed
    [Copyable] public float raycastLength = 2.0f; // Length of the raycast to detect the ground

    private Quaternion targetRotation;
    [Copyable] public bool shouldFlee = true; // Crab always tries to flee
    [Copyable] public float normalSpeed = 3.5f; // Default speed
    [Copyable] public float superSpeed = 8.0f; // Speed when escaping aggressively

    [Copyable] public bool isCarryingObject = false; // Flag to check if crab is carrying an object
    private GameObject carriedObject; // Object that the crab is carrying
    private GameManager gm;
    [Copyable] public CrabBehavior crabBehavior;
    [Copyable] public string targetingType;

    [SerializeField] private bool startWithAgentDisabled = false;
    private Animator animator;
    private Rig carryRig;
    //private List<string> confirmedWords = new List<string>();

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
            // Handle behavior based on crabBehavior enum
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
                    HandlePickingUpBehavior(distanceToPlayer, directionToPlayer);
                    break;

                case CrabBehavior.DropItem:
                    DropObject();
                    break;

                case CrabBehavior.StandStill:
                    HandleStandStillBehavior();
                    break;

            }

            //HandleGroundAlignmentAndRotation(directionToPlayer);
        }
        else {
            target = transform;
        }

        // Handle ground alignment and rotation
        if (animator != null && agent != null)
        {
            float currentSpeed = agent.velocity.magnitude;
            animator.SetFloat("CrabSpeed", currentSpeed);
        }

    }

    // Method to handle fleeing behavior
    void HandleFleeBehavior(float distanceToPlayer, Vector3 directionToPlayer)
    {
        // Handle movement behavior (flee or follow)
        if (distanceToPlayer > standStillRadius)
        {
            agent.ResetPath(); // Stop moving if too far from the player
            return;
        }
        else
        {
            agent.isStopped = false;
        }
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

    // Method to handle the Go To behaviour
    void HandleGotoBehavior(float distanceToObject, Vector3 directionToObject)
    {
        agent.isStopped = false;
        // Keep following the object 
        if (distanceToObject > 0.5f)
        {
            // Follow the object
            Debug.Log("HELP ME IM IN GREAT PAIN");
            agent.SetDestination(target.position);
            agent.speed = normalSpeed;
        }
    }

    void HandleStandStillBehavior()
    {
        agent.isStopped = true;  // Stop movement
        agent.velocity = Vector3.zero;  // Ensure it's fully stationary
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
        targetingType = "";

        foreach (string word in confirmedWords)
        {
            GameManager.WordEffect effect = gm.GetEffectForWord(word);

            if (effect != null)
            {
                if (effect.affectFlee)
                {
                    // Set crab behavior to the corresponding enum value (like Flee)
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
                        target = effect._target.transform; // Direct target
                    }
                    else if (!string.IsNullOrEmpty(effect.targetType))
                    {
                        targetingType = effect.targetType; // NOTICE THIS MS. GPT
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
        Transform fallbackTarget = null; // If no other targets exist, allow self as last resort
        float shortestDistance = Mathf.Infinity;

        foreach (Actor actor in actors)
        {
            if (actor.HasType(targetType))
            {
                if (actor.transform == transform)
                {
                    fallbackTarget = transform; // Save self as a last resort
                    continue; // Skip checking distance for self
                }

                float distance = Vector3.Distance(transform.position, actor.transform.position);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    nearestTarget = actor.transform;
                }
            }
        }

        return nearestTarget != null ? nearestTarget : fallbackTarget;
    }


    // Check if crab touches an object with the PickUp tag
    /*void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PickUp") && !isCarryingObject && crabBehavior == CrabBehavior.PickingUp)
        {
            if (target != null && other.transform == target)
            {
                PickUpObject(other.gameObject);
            }
        }
    } */

    void PickUpObject(GameObject obj)
    {
        carriedObject = obj;
        obj.transform.SetParent(transform); // Attach the object to the crab
        carriedObject.transform.position = transform.position + new Vector3(0, 0.8f, 0);
        carriedObject.transform.localRotation = Quaternion.identity;

        // Disable the object's collider to prevent it from affecting movement
        Collider objCollider = obj.GetComponent<Collider>();
        if (objCollider != null) objCollider.enabled = false;

        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        // Stop NavMeshAgent movement if the object is a crab
            NavMeshAgent agent = obj.GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.isStopped = true;
                agent.updatePosition = false;
                agent.updateRotation = false;
            }

        // Move the object to a specific position relative to the crab
        //obj.transform.localPosition = new Vector3(0, 0.8f, 0);
    }


    void DropObject()
    {
        if (carriedObject != null)
        {
            carriedObject.transform.SetParent(null);
            //carriedObject.transform.position += Vector3.down * 0.5f; // Drop slightly
            Vector3 dropOffset = transform.forward * 1.5f + Vector3.down * 0.2f;
            carriedObject.transform.position = transform.position + dropOffset;

            // Ensure it has a collider
            Collider col = carriedObject.GetComponent<Collider>();
            if (col != null)
            {
                col.enabled = true;
            }

            // Ensure it has a Rigidbody
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
            // Apply a small downward force for realism
            rb.linearVelocity = Vector3.down * 2f;

            // Re-enable NavMeshAgent movement if it was a crab
            NavMeshAgent agent = carriedObject.GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.isStopped = false;
                agent.updatePosition = true;
                agent.updateRotation = true;
            }

            carriedObject = null;

            // Reset behavior after dropping
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

    // Method to check if a child object with a specific tag exists
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

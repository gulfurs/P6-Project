using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    public GameObject target;
    public List<string> actorTypes;
    public List<Objective> objectiveList; 
    private Outline outline;

    void Start()
    {
        target = gameObject;
        outline = GetComponent<Outline>();
    }

    public bool HasType(string type)
    {
        return actorTypes.Contains(type);
    }

    void Update()
    {
        if (outline != null) { 
        outline.enabled = ShouldHighlight(); 
        }
    }

    private bool ShouldHighlight()
    {
        if (ObjectiveManager.Instance == null) return false;

        foreach (var activeObjective in ObjectiveManager.Instance.activeObjectives)
        {
            if (objectiveList.Contains(activeObjective.objective))
            {
                return true;
            }
        }
        return false;
    }
}

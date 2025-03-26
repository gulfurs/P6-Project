using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    public GameObject target;
    public List<string> actorTypes;

    void Start()
    {
        target = gameObject;
    }

    public bool HasType(string type)
    {
        return actorTypes.Contains(type);
    }
}

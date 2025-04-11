using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic; 

public class ActorManager : MonoBehaviour
{
    public static ActorManager Instance;

    private Dictionary<Actor, GameObject> actorOriginals = new();

    void Awake() => Instance = this;

    public void RegisterActor(Actor actor)
    {
        GameObject clone = Instantiate(actor.gameObject);
        clone.SetActive(false); // Don't put it in the scene
        actorOriginals[actor] = clone;
    }

    public void ResetActor(Actor actor)
    {
        if (actorOriginals.TryGetValue(actor, out var prefabClone))
        {
            GameObject newActor = Instantiate(prefabClone, actor.transform.position, actor.transform.rotation);
            Destroy(actor.gameObject);
        }
    }

    public void ResetAllForObjective(Objective obj)
    {
        foreach (var pair in actorOriginals)
        {
            if (pair.Key.objectiveList.Contains(obj))
            {
                ResetActor(pair.Key);
            }
        }
    }
}

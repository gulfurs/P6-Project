using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ActorData
{
    public GameObject prefab;
    public Vector3 originalPosition;
    public Quaternion originalRotation;
}


public class ActorManager : MonoBehaviour
{
    public static ActorManager Instance;

    private Dictionary<Actor, ActorData> actorOriginals = new();
    private List<Actor> activeActors = new();

    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    public void RegisterActor(Actor actor)
    {
        if (actorOriginals.ContainsKey(actor)) return;

        GameObject clone = Instantiate(actor.gameObject);
        clone.SetActive(false);
        clone.transform.SetParent(transform);

        actorOriginals[actor] = new ActorData
        {
            prefab = actor.prefabReference,
            originalPosition = actor.transform.position,
            originalRotation = actor.transform.rotation
        };

        activeActors.Add(actor);
    }

    public void ResetActor(Actor actor)
    {
        if (!actorOriginals.TryGetValue(actor, out var data)) return;

        Destroy(actor.gameObject);

        GameObject newActorGO = Instantiate(data.prefab, data.originalPosition, data.originalRotation);
        newActorGO.SetActive(true);

        Actor newActor = newActorGO.GetComponent<Actor>();
        actorOriginals.Remove(actor);
        actorOriginals[newActor] = data;

        activeActors.Remove(actor);
        activeActors.Add(newActor);
    }

    public void ResetAllForObjective(Objective obj)
    {
        List<Actor> toReset = new();

        foreach (var actor in activeActors)
        {
            if (actor != null && actor.objectiveList.Contains(obj))
            {
                toReset.Add(actor);
            }
        }

        foreach (var actor in toReset)
        {
            ResetActor(actor);
        }
    }
}

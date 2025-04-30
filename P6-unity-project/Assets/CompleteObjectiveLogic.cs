using UnityEngine;
using UnityEngine.Playables;
using System.Reflection;

public class ObjectiveCompletionHandler : MonoBehaviour
{
    public Objective objectiveToWatch;
    public Component overrideComponent;

    void Awake()
    {
        ObjectiveManager.OnObjectiveCompleted += OnObjectiveCompleted;
    }

    void OnDestroy()
    {
        ObjectiveManager.OnObjectiveCompleted -= OnObjectiveCompleted;
    }

    private void OnObjectiveCompleted(Objective completedObjective)
    {
        if (completedObjective == objectiveToWatch)
        {
            ApplyCompletionEffects();
        }
    }

    private void ApplyCompletionEffects()
    {
        PlayableDirector director = GetComponent<PlayableDirector>();
        if (director != null && director.playableGraph.IsValid())
        {
            director.Stop();
            Debug.Log($"Stopped PlayableDirector on {gameObject.name}");
        }

        if (overrideComponent == null) return;

        Component myComponent = GetComponent(overrideComponent.GetType());
        if (myComponent == null) return;

        CopyComponentValues(overrideComponent, myComponent);
    }

    private void CopyComponentValues(Component source, Component target)
    {
        FieldInfo[] fields = source.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
        foreach (var field in fields)
        {
            if (!field.IsDefined(typeof(CopyableAttribute), false))
                continue;

            field.SetValue(target, field.GetValue(source));
        }
    }
}

using System.Collections.Generic;
using UnityEngine;

public class CrabStep : TutorialStep
{
    [System.Serializable]
    public class CrabGoal
    {
        public CrabHandler crab;
        public CrabBehavior desiredBehavior;
    }

    public List<CrabGoal> crabGoals;

    public override void UpdateStep(StarterAssetsInputs input)
    {
        base.UpdateStep(input); // Retain typewriter/dialogue flow

        if (stepCompleted) return;

        bool allMatch = true;

        foreach (var goal in crabGoals)
        {
            if (goal.crab == null || goal.crab.crabBehavior != goal.desiredBehavior)
            {
                allMatch = false;
                break;
            }
        }

        if (allMatch)
        {
            stepCompleted = true;
            Debug.Log("All crabs reached their assigned behaviors. Step complete!");
        }
    }
}

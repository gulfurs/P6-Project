using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

[CreateAssetMenu(fileName = "NewObjective", menuName = "Objectives/Objective")]
public class Objective : ScriptableObject
{
    public string objectiveName;
    public string description;
    public int goal;
    public List<Objective> nextObjectives;
    public int difficulty = 1;
    public int xp;
    public Transform objectiveTransform; // XP reward for completing this objective
}
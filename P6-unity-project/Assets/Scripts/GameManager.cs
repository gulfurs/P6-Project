using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using TMPro;

public class GameManager : MonoBehaviour
{
    [System.Serializable]
    public class WordEffect
    {
        public string word;
        public bool fleeBehavior; // If true, crab flees; if false, crab follows
        public GameObject _target; // Determines the target of interest
    }

    public List<WordEffect> wordEffectsList = new List<WordEffect>();
    private Dictionary<string, WordEffect> wordEffectsDictionary = new Dictionary<string, WordEffect>();
    public Animator borders; // Animation reference

    void Start()
    {
        foreach (var effect in wordEffectsList)
        {
            wordEffectsDictionary[effect.word] = effect;
        }
    }

    public WordEffect GetEffectForWord(string word)
    {
        if (wordEffectsDictionary.TryGetValue(word, out WordEffect effect))
        {
            return effect;
        }
        return null; 
    }
}


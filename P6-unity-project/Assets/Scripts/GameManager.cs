using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using TMPro;

public enum CrabBehavior
{
    Flee,       // Crab is fleeing from something
    Follow,     // Crab is following a target
    PickingUp,   // Crab is following an object to pick it up
    DropItem,
    GoTo,
    StandStill,
    Race
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [System.Serializable]
    public class WordEffect
    {
        public string word;
        public CrabBehavior crabBehavior;
        public GameObject _target; // Specific target (if not using type)
        public string targetType;  // Type-based targeting (optional)
        public bool affectFlee = true;  // Toggle whether the effect influences fleeing behavior
        public bool affectTarget = true; // Toggle whether the effect influences the target
        public AudioClip soundword;
        public Sprite wordSprite;
    }

    private AudioSource audioSource; 

    public List<WordEffect> wordEffectsList = new List<WordEffect>();
    private Dictionary<string, WordEffect> wordEffectsDictionary = new Dictionary<string, WordEffect>();
    public Animator borders; 

    void Awake()
    {
        // Check if the instance already exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); 
        }   
        else
        {
            Instance = this; 
            DontDestroyOnLoad(gameObject); 
        }
    }


    void Start()
    {
        audioSource = GetComponent<AudioSource>();

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

    public void PlaySoundForWord(string word)
    {
        WordEffect effect = GetEffectForWord(word);
        if (effect != null && effect.soundword != null && audioSource != null)
        {
            audioSource.PlayOneShot(effect.soundword);
        }
    }
}


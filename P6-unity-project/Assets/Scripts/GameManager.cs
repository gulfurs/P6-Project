using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.Playables;
using TMPro;

public enum CrabBehavior
{
    Flee,
    Follow,
    PickingUp,
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
        public GameObject _target;
        public string targetType;
        public bool affectFlee = true;
        public bool affectTarget = true;
        public AudioClip soundword;
        public Sprite wordSprite;
    }

    private AudioSource audioSource;
    public PlayableDirector timelineToPlay;

    public List<WordEffect> wordEffectsList = new List<WordEffect>();
    private Dictionary<string, WordEffect> wordEffectsDictionary = new Dictionary<string, WordEffect>();
    public Animator borders; 

    void Awake()
    {
        // Check if the instance already exists
        if (Instance != null && Instance != this)
        {
            //Destroy(gameObject); 
        }   
        else
        {
            Instance = this; 
            //DontDestroyOnLoad(gameObject); 
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

    public void OnSliderValueChanged(float value)
    {
        if (Mathf.Approximately(value, 100f))
        {
            if (timelineToPlay != null)
            {
                timelineToPlay.Play();
            }
            else
            {
                Debug.LogWarning("Timeline is not assigned.");
            }
        }
    }
}


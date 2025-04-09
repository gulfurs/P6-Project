using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
public class CutScenePage : MonoBehaviour
 {
    [System.Serializable]
    public class ComicPage
    {
        public Sprite image;  // Background image
        public string[] texts; // Lines of text
    }

    public ComicPage[] pages;  // List of pages
    public Image background;   // Fullscreen background
    public TextMeshProUGUI dialogueText; // Text element

    [Header("Audio")]
    public AudioSource audioSource; // Reference to AudioSource
    public AudioClip typeSound; // The typing sound effect
    [Range(0.1f, 1.0f)]
    public float typeSoundVolume = 0.5f; // Volume control
    [Range(0.5f, 2.0f)]
    public float pitchVariation = 1.1f; // Pitch variation

    [Header("Typing Settings")]
    [Range(0.01f, 0.5f)]
    public float typingSpeed = 0.08f; // Slower typing speed (higher = slower)
    public bool skipPunctuationDelay = false;

    private int currentPageIndex = 0;
    private int currentTextIndex = 0;
    private bool isTyping = false;
    void Start()
    {
            
        ShowPage(0);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Click anywhere
        {
            if (isTyping)
            {
                SkipTyping();
            }
            else
            {
                ShowNextText();
            }
        }
    }

    void ShowPage(int index)
    {
        if (index >= pages.Length) 
        {
            EndCutscene();
            return;
        }

        currentPageIndex = index;
        currentTextIndex = 0;
        background.sprite = pages[index].image;
        StartCoroutine(TypeText(pages[index].texts[currentTextIndex]));
    }

    void ShowNextText()
    {
        ComicPage page = pages[currentPageIndex];

        if (currentTextIndex < page.texts.Length - 1)
        {
            currentTextIndex++;
            StartCoroutine(TypeText(page.texts[currentTextIndex]));
        }
        else
        {
            ShowPage(currentPageIndex + 1);
        }
    }

    IEnumerator TypeText(string text)
    {
        isTyping = true;
        dialogueText.text = "";
        
        foreach (char letter in text)
        {
            dialogueText.text += letter;
            
            // Play sound for each letter, except for spaces and punctuation if desired
            if (letter != ' ' && !char.IsPunctuation(letter) && typeSound != null)
            {
                // Randomize pitch slightly for natural variation
                audioSource.pitch = Random.Range(1.0f, pitchVariation);
                audioSource.PlayOneShot(typeSound, typeSoundVolume);
            }
            
            // Add longer delay for punctuation to create natural pauses
            if (!skipPunctuationDelay && (letter == '.' || letter == '!' || letter == '?' || letter == ','))
            {
                float pauseTime = (letter == ',' || letter == ';') ? typingSpeed * 8 : typingSpeed * 15;
                yield return new WaitForSeconds(pauseTime);
            }
            else
            {
                yield return new WaitForSeconds(typingSpeed); // Slower typing speed
            }
        }
        
        isTyping = false;
    }

    void SkipTyping()
    {
        StopAllCoroutines();
        dialogueText.text = pages[currentPageIndex].texts[currentTextIndex];
        isTyping = false;
    
        // Stop any currently playing typing sounds
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }

    void EndCutscene()
    {
        // Start the coroutine that loads the next scene asynchronously.
        StartCoroutine(LoadNextSceneAsync());
    }

    IEnumerator LoadNextSceneAsync()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        // Start the asynchronous load operation.
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(nextSceneIndex);

        // Optionally, you can wait until the asynchronous operation completes.
        while (!asyncLoad.isDone)
        {
            // Here, you could update a loading bar, if desired.
            yield return null;
        }
    }
}

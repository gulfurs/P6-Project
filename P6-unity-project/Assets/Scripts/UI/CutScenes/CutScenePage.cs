using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

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
            yield return new WaitForSeconds(0.05f); // Speed of typing effect
        }
        isTyping = false;
    }

    void SkipTyping()
    {
        StopAllCoroutines();
        dialogueText.text = pages[currentPageIndex].texts[currentTextIndex];
        isTyping = false;
    }

    void EndCutscene()
    {
        Debug.Log("Cutscene finished, go to gameplay!");
    }
}

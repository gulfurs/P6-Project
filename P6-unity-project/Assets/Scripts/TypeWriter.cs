using System.Collections;
using TMPro;
using UnityEngine;

public class TypeWriter : MonoBehaviour
{
    public TextMeshProUGUI textMesh;
    public float typingSpeed = 0.05f;

    private Coroutine typingCoroutine;
    private string currentFullText;
    public bool isTyping;
    private LogManager logManager;

    void Start()
    {
        logManager = FindObjectOfType<LogManager>();
    }

    public void StartTyping(string fullText)
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        // Format the text with asterisks turned into bold yellow
        currentFullText = FormatText(fullText);
        typingCoroutine = StartCoroutine(TypeText(currentFullText));
    }

    private IEnumerator TypeText(string fullText)
    {
        isTyping = true;
        textMesh.text = "";

        // Directly type the formatted text
        for (int i = 0; i < fullText.Length; i++)
        {
            // Only type out the raw character (not the HTML tags)
            if (fullText[i] == '<')
            {
                // Skip over HTML tags
                while (fullText[i] != '>') i++;
            }
            else
            {
                textMesh.text += fullText[i];
            }
            yield return new WaitForSeconds(typingSpeed);
        }

        // Automatically call SkipTyping once typing is complete
        SkipTyping();
    }

    public void SkipTyping()
    {
        if (isTyping)
        {
            StopCoroutine(typingCoroutine);
            textMesh.text = currentFullText;
            isTyping = false;
        }
    }

    private string FormatText(string text)
    {
        string[] words = text.Split(' ');
        for (int i = 0; i < words.Length; i++)
        {
            if (words[i].StartsWith("*"))
            {
                string cleanWord = words[i].Substring(1); // Remove '*'
                cleanWord = cleanWord.Replace("-", " ");  // Remove dash from the word
                words[i] = $"<b><color=yellow>{cleanWord}</color></b>"; // Apply rich text formatting

                if (logManager != null)
                {
                    logManager.AddWord(cleanWord); // Log the word
                }
            }
        }
        return string.Join(" ", words);
    }
}

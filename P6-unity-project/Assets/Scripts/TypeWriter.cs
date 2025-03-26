using System.Collections;
using TMPro;
using UnityEngine;

public class TypeWriter : MonoBehaviour
{
    public TextMeshProUGUI textMesh;
    public float typingSpeed = 0.05f;

    private Coroutine typingCoroutine;
    private string currentFullText;
    private bool isTyping;

    public void StartTyping(string fullText)
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        currentFullText = fullText;
        typingCoroutine = StartCoroutine(TypeText(fullText));
    }

    private IEnumerator TypeText(string fullText)
    {
        isTyping = true;
        textMesh.text = "";

        foreach (char letter in fullText)
        {
            textMesh.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
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
}

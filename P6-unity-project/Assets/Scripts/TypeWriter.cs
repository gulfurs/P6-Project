using System.Collections;
using TMPro;
using UnityEngine;

public class TypeWriter : MonoBehaviour
{
    public TextMeshProUGUI textMesh;
    public float typingSpeed = 0.05f; // Time between each character

    private Coroutine typingCoroutine;

    public void StartTyping(string fullText)
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(fullText));
    }

    private IEnumerator TypeText(string fullText)
    {
        textMesh.text = ""; // Clear text
        foreach (char letter in fullText)
        {
            textMesh.text += letter;
            yield return new WaitForSeconds(typingSpeed); // Wait before adding next letter
        }
    }
}

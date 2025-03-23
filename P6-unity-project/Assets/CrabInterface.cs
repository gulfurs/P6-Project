using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using TMPro;

public class CrabInterface : MonoBehaviour
{
    public TextMeshProUGUI boardText; // Assign in Inspector
    private HashSet<string> displayedWords = new HashSet<string>();

    private void Start()
    {
        LogManager logManager = FindObjectOfType<LogManager>();
        if (logManager != null)
        {
            logManager.SetCrabInterface(this); // Register itself with LogManager
        }
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape) || Input.GetKey(KeyCode.Tab))
        {
            Destroy(gameObject);
        }
    }

    public void AddWordToBoard(string word)
    {
        if (!displayedWords.Contains(word))
        {
        displayedWords.Add(word);
            boardText.text += (boardText.text == "" ? "" : ", ") + word;
        }
    }
}

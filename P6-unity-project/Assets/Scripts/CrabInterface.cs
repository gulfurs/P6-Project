using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using TMPro;

public class CrabInterface : MonoBehaviour
{
    public TextMeshProUGUI boardText; 
    public Button clearButton;
    public Button confirmButton;
    public CrabHandler crab;
    private HashSet<string> displayedWords = new HashSet<string>();
    private const int maxWords = 2;
    private GameManager gm;

    private void Start()
    {
        LogManager logManager = FindObjectOfType<LogManager>();
        gm = FindObjectOfType<GameManager>();

        if (logManager != null)
        {
            logManager.SetCrabInterface(this); 
        }

        if (clearButton != null)
        {
            clearButton.onClick.AddListener(ClearBoard);
        }

        if (confirmButton != null)
        {
            confirmButton.onClick.AddListener(ConfirmBoardText);
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
        if (displayedWords.Count >= maxWords)
        {
            return; 
        }

        if (!displayedWords.Contains(word))
        {
            displayedWords.Add(word);
            boardText.text = string.Join(", ", displayedWords);
        }
    }

    public void ClearBoard()
    {
        displayedWords.Clear();
        boardText.text = "";
    }

    public void ConfirmBoardText()
    {
        if (crab == null || gm == null)
        {
            return;
        }

        List<string> wordsList = new List<string>(displayedWords);
        crab.ApplyBoardTextEffects(wordsList);
        ClearBoard();
    }
}

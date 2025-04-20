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
    public Animator animator;
    public SpriteRenderer spriteRender;

    private void Start()
    {
        GameObject uiCamObj = GameObject.FindGameObjectWithTag("UI");
        Canvas canvas = GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = uiCamObj.GetComponent<Camera>();
        canvas.planeDistance = 1f;
        canvas.sortingLayerName = "UI";
        canvas.sortingOrder = 500;

        var renderers = GetComponentsInChildren<SpriteRenderer>();
        foreach (var renderer in renderers)
        {
            renderer.sortingLayerName = "UI";
            renderer.sortingOrder += 500;
        }


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
            GameManager.WordEffect effect = gm.GetEffectForWord(word);

            if (effect != null)
            {
                if (effect.affectFlee) {
                    // Switch based on effect.crabBehavior
                    switch (effect.crabBehavior)
                    {
                        case CrabBehavior.Flee:
                        case CrabBehavior.GoTo:
                        case CrabBehavior.Follow:
                            SetBehavior(1);
                            Debug.Log("WE FOLLOW MUSK ON TWITTER");
                            break;
                        case CrabBehavior.PickingUp:
                            SetBehavior(4);
                            Debug.Log("WE PICK UP THE PHONE");
                            break;
                        case CrabBehavior.DropItem:
                            SetBehavior(3);
                            Debug.Log("WE DROP TILTED TOWERS");// Use a different animation for DropItem
                            break;
                        case CrabBehavior.StandStill:
                            SetBehavior(2);
                            Debug.Log("FREEZE!");
                            break;
                        default:
                            SetBehavior(0);  // Default case if the behavior doesn't match
                            break;
                    }
                }

                if (effect.affectTarget)
                {
                    spriteRender.sprite = effect.wordSprite;

                    // Normalize the size to a larger fixed scale
                    if (spriteRender.sprite != null) {
                        float targetSize = 7f;
                        Vector2 spriteSize = effect.wordSprite.bounds.size;

                        float scaleFactor = targetSize / Mathf.Max(spriteSize.x, spriteSize.y);
                        spriteRender.transform.localScale = new Vector3(scaleFactor, scaleFactor, 1);
                    }
                }

            }
            else
            {
                Debug.Log("MISSING MY LOVE");
            }
        }
    }

    public void SetBehavior(int behavior)
    {
        if (animator != null)
        {
            animator.SetFloat("BehaviorIndex", behavior);
        }
    }

    public void ClearBoard()
    {
        displayedWords.Clear();
        boardText.text = "";
        SetBehavior(0);
        spriteRender.sprite = null;
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

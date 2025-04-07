using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine.UI;

public class UI_Diglot : MonoBehaviour
{
    [System.Serializable]
    public class JournalPage
    {
        public string pageTitle;
        [TextArea(3, 10)]
        public string pageContent;
        public List<string> keywords = new List<string>();
        public bool isUnlocked = true; // Default to unlocked since we're unlocking via item pickup
    }

    [Tooltip("Define journal pages")]
    public List<JournalPage> journalPages = new List<JournalPage>();

    [Header("UI References")]
    public TextMeshProUGUI contentTextArea;
    public TextMeshProUGUI titleTextArea;
    public TextMeshProUGUI pageNumberText;
    public Button nextPageButton;
    public Button prevPageButton;
    public Button closeButton;
    public GameObject journalPanel;
    public Color keywordColor = new Color(0.2f, 0.6f, 1f);
    
    [Header("Audio")]
    public AudioClip pageFlipSound;

    private int currentPageIndex = 0;
    private AudioSource audioSource;
    
    void Start()
    {
        // Set up navigation buttons
        if (nextPageButton != null)
            nextPageButton.onClick.AddListener(NextPage);
        
        if (prevPageButton != null)
            prevPageButton.onClick.AddListener(PreviousPage);
            
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseJournal);
            
        // Set up audio source
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && pageFlipSound != null)
            audioSource = gameObject.AddComponent<AudioSource>();
        
        // Display first page
        ShowCurrentPage();
    }

    void Update()
    {
        // Check if journal panel is active
        if (journalPanel != null && journalPanel.activeSelf)
        {
            // Optional keyboard navigation
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.G))
            {
                NextPage();
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.H))
            {
                PreviousPage();
            }
            else if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1)) // ESC or right-click
            {
                CloseJournal();
            }
            
            // Ensure cursor is visible when journal is open
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
    
    public void OpenJournal()
    {
        if (journalPanel != null)
        {
            journalPanel.SetActive(true);
            ShowCurrentPage();
            
            // Make cursor visible for interaction
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
    
    public void CloseJournal()
    {
        if (journalPanel != null)
        {
            journalPanel.SetActive(false);
            
            // Reset cursor state
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
    
    public void NextPage()
    {
        if (currentPageIndex < journalPages.Count - 1)
        {
            currentPageIndex++;
            PlayPageFlipSound();
            ShowCurrentPage();
        }
    }
    
    public void PreviousPage()
    {
        if (currentPageIndex > 0)
        {
            currentPageIndex--;
            PlayPageFlipSound();
            ShowCurrentPage();
        }
    }
    
    private void PlayPageFlipSound()
    {
        // Safe way to play sound that won't break functionality if sound is missing
        if (audioSource != null && pageFlipSound != null)
        {
            audioSource.PlayOneShot(pageFlipSound, 0.5f);
        }
    }
    
    private void ShowCurrentPage()
    {
        if (journalPages.Count == 0)
            return;
            
        // Update navigation button states
        if (nextPageButton != null)
            nextPageButton.interactable = currentPageIndex < journalPages.Count - 1;
            
        if (prevPageButton != null)
            prevPageButton.interactable = currentPageIndex > 0;
            
        // Update page number text
        if (pageNumberText != null)
            pageNumberText.text = $"Page {currentPageIndex + 1} of {journalPages.Count}";
            
        // Display page if it's unlocked
        if (journalPages[currentPageIndex].isUnlocked)
        {
            if (titleTextArea != null)
                titleTextArea.text = journalPages[currentPageIndex].pageTitle;
                
            UpdatePageContent();
        }
    }
    
    private void UpdatePageContent()
    {
        if (contentTextArea == null || currentPageIndex < 0 || currentPageIndex >= journalPages.Count)
            return;
            
        string pageText = journalPages[currentPageIndex].pageContent;
        
        // Highlight all keywords on this page
        foreach (string keyword in journalPages[currentPageIndex].keywords)
        {
            if (!string.IsNullOrEmpty(keyword))
            {
                string colorHex = ColorUtility.ToHtmlStringRGB(keywordColor);
                // Use regex to find the keyword while preserving case
                pageText = Regex.Replace(
                    pageText, 
                    $"({Regex.Escape(keyword)})", 
                    $"<color=#{colorHex}>$1</color>",  
                    RegexOptions.IgnoreCase
                );
            }
        }
        
        contentTextArea.text = pageText;
    }
    
    // Unlock a specific journal page
    public void UnlockPage(int pageIndex)
    {
        if (pageIndex >= 0 && pageIndex < journalPages.Count)
        {
            journalPages[pageIndex].isUnlocked = true;
            
            // If we're currently viewing this page, refresh it
            if (currentPageIndex == pageIndex)
                ShowCurrentPage();
        }
    }
    
    // Get all keywords from this journal
    public List<string> GetAllKeywords()
    {
        List<string> allKeywords = new List<string>();
        
        foreach (var page in journalPages)
        {
            if (page.isUnlocked)
            {
                allKeywords.AddRange(page.keywords);
            }
        }
        
        return allKeywords;
    }
}
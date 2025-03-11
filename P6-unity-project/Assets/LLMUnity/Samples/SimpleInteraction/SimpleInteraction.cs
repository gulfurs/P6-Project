using UnityEngine;
using LLMUnity;
using UnityEngine.UI;
using TMPro;

namespace LLMUnitySamples
{
    public class SimpleInteraction : MonoBehaviour
    {
        public LLMCharacter llmCharacter;
        //public InputField playerText;
        //public Text AIText;

        public TMP_InputField playerText;
        public TextMeshPro AIText;

        [Header("Toggle Settings")]
        public KeyCode toggleKey = KeyCode.T;
        public GameObject chatInterface;

        private bool isChatActive = false;
        //private PlayerMovement playerMovement;

        void Start()
        {
            playerText.onSubmit.AddListener(onInputFieldSubmit);
            //playerText.Select();

            SetChatActive(false);
        }

        void Update()
        {
            if (Input.GetKeyDown(toggleKey))
            {
                SetChatActive(!isChatActive);
            }

            if (isChatActive && Input.GetKeyDown(KeyCode.Escape))
            {
                SetChatActive(false);
            }
        }

        void SetChatActive(bool active)
        {
            isChatActive = active;
            chatInterface.SetActive(active);

            if (active)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                playerText.interactable = true;
                playerText.Select();
            } else {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        void onInputFieldSubmit(string message)
        {
            playerText.interactable = false;
            AIText.text = "...";
            _ = llmCharacter.Chat(message, SetAIText, AIReplyComplete);
        }

        public void SetAIText(string text)
        {
            AIText.text = text;
        }

        public void AIReplyComplete()
        {
            playerText.interactable = true;
            playerText.Select();
            playerText.text = "";
        }

        public void CancelRequests()
        {
            llmCharacter.CancelRequests();
            AIReplyComplete();
        }

        public void ExitGame()
        {
            Debug.Log("Exit button clicked");
            Application.Quit();
        }

        bool onValidateWarning = true;
        void OnValidate()
        {
            if (onValidateWarning && !llmCharacter.remote && llmCharacter.llm != null && llmCharacter.llm.model == "")
            {
                Debug.LogWarning($"Please select a model in the {llmCharacter.llm.gameObject.name} GameObject!");
                onValidateWarning = false;
            }
        }
    }
}

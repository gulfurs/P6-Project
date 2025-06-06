using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LogUI : MonoBehaviour
{
    public TextMeshProUGUI wordText;
    public TMP_InputField definitionInput;
    private LogManager manager;
    private string word;

    public void Setup(string word, string definition, LogManager manager)
    {
        this.word = word;
        this.manager = manager;
        wordText.text = word;
        definitionInput.text = definition;

        definitionInput.onValueChanged.AddListener(UpdateDefinition);
    }

    private void UpdateDefinition(string userInput)
    {
        manager.UpdateUserDefinition(word, userInput);
    }
}

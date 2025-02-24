using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class HexPuzzle : MonoBehaviour, IPointerClickHandler
{
    public TMP_Text[] wordTexts; // Six text fields around
    public Image symbolImage; // Center symbol (draggable)
    public TMP_Text correctTextField; // Correct answer field
    private string correctWord;
    private bool isLockedOn = false;

    private void Start()
    {
        AssignPuzzle();
        AddDragHandlers();
    }

    public void AssignPuzzle()
    {
        int randomIndex = Random.Range(0, wordTexts.Length);
        correctWord = wordTexts[randomIndex].text;
    }

    public void CheckAnswer(TMP_Text droppedText)
    {
        if (droppedText == correctTextField)
        {
            Debug.Log("Correct!");
        }
        else
        {
            Debug.Log("Wrong answer! Try again.");
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        LockOnToPuzzle();
    }

    private void LockOnToPuzzle()
    {
        if (!isLockedOn)
        {
            Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, Camera.main.transform.position.z);
            isLockedOn = true;
        }
    }

    private void AddDragHandlers()
    {
        EventTrigger trigger = symbolImage.gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry dragEntry = new EventTrigger.Entry { eventID = EventTriggerType.Drag };
        dragEntry.callback.AddListener((data) => OnDrag((PointerEventData)data));
        trigger.triggers.Add(dragEntry);

        EventTrigger.Entry endDragEntry = new EventTrigger.Entry { eventID = EventTriggerType.EndDrag };
        endDragEntry.callback.AddListener((data) => OnEndDrag((PointerEventData)data));
        trigger.triggers.Add(endDragEntry);
    }

    public void OnDrag(PointerEventData eventData)
    {
        symbolImage.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        foreach (TMP_Text wordText in wordTexts)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(wordText.rectTransform, eventData.position))
            {
                CheckAnswer(wordText);
                symbolImage.transform.position = symbolImage.transform.parent.position; // Reset position
                return;
            }
        }
        symbolImage.transform.position = symbolImage.transform.parent.position; // Reset position if not dropped on any text field
    }
}
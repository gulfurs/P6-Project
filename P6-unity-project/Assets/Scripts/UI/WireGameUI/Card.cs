using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IPointerDownHandler, IDragHandler, IDropHandler
{

    public Image outline;
    public string GroupName => transform.parent.name;

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("OnDrag");
        PairsManager.Instance.Dragging();
    }


    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("OnDrop");
        //Highlight(false);
        PairsManager.Instance.DroppedOnCard(this);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("OnPointerDown");
        //Highlight(true);
        PairsManager.Instance.CardPicked(this);
    }

    public void Highlight(bool highlight){

        outline.color = highlight ? Color.green : Color.white;
    }






}

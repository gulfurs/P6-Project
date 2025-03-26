using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Connection{

    public Card a;
    public Card b;
    public LineRenderer connectedWith;

    public Connection(Card a, Card b, LineRenderer connectedWith){

        this.a = a;
        this.b = b;
        this.connectedWith = connectedWith;

        Vector2 cardAPos = a.transform.position;
        Vector2 cardBPos = b.transform.position;

        a.Highlight(true);
        b.Highlight(true);

        connectedWith.SetPosition(0, cardAPos);
        connectedWith.SetPosition(1, cardBPos);
        connectedWith.enabled = true;

    }
}



public class PairsManager : MonoBehaviour
{

    public static PairsManager Instance;

    [SerializeField] Transform linesContainer;
    [SerializeField] LineRenderer linePrefab;

    [SerializeField] LineRenderer draggingVisualLine;
    Card draggingCard;

    public int pairsCount = 3;
    LineRenderer[] lines;
    
    List<Connection> connectedPairs = new List<Connection>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start(){
        GenerateLines();
    }

    private void Update(){
        if (Input.GetMouseButtonDown(0)){
            if (draggingCard != null || draggingVisualLine.enabled){
                draggingCard.Highlight(false);
                draggingCard = null;
                draggingVisualLine.enabled = false;
            }
        }
    }
    
    void GenerateLines(){
        lines = new LineRenderer[pairsCount];
        for (int i = 0; i < pairsCount; i++)
        {
            var line = Instantiate(linePrefab, linesContainer);
            line.enabled = false;
            lines[i] = line;
        }
    }

    LineRenderer GetLineFromPool(){
        foreach (LineRenderer line in lines)
        {
            if (line.enabled == false){
                return line;
            }
        }
        return null;
    }

    public void CardPicked(Card card)
    {
        Debug.Log("CardPicked");
        RemoveConnectionIfAlreadyConnected(card);

        draggingCard = card;
        draggingCard.Highlight(true);

        Vector2 cardPos = card.transform.position;
        draggingVisualLine.SetPosition(0, cardPos);
        draggingVisualLine.SetPosition(1, cardPos);

        if (draggingVisualLine.enabled == false){
            draggingVisualLine.enabled = true;
        }

    }

    public void Dragging(){
        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        draggingVisualLine.SetPosition(1, pos);
    }

    public void DroppedOnCard(Card card){

        if (draggingCard.GroupName == card.GroupName){
            return;
        }

        RemoveConnectionIfAlreadyConnected(card);

        if (draggingVisualLine.enabled == true){
            draggingVisualLine.enabled = false;
        }
        draggingCard = null;


        Connection connection = new Connection(draggingCard, card, GetLineFromPool());
        connectedPairs.Add(connection);
        draggingCard = null;

        ValidatePairs();
    }

    void RemoveConnectionIfAlreadyConnected (Card card){
        foreach (Connection connection in connectedPairs)
        {
            if (connection.a == card || connection.b == card){
                connection.a.Highlight(false);
                connection.b.Highlight(false);

                connection.connectedWith.enabled = false;
                connection.connectedWith = null;
                connection.a = null;
                connection.b = null;

                connectedPairs.Remove(connection);
                break;
            }
        }
    }

    void ValidatePairs(){
        if (connectedPairs.Count >= pairsCount){
            Debug.Log("You win!");
            bool allRight = true;
            
            foreach (var pair in connectedPairs){
                if (pair.a.name != pair.b.name){
                    allRight = false;
                    pair.connectedWith.startColor = Color.red;
                    pair.connectedWith.endColor = Color.red;
                } else {
                    pair.connectedWith.startColor = Color.green;
                    pair.connectedWith.endColor = Color.green;
                }
            }

            if (allRight){
                Debug.Log("All pairs are correct!");
            } else {
                Debug.Log("Some pairs are incorrect!");
            }
        }
    }

    private void OnDestroy()
    {
        Instance = null;
    }

}

using UnityEngine;

public class Table : MonoBehaviour
{
    public MemoryManager memoryGame;
    
    private void OnMouseDown()
    {
        // When the table is clicked, activate the memory game
        if (memoryGame != null)
        {
            memoryGame.ActivateMemoryGame();
        }
    }
}
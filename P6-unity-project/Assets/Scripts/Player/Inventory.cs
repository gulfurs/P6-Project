using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public List<Sprite> inventorySprites = new List<Sprite>(); // Stores picked-up item sprites
    //public int maxItems = 3; // Max inventory size

    public Image[] inventorySlots; // UI slots to display items

    void Start()
    {
        UpdateInventoryUI();
    }

    void OnTriggerEnter(Collider other)
    {

        Debug.Log("Entered trigger "); // Debug log

        if (other.CompareTag("Item"))
        {
            ItemPickUp itemPickUp = other.GetComponent<ItemPickUp>();
            Debug.Log("Entered trigger 2 "); // Debug log

            if(itemPickUp != null && itemPickUp.itemSprite != null)
            {
                inventorySprites.Add(itemPickUp.itemSprite);
                Debug.Log("entered trigger 3"); // Debug log
                Destroy(other.gameObject); // Remove item from scene
                UpdateInventoryUI();
            }

        }
    }

    void UpdateInventoryUI()
    {
         Debug.Log("Inventory 1 ");
        for (int i = 0; i < inventorySlots.Length; i++)
        {
             Debug.Log("Inventory 2 ");
            if (i < inventorySprites.Count && inventorySprites[i] != null)
            {
                 Debug.Log("Inventory 3 ");
                inventorySlots[i].sprite = inventorySprites[i]; // Set sprite
                inventorySlots[i].enabled = true;
            }
            else
            {
                inventorySlots[i].sprite = null; // Clear if empty
                inventorySlots[i].enabled = false;
            }
        }
    }
}

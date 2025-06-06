using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    public Transform handTransform; // Where items are held
    public List<EquipmentController> equippedItems = new List<EquipmentController>(2); // Two slots
    private int currentIndex = 0;

    private StarterAssetsInputs inputHandler;

    public EquipmentController defaultController;

    void Start()
    {
        inputHandler = GetComponent<StarterAssetsInputs>();
        equippedItems.Clear();
        equippedItems.Add(defaultController);
        equippedItems.Add(defaultController);
        currentIndex = 0;
    }

    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        if (inputHandler.equip1) EquipItem(0);
        if (inputHandler.equip2) EquipItem(1);

        if (inputHandler.drop)
        {
            inputHandler.drop = false; 
            DropCurrentItem();
        }

        if (Mathf.Abs(inputHandler.cycleInput) > 0.1f) 
        {
            CycleEquipment(inputHandler.cycleInput > 0);
            inputHandler.cycleInput = 0; 
        }
    }


    public void EquipNewItem(EquipmentController newItem)
    {
        if (equippedItems.Count < 2)
        {
            equippedItems.Add(newItem);
            currentIndex = equippedItems.Count - 1;
        }
        else
        {
            int previousIndex = currentIndex; 
            DropCurrentItem();
            equippedItems[previousIndex] = newItem; 
            currentIndex = previousIndex; 
        }

        EquipItem(currentIndex);
        AttachItem(newItem);
    }


    void EquipItem(int index)
    {
        if (index < equippedItems.Count)
        {
            currentIndex = index;
            ActivateCurrentItem();
        }
    }

    void CycleEquipment(bool forward)
    {
        if (equippedItems.Count > 1)
        {
            currentIndex = forward ? (currentIndex + 1) % equippedItems.Count : (currentIndex - 1 + equippedItems.Count) % equippedItems.Count;
            ActivateCurrentItem();
        }
    }

    void ActivateCurrentItem()
    {
        for (int i = 0; i < equippedItems.Count; i++)
        {
            bool shouldBeActive = (i == currentIndex);
            if (equippedItems[i] != null)
            {
                equippedItems[i].gameObject.SetActive(shouldBeActive);

                if (!shouldBeActive && equippedItems[i] == equippedItems[currentIndex])
                {
                    equippedItems[i].gameObject.SetActive(true);
                }
            }
        }
    }


    void DropCurrentItem()
    {
        if (equippedItems.Count == 0) return;

        EquipmentController itemToDrop = equippedItems[currentIndex];

        // Replace the dropped item with the default melee controller
        equippedItems[currentIndex] = defaultController;
        itemToDrop.gameObject.SetActive(false);

        if (itemToDrop.EquipmentRoot != null)
        {
            GameObject root = itemToDrop.EquipmentRoot;
            root.SetActive(true);

            // Get the camera's forward direction
            Vector3 dropDirection = Camera.main.transform.forward;
            Vector3 dropPosition = Camera.main.transform.position + dropDirection * 1.5f + Vector3.down * 0.3f; // Drop slightly below the camera's center

            root.transform.position = dropPosition;

            // Check if it has a Rigidbody
            Rigidbody rb = root.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = root.AddComponent<Rigidbody>(); // Add Rigidbody if missing
            }

            // Apply force in the camera's direction
            rb.AddForce(dropDirection * 3f + Vector3.up * 2f, ForceMode.Impulse);
        }

        /*if (equippedItems.Count > 0)
        {
            currentIndex = 0;
            ActivateCurrentItem();
        }*/
    }



    void AttachItem(EquipmentController item)
    {
        //item.transform.SetParent(handTransform);
        //item.transform.localPosition = Vector3.zero;
        //item.transform.localRotation = Quaternion.identity;
        item.gameObject.SetActive(true);
        item.Equipped = true;
    }
}
using UnityEngine;
using System;
using System.Collections.Generic;

public class EquipInteract : InteractHandler
{
    public EquipmentController equipItem; // Reference to the equipment

    public override void InteractLogic()
    {
        base.InteractLogic();
        interactable = true;
        EquipmentManager equipmentManager = FindObjectOfType<EquipmentManager>(); // Get the EquipmentManager

        if (equipmentManager != null && equipItem != null)
        {
            equipItem.EquipmentRoot = gameObject;
            equipmentManager.EquipNewItem(equipItem); // Send the item to be equipped
            transform.SetParent(null);
            gameObject.SetActive(false); // Hide the pickup object after interacting
        }
    }
}

using UnityEngine;
using System;
using System.Collections.Generic;

public class EquipInteract : InteractHandler
{
    public EquipmentController equipItem;

    public override void InteractLogic()
    {
        base.InteractLogic();
        interactable = true;
        EquipmentManager equipmentManager = FindObjectOfType<EquipmentManager>();

        if (equipmentManager != null && equipItem != null)
        {
            equipItem.EquipmentRoot = gameObject;
            equipmentManager.EquipNewItem(equipItem);
            transform.SetParent(null);
            gameObject.SetActive(false);
        }
    }
}

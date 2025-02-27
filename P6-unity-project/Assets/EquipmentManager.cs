using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class EquipmentManager : MonoBehaviour
{
    [Tooltip("Parent transform where all weapons will be added in the hierarchy")]
    public Transform EquipmentSocket;

    [Header("Equipment")]
    public List<EquipmentController> equipments = new List<EquipmentController>();
    public int equipmentIndex = 0;

    [Header("Input")]
    private StarterAssetsInputs _input;
    public bool isAiming;

    [Tooltip("Secondary camera used to avoid seeing weapon go throw geometries")]
    public Image equipIcon;
    public TextMeshProUGUI ammoText;
    
    /*
    public void AddEquipment() { 
        if (_input.interact)
    } */

    void Start()
    {
        _input = GetComponent<StarterAssetsInputs>();

        EquipWeapon(equipmentIndex);
    }
    void Update()
    {
        // Check for Equip button input, and cycle through the equipment slots
        if (_input.equip1)
        {
            SwitchEquipment(0); // Equip slot 1
        }
        else if (_input.equip2)
        {
            SwitchEquipment(1); // Equip slot 2
        }
        else if (_input.equip3)
        {
            SwitchEquipment(2); // Equip slot 3
        }
    }

    public void SwitchEquipment(int slotIndex)
    {
        // Check if the index is valid
        if (slotIndex < 0 || slotIndex >= equipments.Count) return;

        // Deactivate the current equipment
        equipments[equipmentIndex].gameObject.SetActive(false);

        // Update the equipment index to the selected slot
        equipmentIndex = slotIndex;

        // Activate the new equipment
        EquipWeapon(equipmentIndex);

        // Update UI or other feedback systems (like ammo text)
        UpdateUI();
    }

    public void EquipWeapon(int slotIndex)
    {
        // Ensure the index is valid
        if (slotIndex < 0 || slotIndex >= equipments.Count) return;

        // Activate the selected equipment and place it in the equipment socket
        equipments[slotIndex].transform.SetParent(EquipmentSocket);
        equipments[slotIndex].gameObject.SetActive(true);
    }

    void UpdateUI()
    {
        // Example of updating UI, like ammo count or equipment icon
        if (equipments[equipmentIndex].CurrentAmmo > 0)
        {
            ammoText.text = $"Ammo: {equipments[equipmentIndex].CurrentAmmo}";
        }
        else
        {
            ammoText.text = "Out of Ammo";
        }

        // Update icon based on selected equipment
        equipIcon = equipments[equipmentIndex].GetComponent<EquipmentController>().EquipmentIcon;
    }
}

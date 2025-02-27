using System;
using System.Collections.Generic;
using UnityEngine;
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

    [Tooltip("Secondary camera used to avoid seeing weapon go through geometries")]
    public Image equipIcon;
    public TextMeshProUGUI ammoText;

    void Start()
    {
        _input = GetComponent<StarterAssetsInputs>();
        EquipWeapon(equipmentIndex);  // Equip the starting equipment
    }

    void Update()
    {
        // Check for Equip button input and cycle through the equipment slots
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

        // Raycast to detect and collect equipment when interact button is pressed
        if (_input.interact)
        {
            // Get the ray based on the mouse position or other input method
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

            // Visualize the ray in the scene view (for debugging)
            Debug.DrawRay(ray.origin, ray.direction * 10, Color.red);

            RaycastForEquipment(ray);
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
        equipIcon.sprite = equipments[equipmentIndex].EquipmentIcon.sprite;
    }

    void RaycastForEquipment(Ray ray)
    {
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            EquipmentController collectedEquipment = hit.collider.GetComponent<EquipmentController>();

            if (collectedEquipment != null && !collectedEquipment.Equipped)
            {
                // Only replace equipment in slots 1 or 2
                if (equipmentIndex != 2)
                {
                    collectedEquipment.Equipped = true;

                    // Destroy the collected equipment in the scene (hide it or clean it up)
                    Destroy(collectedEquipment.gameObject);

                    // Create a duplicate of the collected equipment
                    EquipmentController newEquipment = Instantiate(collectedEquipment, collectedEquipment.transform.position, collectedEquipment.transform.rotation);

                    // Optionally, you can adjust the position of the new equipment if needed
                    newEquipment.transform.SetParent(EquipmentSocket); // You might want to position it under the socket or in a more relevant spot

                    ReplaceEquipment(newEquipment);
                }
            }
        }
    }

    public void ReplaceEquipment(EquipmentController newEquipment)
    {
        // Deactivate the current equipment in the selected slot
        equipments[equipmentIndex].gameObject.SetActive(false);

        equipments[equipmentIndex] = newEquipment;

        // Add new equipment to the slot, replace the previous one
        EquipWeapon(equipmentIndex);

        // Optionally, you can update the UI when the equipment is changed
        UpdateUI();
    }
}

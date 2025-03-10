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
    public EquipmentController DefaultEquipment;
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

        //Logic for replacing each object in 'equipments'-list with the DefaultEquipment (1, 2, 3)
        for (int i = 0; i < equipments.Count; i++)
        {
            if (equipments[i] == null)
            {
                equipments[i] = Instantiate(DefaultEquipment);
            }
        }

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

        if (_input.drop)
        {
            DropEquipment();
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
        if (slotIndex < 0 || slotIndex >= equipments.Count) return;

        EquipmentController equipment = equipments[slotIndex];

        // Set the parent and activate the new equipment
        equipment.transform.SetParent(EquipmentSocket);
        equipment.gameObject.SetActive(true);

        // Check if the equipment has a Rigidbody and disable it
        Rigidbody rb = equipment.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true; // Disable physics interaction (effectively disables Rigidbody)
        }

        if (equipment.EquipmentOffset != null)
        {
            equipment.transform.localPosition = equipment.EquipmentOffset.localPosition;
            equipment.transform.localRotation = equipment.EquipmentOffset.localRotation;
        }
        else
        {
            equipment.transform.localPosition = Vector3.zero;
            equipment.transform.localRotation = Quaternion.identity;
        }
    }


    void DropEquipment()
    {
        // Ensure we're not trying to drop slot 3 or an item that isn't a Weapon
        if (equipmentIndex != 2 && equipments[equipmentIndex].EquipType != EquipmentController.EquipmentType.None)
        {
            // Get the currently equipped item
            EquipmentController equipmentToDrop = equipments[equipmentIndex];

            // Instantiate the dropped item at the player's position or slightly offset in front
            Vector3 dropPosition = Camera.main.transform.position + Camera.main.transform.forward * 1.5f; // 1.5 units in front of the player
            Quaternion dropRotation = Camera.main.transform.rotation;

            // Instantiate the equipment to drop
            EquipmentController droppedEquipment = Instantiate(equipmentToDrop, dropPosition, dropRotation);
            droppedEquipment.Equipped = false; // Mark as not equipped

            // Disable and destroy the original equipment
            Destroy(equipmentToDrop.gameObject);

            // Optionally, you can add a Rigidbody or force to make the drop more realistic (e.g., physics)
            Rigidbody rb = droppedEquipment.gameObject.AddComponent<Rigidbody>();
            rb.AddForce(Camera.main.transform.forward * 5f, ForceMode.Impulse); // Apply a small force forward

            // Replace the current equipment with DefaultEquipment
            ReplaceEquipment(DefaultEquipment);

            // Update the UI or handle the dropped item
            UpdateUI();
        }
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

                    Destroy(collectedEquipment.gameObject);

                    EquipmentController newEquipment = Instantiate(collectedEquipment, collectedEquipment.transform.position, collectedEquipment.transform.rotation);

                    
                    newEquipment.transform.SetParent(EquipmentSocket); 

                    ReplaceEquipment(newEquipment);
                }
            }
        }
    }

    public void ReplaceEquipment(EquipmentController newEquipment)
    {
        // Deactivate and destroy the old equipment in the selected slot
        EquipmentController oldEquipment = equipments[equipmentIndex];
        if (oldEquipment != null)
        {
            Destroy(oldEquipment.gameObject); // Completely remove the old equipment from the hierarchy
        }

        // Replace the equipment in the list
        equipments[equipmentIndex] = newEquipment;

        // Equip the new equipment (set it as active and positioned correctly)
        EquipWeapon(equipmentIndex);

        // Optionally, you can update the UI when the equipment is changed
        UpdateUI();
    }
}

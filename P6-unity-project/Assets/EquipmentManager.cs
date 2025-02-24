using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class EquipmentManager : MonoBehaviour
{
    [Tooltip("Parent transform where all weapons will be added in the hierarchy")]
    public Transform EquipmentSocket;

    [Header("Equipment")]
    public List<EquipmentController> equipments = new List<EquipmentController>();

    [Header("Input")]
    private StarterAssetsInputs _input;
    public bool isAiming;

    [Tooltip("Secondary camera used to avoid seeing weapon go throw geometries")]
    public Sprite weaponIcon;
    public TextMeshProUGUI ammoText;

    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
